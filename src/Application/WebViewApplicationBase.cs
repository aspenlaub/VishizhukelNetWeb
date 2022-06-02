﻿using System;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
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
    protected readonly IMethodNamesFromStackFramesExtractor MethodNamesFromStackFramesExtractor;

    protected WebViewApplicationBase(IButtonNameToCommandMapper buttonNameToCommandMapper,
            IToggleButtonNameToHandlerMapper toggleButtonNameToHandlerMapper,
            TGuiAndApplicationSynchronizer guiAndApplicationSynchronizer, TModel model,
            ISimpleLogger simpleLogger, IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor)
        : base(buttonNameToCommandMapper, toggleButtonNameToHandlerMapper, guiAndApplicationSynchronizer, model, simpleLogger) {
        MethodNamesFromStackFramesExtractor = methodNamesFromStackFramesExtractor;
        WebViewNavigatingHelper = new WebViewNavigatingHelper(model, simpleLogger, MethodNamesFromStackFramesExtractor);
        WebViewNavigationHelper = new WebViewNavigationHelper<TModel>(model, simpleLogger, this, WebViewNavigatingHelper, MethodNamesFromStackFramesExtractor);
    }

    public override async Task OnLoadedAsync() {
        CreateCommandsAndHandlers();
        Model.WebViewUrl.Text = "http://localhost/";
        await EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }

    public async Task OnWebViewSourceChangedAsync(string uri) {
        using (SimpleLogger.BeginScope(SimpleLoggingScopeId.CreateWithRandomId(nameof(OnWebViewSourceChangedAsync)))) {
            var methodNamesFromStack = MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            SimpleLogger.LogInformationWithCallStack($"Web view source changes to '{uri}'", methodNamesFromStack);
            Model.WebView.IsNavigating = uri != null;
            Model.WebViewUrl.Text = uri ?? "(off road)";
            Model.WebView.LastNavigationStartedAt = DateTime.Now;
            Model.WebViewContentSource.Text = "";
            await EnableOrDisableButtonsThenSyncGuiAndAppAsync();
            SimpleLogger.LogInformationWithCallStack($"GUI navigating to '{Model.WebViewUrl.Text}'", methodNamesFromStack);
            IndicateBusy(true);
        }
    }

    public async Task OnWebViewNavigationCompletedAsync(string contentSource, bool isSuccess) {
        using (SimpleLogger.BeginScope(SimpleLoggingScopeId.CreateWithRandomId(nameof(OnWebViewNavigationCompletedAsync)))) {
            var methodNamesFromStack = MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            SimpleLogger.LogInformationWithCallStack($"Web view navigation complete: '{Model.WebViewUrl.Text}'", methodNamesFromStack);
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

    public async Task<TResult> RunScriptAsync<TResult>(IScriptStatement scriptStatement, bool mayFail, bool maySucceed) where TResult : IScriptCallResponse, new() {
        using (SimpleLogger.BeginScope(SimpleLoggingScopeId.CreateWithRandomId(nameof(RunScriptAsync)))) {
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