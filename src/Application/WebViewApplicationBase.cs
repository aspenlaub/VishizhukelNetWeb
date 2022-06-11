using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Application;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Enums;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Helpers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using IScriptCallResponse = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces.IScriptCallResponse;
using IScriptStatement = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces.IScriptStatement;
using IWebViewApplicationModelBase = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces.IWebViewApplicationModelBase;
using IWebViewNavigatingHelper = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces.IWebViewNavigatingHelper;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Application;

public abstract class WebViewApplicationBase<TGuiAndApplicationSynchronizer, TModel>
        : ApplicationBase<TGuiAndApplicationSynchronizer, TModel>, IGuiAndWebViewAppHandler<TModel>
            where TModel : class, IWebViewApplicationModelBase
            where TGuiAndApplicationSynchronizer : IGuiAndWebViewApplicationSynchronizer<TModel> {
    protected readonly IWebViewNavigatingHelper WebViewNavigatingHelper;
    protected readonly IMethodNamesFromStackFramesExtractor MethodNamesFromStackFramesExtractor;
    protected readonly IOucidLogAccessor OucidLogAccessor;

    protected WebViewApplicationBase(IButtonNameToCommandMapper buttonNameToCommandMapper,
            IToggleButtonNameToHandlerMapper toggleButtonNameToHandlerMapper,
            TGuiAndApplicationSynchronizer guiAndApplicationSynchronizer, TModel model,
            ISimpleLogger simpleLogger, IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor,
            IOucidLogAccessor oucidLogAccessor)
        : base(buttonNameToCommandMapper, toggleButtonNameToHandlerMapper, guiAndApplicationSynchronizer, model, simpleLogger) {
        MethodNamesFromStackFramesExtractor = methodNamesFromStackFramesExtractor;
        WebViewNavigatingHelper = new WebViewNavigatingHelper(model, simpleLogger, MethodNamesFromStackFramesExtractor);
        OucidLogAccessor = oucidLogAccessor;
    }

    public override async Task OnLoadedAsync() {
        CreateCommandsAndHandlers();
        Model.WebViewUrl.Text = "http://localhost/";
        await EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }

    public async Task OnWebViewSourceChangedAsync(string url) {
        using (SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(OnWebViewSourceChangedAsync)))) {
            var methodNamesFromStack = MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            var urlWithoutOucid = OucidLogAccessor.RemoveOucidFromUrl(url);
            SimpleLogger.LogInformationWithCallStack($"Web view source changes to '{urlWithoutOucid}'", methodNamesFromStack);
            Model.WebView.IsNavigating = url != null;
            Model.WebViewUrl.Text = urlWithoutOucid ?? "(off road)";
            Model.WebViewContentSource.Text = "";
            await EnableOrDisableButtonsThenSyncGuiAndAppAsync();
            SimpleLogger.LogInformationWithCallStack($"GUI navigating to '{urlWithoutOucid}'", methodNamesFromStack);
            IndicateBusy(true);
        }
    }

    public async Task OnWebViewNavigationCompletedAsync(string contentSource, bool isSuccess) {
        using (SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(OnWebViewNavigationCompletedAsync)))) {
            var methodNamesFromStack = MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            var urlWithoutOucid = OucidLogAccessor.RemoveOucidFromUrl(Model.WebViewUrl.Text);
            SimpleLogger.LogInformationWithCallStack($"Web view navigation complete: '{urlWithoutOucid}'", methodNamesFromStack);
            Model.WebView.IsNavigating = false;
            Model.WebViewContentSource.Text = contentSource;
            Model.WebView.HasValidDocument = isSuccess;
            if (!isSuccess) {
                SimpleLogger.LogInformationWithCallStack(Properties.Resources.AppFailed, methodNamesFromStack);
                Model.Status.Text = Properties.Resources.CouldNotLoadUrl;
                Model.Status.Type = StatusType.Error;
            }

            await EnableOrDisableButtonsThenSyncGuiAndAppAsync();
            IndicateBusy(true);
        }
    }

    public async Task<NavigationResult> NavigateToUrlAsync(string url) {
        return await NavigateToUrlAsync(url, new NavigateToUrlSettings());
    }

    public async Task<NavigationResult> NavigateToUrlAsync(string url, NavigateToUrlSettings settings) {
        var methodNamesFromStack = MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
        SimpleLogger.LogInformationWithCallStack($"App navigating to '{url}'", methodNamesFromStack);

        if (Model.WebView.IsNavigating && !await WebViewNavigatingHelper.WaitUntilNotNavigatingAnymoreAsync(url)) {
            return NavigationResult.Failure();
        }

        var errorsAndInfos = new ErrorsAndInfos();
        var oucid = await OucidLogAccessor.GenerateOucidAsync(errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            SimpleLogger.LogInformationWithCallStack("Error generating oucid and writing to oucid log", methodNamesFromStack);
            Model.Status.Text = errorsAndInfos.ErrorsToString();
            Model.Status.Type = StatusType.Error;
            return NavigationResult.Failure(errorsAndInfos);
        }

        var urlWithoutOucid = url;
        url = OucidLogAccessor.AppendOucidToUrl(url, oucid, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            SimpleLogger.LogInformationWithCallStack("Error appending oucid to url", methodNamesFromStack);
            Model.Status.Text = errorsAndInfos.ErrorsToString();
            Model.Status.Type = StatusType.Error;
            return NavigationResult.Failure(errorsAndInfos);
        }

        SimpleLogger.LogInformationWithCallStack(string.Format(Properties.Resources.NavigatingToUrl, urlWithoutOucid), methodNamesFromStack);
        await GuiAndApplicationSynchronizer.NavigateToUrl(url, settings, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            return NavigationResult.Failure(errorsAndInfos);
        }

        if (settings.StopAfterNavigationStarted) {
            return NavigationResult.Success(errorsAndInfos);
        }

        if (!await WebViewNavigatingHelper.WaitUntilNotNavigatingAnymoreAsync(url)) {
            return NavigationResult.Failure();
        }

        var oucidAggregatedResponse = await OucidLogAccessor.ReadAndDeleteOucidAsync(oucid, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            SimpleLogger.LogInformationWithCallStack("Error writing to oucid log", methodNamesFromStack);
            Model.Status.Text = errorsAndInfos.ErrorsToString();
            Model.Status.Type = StatusType.Error;
            return NavigationResult.Failure(errorsAndInfos);
        }

        if (settings.StopAfterOucidResponse) {
            return NavigationResult.Success(errorsAndInfos, oucidAggregatedResponse);
        }

        await EnableOrDisableButtonsThenSyncGuiAndAppAsync();

        Model.Status.Text = "";
        Model.Status.Type = StatusType.None;
        return NavigationResult.Success(errorsAndInfos, oucidAggregatedResponse);
    }

    public async Task<TResult> RunScriptAsync<TResult>(IScriptStatement scriptStatement, bool mayFail, bool maySucceed) where TResult : IScriptCallResponse, new() {
        using (SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(RunScriptAsync)))) {
            var scriptCallResponse = await GuiAndApplicationSynchronizer.RunScriptAsync<TResult>(scriptStatement);

            if (scriptCallResponse.Success.Inconclusive) {
                Model.Status.Text = string.IsNullOrEmpty(scriptStatement.InconclusiveErrorMessage) ? scriptStatement.NoSuccessErrorMessage : scriptStatement.InconclusiveErrorMessage;
                Model.Status.Type = StatusType.Error;
                return scriptCallResponse;
            }

            if (scriptCallResponse.Success.YesNo && maySucceed || !scriptCallResponse.Success.YesNo && mayFail) {
                return scriptCallResponse;
            }

            Model.Status.Text = scriptCallResponse.Success.YesNo ? scriptStatement.NoFailureErrorMessage : scriptStatement.NoSuccessErrorMessage;
            Model.Status.Type = StatusType.Error;
            return scriptCallResponse;
        }
    }

    public async Task WaitUntilNotNavigatingAnymoreAsync() {
        await GuiAndApplicationSynchronizer.WaitUntilNotNavigatingAnymoreAsync();
    }
}