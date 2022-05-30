using System;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Application;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Enums;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Helpers;
using Microsoft.Extensions.Logging;
using IScriptCallResponse = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces.IScriptCallResponse;
using IScriptStatement = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces.IScriptStatement;
using IWebViewApplicationModelBase = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces.IWebViewApplicationModelBase;
using IWebViewNavigatingHelper = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces.IWebViewNavigatingHelper;
using IWebViewNavigationHelper = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces.IWebViewNavigationHelper;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Application;

public abstract class WebViewApplicationBase<TGuiAndApplicationSynchronizer, TModel>
        : ApplicationBase<TGuiAndApplicationSynchronizer, TModel>, Interfaces.IGuiAndWebViewAppHandler<TModel>
            where TModel : class, IWebViewApplicationModelBase
            where TGuiAndApplicationSynchronizer : Interfaces.IGuiAndWebViewApplicationSynchronizer<TModel> {
    protected readonly IWebViewNavigatingHelper WebViewNavigatingHelper;
    protected readonly IWebViewNavigationHelper WebViewNavigationHelper;

    protected WebViewApplicationBase(IButtonNameToCommandMapper buttonNameToCommandMapper,
        IToggleButtonNameToHandlerMapper toggleButtonNameToHandlerMapper,
        TGuiAndApplicationSynchronizer guiAndApplicationSynchronizer, TModel model,
        ISimpleLogger simpleLogger)
        : base(buttonNameToCommandMapper, toggleButtonNameToHandlerMapper, guiAndApplicationSynchronizer, model, simpleLogger) {
        WebViewNavigatingHelper = new WebViewNavigatingHelper(model, simpleLogger);
        WebViewNavigationHelper = new WebViewNavigationHelper<TModel>(model, simpleLogger, this, WebViewNavigatingHelper);
    }

    public override async Task OnLoadedAsync() {
        CreateCommandsAndHandlers();
        Model.WebViewUrl.Text = "http://localhost/";
        await EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }

    public async Task OnWebViewSourceChangedAsync(string uri) {
        using (SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(OnWebViewSourceChangedAsync), SimpleLogger.LogId))) {
            SimpleLogger.LogInformation($"Web view source changes to '{uri}'");
            Model.WebView.IsNavigating = uri != null;
            Model.WebViewUrl.Text = uri ?? "(off road)";
            Model.WebView.LastNavigationStartedAt = DateTime.Now;
            Model.WebViewContentSource.Text = "";
            await EnableOrDisableButtonsThenSyncGuiAndAppAsync();
            SimpleLogger.LogInformation($"GUI navigating to '{Model.WebViewUrl.Text}'");
            IndicateBusy(true);
        }
    }

    public async Task OnWebViewNavigationCompletedAsync(string contentSource, bool isSuccess) {
        using (SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(OnWebViewNavigationCompletedAsync), SimpleLogger.LogId))) {
            SimpleLogger.LogInformation($"Web view navigation complete: '{Model.WebViewUrl.Text}'");
            Model.WebView.IsNavigating = false;
            Model.WebViewContentSource.Text = contentSource;
            Model.WebView.HasValidDocument = isSuccess;
            if (!isSuccess) {
                SimpleLogger.LogInformation(Properties.Resources.AppFailed);
                Model.Status.Text = Properties.Resources.CouldNotLoadUrl;
                Model.Status.Type = StatusType.Error;
            }

            await EnableOrDisableButtonsThenSyncGuiAndAppAsync();
            IndicateBusy(true);
        }
    }

    public async Task<TResult> RunScriptAsync<TResult>(IScriptStatement scriptStatement, bool mayFail, bool maySucceed) where TResult : IScriptCallResponse, new() {
        using (SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(RunScriptAsync), SimpleLogger.LogId))) {
            var scriptCallResponse = await GuiAndApplicationSynchronizer.RunScriptAsync<TResult>(scriptStatement);

            if (scriptCallResponse.Success.Inconclusive) {
                Model.Status.Text = string.IsNullOrEmpty(scriptStatement.InconclusiveErrorMessage) ? scriptStatement.NoSuccessErrorMessage : scriptStatement.InconclusiveErrorMessage;
                Model.Status.Type = StatusType.Error;
                return scriptCallResponse;
            }

            if ((scriptCallResponse.Success.YesNo && maySucceed) || (!scriptCallResponse.Success.YesNo && mayFail)) {
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