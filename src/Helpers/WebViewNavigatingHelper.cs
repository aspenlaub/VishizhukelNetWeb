using System;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Vishizhukel.Interfaces.Application;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Enums;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Helpers;

public class WebViewNavigatingHelper : IWebViewNavigatingHelper {
    public const int QuickSeconds = 5, MaxSeconds = 600;
    private const int IntervalInMilliseconds = 500, LargeIntervalInMilliseconds = 5000;
    private const int DoubleCheckIntervalInMilliseconds = 200, DoubleCheckLargeIntervalInMilliseconds = 1000;

    private readonly IWebViewApplicationModelBase Model;
    private readonly IApplicationLogger ApplicationLogger;

    public WebViewNavigatingHelper(IWebViewApplicationModelBase model, IApplicationLogger applicationLogger) {
        Model = model;
        ApplicationLogger = applicationLogger;
    }

    public async Task<bool> WaitUntilNotNavigatingAnymoreAsync(string url, DateTime minLastUpdateTime) {
        if (Model.WebView is { IsWired: false }) {
            Model.Status.Text = string.Format(Properties.Resources.WebViewMustBeWired, MaxSeconds);
            Model.Status.Type = StatusType.Error;
            ApplicationLogger.LogMessage(url == "" ? Properties.Resources.ProblemWaitingForPotentialNavigationEnd : $"Problem when navigating to '{url}'");
            return false;
        }

        ApplicationLogger.LogMessage(Properties.Resources.WaitUntilNotNavigatingAnymore);
        await WaitUntilNotNavigatingAnymoreAsync(minLastUpdateTime, QuickSeconds, IntervalInMilliseconds, DoubleCheckIntervalInMilliseconds);

        if (Model.WebView.IsNavigating) {
            ApplicationLogger.LogMessage(Properties.Resources.WaitLongerUntilNotNavigatingAnymore);
            await WaitUntilNotNavigatingAnymoreAsync(minLastUpdateTime, MaxSeconds - QuickSeconds, LargeIntervalInMilliseconds, DoubleCheckLargeIntervalInMilliseconds);
        }

        if (!Model.WebView.IsNavigating) {
            ApplicationLogger.LogMessage(Properties.Resources.NotNavigatingAnymore);
            return true;
        }

        Model.Status.Text = string.Format(Properties.Resources.WebViewStillBusyAfter, MaxSeconds);
        Model.Status.Type = StatusType.Error;
        ApplicationLogger.LogMessage(url == "" ? Properties.Resources.ProblemWaitingForPotentialNavigationEnd : $"Problem when navigating to '{url}'");
        return false;

    }

    private async Task WaitUntilNotNavigatingAnymoreAsync(DateTime minLastUpdateTime, int seconds, int intervalInMilliseconds, int doubleCheckIntervalInMilliseconds) {
        var attempts = seconds * 1000 / intervalInMilliseconds;
        bool again;
        do {
            while ((Model.WebView.LastNavigationStartedAt < minLastUpdateTime || Model.WebView.IsNavigating) && attempts > 0) {
                await Task.Delay(TimeSpan.FromMilliseconds(intervalInMilliseconds));
                attempts--;
                if (attempts == 0) {
                    ApplicationLogger.LogMessage($"Still navigating after {seconds} seconds");
                }
            }

            again = false;
            for (var i = 0; !again && i < 5; i ++) {
                await Task.Delay(TimeSpan.FromMilliseconds(doubleCheckIntervalInMilliseconds));
                attempts--;
                if (attempts == 0) {
                    ApplicationLogger.LogMessage($"Still navigating after {seconds} seconds");
                }
                again = Model.WebView.IsNavigating;
                if (again) {
                    ApplicationLogger.LogMessage(Properties.Resources.NavigatingAgain);
                }
            }
        } while (again && attempts > 0);
    }
}