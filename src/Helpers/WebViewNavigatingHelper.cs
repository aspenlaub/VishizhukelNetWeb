using System;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Enums;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Helpers;

public class WebViewNavigatingHelper : IWebViewNavigatingHelper {
    public const int QuickSeconds = 5, MaxSeconds = 600;
    private const int IntervalInMilliseconds = 500, LargeIntervalInMilliseconds = 5000;
    private const int DoubleCheckIntervalInMilliseconds = 200, DoubleCheckLargeIntervalInMilliseconds = 1000;

    private readonly IWebViewApplicationModelBase Model;
    private readonly ISimpleLogger SimpleLogger;
    private readonly IMethodNamesFromStackFramesExtractor MethodNamesFromStackFramesExtractor;

    public WebViewNavigatingHelper(IWebViewApplicationModelBase model, ISimpleLogger simpleLogger, IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor) {
        Model = model;
        SimpleLogger = simpleLogger;
        MethodNamesFromStackFramesExtractor = methodNamesFromStackFramesExtractor;
    }

    public async Task<bool> WaitUntilNotNavigatingAnymoreAsync(string url, DateTime minLastUpdateTime) {
        using (SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(WaitUntilNotNavigatingAnymoreAsync), SimpleLogger.LogId))) {
            var methodNamesFromStack = MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            if (Model.WebView is { IsWired: false }) {
                Model.Status.Text = string.Format(Properties.Resources.WebViewMustBeWired, MaxSeconds);
                Model.Status.Type = StatusType.Error;
                SimpleLogger.LogInformationWithCallStack(url == "" ? Properties.Resources.ProblemWaitingForPotentialNavigationEnd : $"Problem when navigating to '{url}'", methodNamesFromStack);
                return false;
            }

            SimpleLogger.LogInformationWithCallStack(Properties.Resources.WaitUntilNotNavigatingAnymore, methodNamesFromStack);
            await WaitUntilNotNavigatingAnymoreAsync(minLastUpdateTime, QuickSeconds, IntervalInMilliseconds, DoubleCheckIntervalInMilliseconds);

            if (Model.WebView.IsNavigating) {
                SimpleLogger.LogInformationWithCallStack(Properties.Resources.WaitLongerUntilNotNavigatingAnymore, methodNamesFromStack);
                await WaitUntilNotNavigatingAnymoreAsync(minLastUpdateTime, MaxSeconds - QuickSeconds, LargeIntervalInMilliseconds, DoubleCheckLargeIntervalInMilliseconds);
            }

            if (!Model.WebView.IsNavigating) {
                SimpleLogger.LogInformationWithCallStack(Properties.Resources.NotNavigatingAnymore, methodNamesFromStack);
                return true;
            }

            Model.Status.Text = string.Format(Properties.Resources.WebViewStillBusyAfter, MaxSeconds);
            Model.Status.Type = StatusType.Error;
            SimpleLogger.LogInformationWithCallStack(url == "" ? Properties.Resources.ProblemWaitingForPotentialNavigationEnd : $"Problem when navigating to '{url}'", methodNamesFromStack);
            return false;
        }
    }

    private async Task WaitUntilNotNavigatingAnymoreAsync(DateTime minLastUpdateTime, int seconds, int intervalInMilliseconds, int doubleCheckIntervalInMilliseconds) {
        var attempts = seconds * 1000 / intervalInMilliseconds;
        bool again;
        var methodNamesFromStack = MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
        do {
            while ((Model.WebView.LastNavigationStartedAt < minLastUpdateTime || Model.WebView.IsNavigating) && attempts > 0) {
                await Task.Delay(TimeSpan.FromMilliseconds(intervalInMilliseconds));
                attempts--;
                if (attempts == 0) {
                    SimpleLogger.LogInformationWithCallStack($"Still navigating after {seconds} seconds", methodNamesFromStack);
                }
            }

            again = false;
            for (var i = 0; !again && i < 5; i ++) {
                await Task.Delay(TimeSpan.FromMilliseconds(doubleCheckIntervalInMilliseconds));
                attempts--;
                if (attempts == 0) {
                    SimpleLogger.LogInformationWithCallStack($"Still navigating after {seconds} seconds", methodNamesFromStack);
                }
                again = Model.WebView.IsNavigating;
                if (again) {
                    SimpleLogger.LogInformationWithCallStack(Properties.Resources.NavigatingAgain, methodNamesFromStack);
                }
            }
        } while (again && attempts > 0);
    }
}