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

    private readonly IWebViewApplicationModelBase _Model;
    private readonly ISimpleLogger _SimpleLogger;
    private readonly IMethodNamesFromStackFramesExtractor _MethodNamesFromStackFramesExtractor;

    public WebViewNavigatingHelper(IWebViewApplicationModelBase model, ISimpleLogger simpleLogger, IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor) {
        _Model = model;
        _SimpleLogger = simpleLogger;
        _MethodNamesFromStackFramesExtractor = methodNamesFromStackFramesExtractor;
    }

    public async Task<bool> WaitUntilNotNavigatingAnymoreAsync(string url) {
        using (_SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(WaitUntilNotNavigatingAnymoreAsync)))) {
            var methodNamesFromStack = _MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            if (_Model.WebView is { IsWired: false }) {
                _Model.Status.Text = string.Format(Properties.Resources.WebViewMustBeWired, MaxSeconds);
                _Model.Status.Type = StatusType.Error;
                _SimpleLogger.LogInformationWithCallStack(url == "" ? Properties.Resources.ProblemWaitingForPotentialNavigationEnd : $"Problem when navigating to '{url}'", methodNamesFromStack);
                return false;
            }

            _SimpleLogger.LogInformationWithCallStack(Properties.Resources.WaitUntilNotNavigatingAnymore, methodNamesFromStack);
            await WaitUntilNotNavigatingAnymoreAsync(QuickSeconds, IntervalInMilliseconds, DoubleCheckIntervalInMilliseconds);

            if (_Model.WebView.IsNavigating) {
                _SimpleLogger.LogInformationWithCallStack(Properties.Resources.WaitLongerUntilNotNavigatingAnymore, methodNamesFromStack);
                await WaitUntilNotNavigatingAnymoreAsync(MaxSeconds - QuickSeconds, LargeIntervalInMilliseconds, DoubleCheckLargeIntervalInMilliseconds);
            }

            if (!_Model.WebView.IsNavigating) {
                _SimpleLogger.LogInformationWithCallStack(Properties.Resources.NotNavigatingAnymore, methodNamesFromStack);
                return true;
            }

            _Model.Status.Text = string.Format(Properties.Resources.WebViewStillBusyAfter, MaxSeconds);
            _Model.Status.Type = StatusType.Error;
            _SimpleLogger.LogInformationWithCallStack(url == "" ? Properties.Resources.ProblemWaitingForPotentialNavigationEnd : $"Problem when navigating to '{url}'", methodNamesFromStack);
            return false;
        }
    }

    private async Task WaitUntilNotNavigatingAnymoreAsync(int seconds, int intervalInMilliseconds, int doubleCheckIntervalInMilliseconds) {
        var attempts = seconds * 1000 / intervalInMilliseconds;
        bool again;
        var methodNamesFromStack = _MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
        do {
            while (_Model.WebView.IsNavigating && attempts > 0) {
                await Task.Delay(TimeSpan.FromMilliseconds(intervalInMilliseconds));
                attempts--;
                if (attempts == 0) {
                    _SimpleLogger.LogInformationWithCallStack($"Still navigating after {seconds} seconds", methodNamesFromStack);
                }
            }

            again = false;
            for (var i = 0; !again && i < 5; i ++) {
                await Task.Delay(TimeSpan.FromMilliseconds(doubleCheckIntervalInMilliseconds));
                attempts--;
                if (attempts == 0) {
                    _SimpleLogger.LogInformationWithCallStack($"Still navigating after {seconds} seconds", methodNamesFromStack);
                }
                again = _Model.WebView.IsNavigating;
                if (again) {
                    _SimpleLogger.LogInformationWithCallStack(Properties.Resources.NavigatingAgain, methodNamesFromStack);
                }
            }
        } while (again && attempts > 0);
    }
}