using System;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Vishizhukel.Interfaces.Application;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Application;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Enums;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Helpers;
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
        IApplicationLogger applicationLogger)
        : base(buttonNameToCommandMapper, toggleButtonNameToHandlerMapper, guiAndApplicationSynchronizer, model,
            applicationLogger) {
        WebViewNavigatingHelper = new WebViewNavigatingHelper(model, applicationLogger);
        WebViewNavigationHelper = new WebViewNavigationHelper<TModel>(model, applicationLogger, this, WebViewNavigatingHelper);
    }

    public override async Task OnLoadedAsync() {
        CreateCommandsAndHandlers();
        Model.WebViewUrl.Text = "http://localhost/";
        await EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }

    public async Task OnWebViewSourceChangedAsync(string uri) {
        ApplicationLogger.LogMessage($"Web view source changes to '{uri}'");
        Model.WebView.IsNavigating = uri != null;
        Model.WebViewUrl.Text = uri ?? "(off road)";
        Model.WebView.LastNavigationStartedAt = DateTime.Now;
        Model.WebViewContentSource.Text = "";
        await EnableOrDisableButtonsThenSyncGuiAndAppAsync();
        ApplicationLogger.LogMessage($"GUI navigating to '{Model.WebViewUrl.Text}'");
        IndicateBusy(true);
    }

    public async Task OnWebViewNavigationCompletedAsync(string contentSource, bool isSuccess) {
        ApplicationLogger.LogMessage($"Web view navigation complete: '{Model.WebViewUrl.Text}'");
        Model.WebView.IsNavigating = false;
        Model.WebViewContentSource.Text = contentSource;
        Model.WebView.HasValidDocument = isSuccess;
        if (!isSuccess) {
            ApplicationLogger.LogMessage(Properties.Resources.AppFailed);
            Model.Status.Text = Properties.Resources.CouldNotLoadUrl;
            Model.Status.Type = StatusType.Error;
        }

        await EnableOrDisableButtonsThenSyncGuiAndAppAsync();
        IndicateBusy(true);
    }

    public async Task<TResult> RunScriptAsync<TResult>(IScriptStatement scriptStatement, bool mayFail, bool maySucceed) where TResult : IScriptCallResponse, new() {
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

    public async Task WaitUntilNotNavigatingAnymoreAsync() {
        await GuiAndApplicationSynchronizer.WaitUntilNotNavigatingAnymoreAsync();
    }
}