using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Enums;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Helpers;

public class WebViewNavigatingHelper(IWebViewApplicationModelBase model, ISimpleLogger simpleLogger,
        IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor)
            : IWebViewNavigatingHelper {
    public const int QuickSeconds = 5, MaxSeconds = 600;
    private const int _intervalInMilliseconds = 500, _largeIntervalInMilliseconds = 5000;
    private const int _doubleCheckIntervalInMilliseconds = 200, _doubleCheckLargeIntervalInMilliseconds = 1000;

    public async Task<bool> WaitUntilNotNavigatingAnymoreAsync(string url) {
        using (simpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(WaitUntilNotNavigatingAnymoreAsync)))) {
            IList<string> methodNamesFromStack = methodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            if (model.WebView is { IsWired: false }) {
                model.Status.Text = string.Format(Properties.Resources.WebViewMustBeWired, MaxSeconds);
                model.Status.Type = StatusType.Error;
                simpleLogger.LogInformationWithCallStack(url == "" ? Properties.Resources.ProblemWaitingForPotentialNavigationEnd : $"Problem when navigating to '{url}'", methodNamesFromStack);
                return false;
            }

            simpleLogger.LogInformationWithCallStack(Properties.Resources.WaitUntilNotNavigatingAnymore, methodNamesFromStack);
            await WaitUntilNotNavigatingAnymoreAsync(QuickSeconds, _intervalInMilliseconds, _doubleCheckIntervalInMilliseconds);

            if (model.WebView.IsNavigating) {
                simpleLogger.LogInformationWithCallStack(Properties.Resources.WaitLongerUntilNotNavigatingAnymore, methodNamesFromStack);
                await WaitUntilNotNavigatingAnymoreAsync(MaxSeconds - QuickSeconds, _largeIntervalInMilliseconds, _doubleCheckLargeIntervalInMilliseconds);
            }

            if (!model.WebView.IsNavigating) {
                simpleLogger.LogInformationWithCallStack(Properties.Resources.NotNavigatingAnymore, methodNamesFromStack);
                return true;
            }

            model.Status.Text = string.Format(Properties.Resources.WebViewStillBusyAfter, MaxSeconds);
            model.Status.Type = StatusType.Error;
            simpleLogger.LogInformationWithCallStack(url == "" ? Properties.Resources.ProblemWaitingForPotentialNavigationEnd : $"Problem when navigating to '{url}'", methodNamesFromStack);
            return false;
        }
    }

    private async Task WaitUntilNotNavigatingAnymoreAsync(int seconds, int intervalInMilliseconds, int doubleCheckIntervalInMilliseconds) {
        int attempts = seconds * 1000 / intervalInMilliseconds;
        bool again;
        IList<string> methodNamesFromStack = methodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
        do {
            while (model.WebView.IsNavigating && attempts > 0) {
                await Task.Delay(TimeSpan.FromMilliseconds(intervalInMilliseconds));
                attempts--;
                if (attempts == 0) {
                    simpleLogger.LogInformationWithCallStack($"Still navigating after {seconds} seconds", methodNamesFromStack);
                }
            }

            again = false;
            for (int i = 0; !again && i < 5; i ++) {
                await Task.Delay(TimeSpan.FromMilliseconds(doubleCheckIntervalInMilliseconds));
                attempts--;
                if (attempts == 0) {
                    simpleLogger.LogInformationWithCallStack($"Still navigating after {seconds} seconds", methodNamesFromStack);
                }
                again = model.WebView.IsNavigating;
                if (again) {
                    simpleLogger.LogInformationWithCallStack(Properties.Resources.NavigatingAgain, methodNamesFromStack);
                }
            }
        } while (again && attempts > 0);
    }
}