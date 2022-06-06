using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Enums;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Extensions;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.GUI;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Helpers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.GUI;

public class GuiAndWebViewApplicationSynchronizerBase<TModel, TWindow>
    : GuiAndApplicationSynchronizerBase<TModel, TWindow>, IGuiAndWebViewApplicationSynchronizer<TModel>
    where TModel : class, IWebViewApplicationModelBase {
    protected readonly IWebViewNavigatingHelper WebViewNavigatingHelper;
    protected readonly IMethodNamesFromStackFramesExtractor MethodNamesFromStackFramesExtractor;

    protected GuiAndWebViewApplicationSynchronizerBase(TModel model, TWindow window, ISimpleLogger simpleLogger, IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor)
            : base(model, window, simpleLogger) {
        MethodNamesFromStackFramesExtractor = methodNamesFromStackFramesExtractor;
        WebViewNavigatingHelper = new WebViewNavigatingHelper(Model, SimpleLogger, MethodNamesFromStackFramesExtractor);
    }

    protected override async Task UpdateFieldIfNecessaryAsync(FieldInfo windowField, PropertyInfo modelProperty) {
        switch (windowField.FieldType.Name) {
            case "WebView2":
                await UpdateWebViewIfNecessaryAsync((IWebView)modelProperty.GetValue(Model));
                break;
            default:
                await base.UpdateFieldIfNecessaryAsync(windowField, modelProperty);
                break;
        }
    }

    private async Task UpdateWebViewIfNecessaryAsync(IWebView modelWebView) {
        using (SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(UpdateWebViewIfNecessaryAsync)))) {
            var methodNamesFromStack = MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            if (modelWebView == null) {
                throw new ArgumentNullException(nameof(modelWebView));
            }

            if (!modelWebView.IsWired || modelWebView.Url == modelWebView.LastUrl) {
                return;
            }

            modelWebView.LastUrl = modelWebView.Url;
            SimpleLogger.LogInformationWithCallStack($"Calling webView2.CoreWebView2.Navigate with '{modelWebView.Url}'", methodNamesFromStack);
            var errorsAndInfos = new ErrorsAndInfos();
            await NavigateToUrlAndWaitForStartOfNavigationAsync(modelWebView.Url, errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                Model.Status.Type = StatusType.Error;
                Model.Status.Text = errorsAndInfos.ErrorsToString();
                return;
            }

            await WebViewNavigatingHelper.WaitUntilNotNavigatingAnymoreAsync(modelWebView.LastUrl);
        }
    }

    public async Task<TResult> RunScriptAsync<TResult>(IScriptStatement scriptStatement) where TResult : IScriptCallResponse, new() {
        using (SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(RunScriptAsync) + "Base"))) {
            var methodNamesFromStack = MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            SimpleLogger.LogInformationWithCallStack(Properties.Resources.ExecutingScript, methodNamesFromStack);
            var errorsAndInfos = new ErrorsAndInfos();
            var webView2 = GetWebViewControl(errorsAndInfos, methodNamesFromStack);
            if (webView2 == null) {
                return await Task.FromResult(new TResult { Success = new YesNoInconclusive { Inconclusive = false, YesNo = false }, ErrorMessage = errorsAndInfos.ErrorsToString() });
            }

            var json = await webView2.CoreWebView2.ExecuteScriptAsync(scriptStatement.Statement);
            SimpleLogger.LogInformationWithCallStack(Properties.Resources.ScriptExecutedDeserializingResult, methodNamesFromStack);
            if (string.IsNullOrEmpty(json)) {
                SimpleLogger.LogInformationWithCallStack(Properties.Resources.ScriptCallJsonResultIsEmpty, methodNamesFromStack);
                return await Task.FromResult(new TResult { Success = new YesNoInconclusive { Inconclusive = false, YesNo = false }, ErrorMessage = Properties.Resources.ScriptCallJsonResultIsEmpty });
            }

            try {
                var scriptCallResult = JsonSerializer.Deserialize<TResult>(json);
                if (scriptCallResult is { }) {
                    return scriptCallResult;

                }
                // ReSharper disable once EmptyGeneralCatchClause
            } catch {
            }

            SimpleLogger.LogInformationWithCallStack(Properties.Resources.CouldNotDeserializeScriptCallJsonResult, methodNamesFromStack);
            return await Task.FromResult(new TResult { Success = new YesNoInconclusive { Inconclusive = false, YesNo = false }, ErrorMessage = Properties.Resources.CouldNotDeserializeScriptCallJsonResult });
        }
    }

    public async Task WaitUntilNotNavigatingAnymoreAsync() {
        await WebViewNavigatingHelper.WaitUntilNotNavigatingAnymoreAsync("");
    }

    public async Task NavigateToUrlAndWaitForStartOfNavigationAsync(string url, IErrorsAndInfos errorsAndInfos) {
        using (SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(NavigateToUrlAndWaitForStartOfNavigationAsync)))) {
            var methodNamesFromStack = MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            SimpleLogger.LogInformationWithCallStack(Properties.Resources.NavigatingToUrl, methodNamesFromStack);
            var webView2 = GetWebViewControl(errorsAndInfos, methodNamesFromStack);
            if (webView2 == null) { return; }

            var navigationStarted = false;

            void OnNavigationStarting(object o, CoreWebView2NavigationStartingEventArgs coreWebView2NavigationStartingEventArgs) {
                navigationStarted = true;
            }

            webView2.CoreWebView2.NavigationStarting += OnNavigationStarting;
            webView2.CoreWebView2.Navigate(url);
            const int maxSeconds = 1;
            var timeToGiveUp = DateTime.Now.AddSeconds(maxSeconds);
            while (DateTime.Now < timeToGiveUp && !navigationStarted) {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }
            webView2.CoreWebView2.NavigationStarting -= OnNavigationStarting;
            if (!navigationStarted) {
                errorsAndInfos.Errors.Add(string.Format(Properties.Resources.NotNavigatingAfterSeconds, maxSeconds));
            }
        }
    }

    private WebView2 GetWebViewControl(IErrorsAndInfos errorsAndInfos, IList<string> methodNamesFromStack) {
        var webView2Property = typeof(TModel).GetPropertiesAndInterfaceProperties().FirstOrDefault(p => p.Name == nameof(IWebViewApplicationModelBase.WebView));
        var webView2 = webView2Property == null || !ModelPropertyToWindowFieldMapping.ContainsKey(webView2Property)
            ? null
            : (WebView2)ModelPropertyToWindowFieldMapping[webView2Property].GetValue(Window);
        if (webView2 != null) {
            return webView2;
        }

        SimpleLogger.LogInformationWithCallStack(Properties.Resources.UiDoesNotContainAWebView, methodNamesFromStack);
        errorsAndInfos.Errors.Add(Properties.Resources.UiDoesNotContainAWebView);

        return null;
    }
}