using System;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Enums;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Microsoft.Extensions.Logging;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Helpers;

public class WebViewNavigatingHelper : IWebViewNavigatingHelper {
    public const int QuickSeconds = 5, MaxSeconds = 600;
    private const int IntervalInMilliseconds = 500, LargeIntervalInMilliseconds = 5000;
    private const int DoubleCheckIntervalInMilliseconds = 200, DoubleCheckLargeIntervalInMilliseconds = 1000;

    private readonly IWebViewApplicationModelBase Model;
    private readonly ISimpleLogger SimpleLogger;
    private readonly ILogConfigurationFactory LogConfigurationFactory;

    public WebViewNavigatingHelper(IWebViewApplicationModelBase model, ISimpleLogger simpleLogger, ILogConfigurationFactory logConfigurationFactory) {
        Model = model;
        SimpleLogger = simpleLogger;
        LogConfigurationFactory = logConfigurationFactory;
    }

    public async Task<bool> WaitUntilNotNavigatingAnymoreAsync(string url, DateTime minLastUpdateTime) {
        var logConfiguration = LogConfigurationFactory.Create();
        using (SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(WaitUntilNotNavigatingAnymoreAsync), logConfiguration.LogId))) {
            if (Model.WebView is { IsWired: false }) {
                Model.Status.Text = string.Format(Properties.Resources.WebViewMustBeWired, MaxSeconds);
                Model.Status.Type = StatusType.Error;
                SimpleLogger.LogInformation(url == "" ? Properties.Resources.ProblemWaitingForPotentialNavigationEnd : $"Problem when navigating to '{url}'");
                return false;
            }

            SimpleLogger.LogInformation(Properties.Resources.WaitUntilNotNavigatingAnymore);
            await WaitUntilNotNavigatingAnymoreAsync(minLastUpdateTime, QuickSeconds, IntervalInMilliseconds, DoubleCheckIntervalInMilliseconds);

            if (Model.WebView.IsNavigating) {
                SimpleLogger.LogInformation(Properties.Resources.WaitLongerUntilNotNavigatingAnymore);
                await WaitUntilNotNavigatingAnymoreAsync(minLastUpdateTime, MaxSeconds - QuickSeconds, LargeIntervalInMilliseconds, DoubleCheckLargeIntervalInMilliseconds);
            }

            if (!Model.WebView.IsNavigating) {
                SimpleLogger.LogInformation(Properties.Resources.NotNavigatingAnymore);
                return true;
            }

            Model.Status.Text = string.Format(Properties.Resources.WebViewStillBusyAfter, MaxSeconds);
            Model.Status.Type = StatusType.Error;
            SimpleLogger.LogInformation(url == "" ? Properties.Resources.ProblemWaitingForPotentialNavigationEnd : $"Problem when navigating to '{url}'");
            return false;
        }
    }

    private async Task WaitUntilNotNavigatingAnymoreAsync(DateTime minLastUpdateTime, int seconds, int intervalInMilliseconds, int doubleCheckIntervalInMilliseconds) {
        var attempts = seconds * 1000 / intervalInMilliseconds;
        bool again;
        do {
            while ((Model.WebView.LastNavigationStartedAt < minLastUpdateTime || Model.WebView.IsNavigating) && attempts > 0) {
                await Task.Delay(TimeSpan.FromMilliseconds(intervalInMilliseconds));
                attempts--;
                if (attempts == 0) {
                    SimpleLogger.LogInformation($"Still navigating after {seconds} seconds");
                }
            }

            again = false;
            for (var i = 0; !again && i < 5; i ++) {
                await Task.Delay(TimeSpan.FromMilliseconds(doubleCheckIntervalInMilliseconds));
                attempts--;
                if (attempts == 0) {
                    SimpleLogger.LogInformation($"Still navigating after {seconds} seconds");
                }
                again = Model.WebView.IsNavigating;
                if (again) {
                    SimpleLogger.LogInformation(Properties.Resources.NavigatingAgain);
                }
            }
        } while (again && attempts > 0);
    }
}