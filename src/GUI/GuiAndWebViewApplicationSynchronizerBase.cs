using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Extensions;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.GUI;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;
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
    protected readonly IOucidLogAccessor OucidLogAccessor;

    protected GuiAndWebViewApplicationSynchronizerBase(TModel model, TWindow window, ISimpleLogger simpleLogger,
                IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor, IOucidLogAccessor oucidLogAccessor)
                    : base(model, window, simpleLogger) {
        MethodNamesFromStackFramesExtractor = methodNamesFromStackFramesExtractor;
        OucidLogAccessor = oucidLogAccessor;
        WebViewNavigatingHelper = new WebViewNavigatingHelper(Model, SimpleLogger, MethodNamesFromStackFramesExtractor);
    }

    protected override async Task UpdateFieldIfNecessaryAsync(FieldInfo windowField, PropertyInfo modelProperty) {
        switch (windowField.FieldType.Name) {
            case "WebView2":
                break;
            default:
                await base.UpdateFieldIfNecessaryAsync(windowField, modelProperty);
                break;
        }
    }

    public async Task<TResult> RunScriptAsync<TResult>(IScriptStatement scriptStatement) where TResult : IScriptCallResponse, new() {
        if (scriptStatement == null) {
            return await Task.FromResult(new TResult { Success = new YesNoInconclusive { Inconclusive = false, YesNo = false }, ErrorMessage = Properties.Resources.NoScriptStatementProvided });
        }

        using (SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(RunScriptAsync) + "Base"))) {
            IList<string> methodNamesFromStack = MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            SimpleLogger.LogInformationWithCallStack(Properties.Resources.ExecutingScript, methodNamesFromStack);
            var errorsAndInfos = new ErrorsAndInfos();
            WebView2 webView2 = GetWebViewControl(errorsAndInfos, methodNamesFromStack);
            if (webView2 == null) {
                return await Task.FromResult(new TResult { Success = new YesNoInconclusive { Inconclusive = false, YesNo = false }, ErrorMessage = errorsAndInfos.ErrorsToString() });
            }

            string json = await webView2.CoreWebView2.ExecuteScriptAsync(scriptStatement.Statement);
            SimpleLogger.LogInformationWithCallStack(Properties.Resources.ScriptExecutedDeserializingResult, methodNamesFromStack);
            if (string.IsNullOrEmpty(json)) {
                SimpleLogger.LogInformationWithCallStack(Properties.Resources.ScriptCallJsonResultIsEmpty, methodNamesFromStack);
                return await Task.FromResult(new TResult { Success = new YesNoInconclusive { Inconclusive = false, YesNo = false }, ErrorMessage = Properties.Resources.ScriptCallJsonResultIsEmpty });
            }

            try {
                TResult scriptCallResult = JsonSerializer.Deserialize<TResult>(json);
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

    public async Task NavigateToUrl(string url, NavigateToUrlSettings settings, IErrorsAndInfos errorsAndInfos) {
        using (SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(NavigateToUrl)))) {
            IList<string> methodNamesFromStack = MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            string urlWithoutOucid = OucidLogAccessor.RemoveOucidFromUrl(url);
            SimpleLogger.LogInformationWithCallStack(string.Format(Properties.Resources.NavigatingToUrl, urlWithoutOucid), methodNamesFromStack);
            WebView2 webView2 = GetWebViewControl(errorsAndInfos, methodNamesFromStack);
            if (webView2 == null) { return; }

            bool navigationStarted = false;

            void OnNavigationStarting(object o, CoreWebView2NavigationStartingEventArgs coreWebView2NavigationStartingEventArgs) {
                navigationStarted = true;
            }

            webView2.CoreWebView2.NavigationStarting += OnNavigationStarting;
            webView2.CoreWebView2.Navigate(url);
            const int maxSeconds = 1;
            DateTime timeToGiveUp = DateTime.Now.AddSeconds(maxSeconds);
            while (DateTime.Now < timeToGiveUp && !navigationStarted) {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }
            webView2.CoreWebView2.NavigationStarting -= OnNavigationStarting;
            if (!navigationStarted) {
                errorsAndInfos.Errors.Add(string.Format(Properties.Resources.NotNavigatingAfterSeconds, maxSeconds));
            }

            // if (settings.StopAfterNavigationStarted) { return; }
        }
    }

    private WebView2 GetWebViewControl(IErrorsAndInfos errorsAndInfos, IList<string> methodNamesFromStack) {
        PropertyInfo webView2Property = typeof(TModel).GetPropertiesAndInterfaceProperties().FirstOrDefault(p => p.Name == nameof(IWebViewApplicationModelBase.WebView));
        WebView2 webView2 = webView2Property == null || !ModelPropertyToWindowFieldMapping.ContainsKey(webView2Property)
            ? null
            : (WebView2)ModelPropertyToWindowFieldMapping[webView2Property].GetValue(Window);
        if (webView2?.CoreWebView2 != null) {
            return webView2;
        }

        SimpleLogger.LogInformationWithCallStack(Properties.Resources.UiDoesNotContainAWebView, methodNamesFromStack);
        errorsAndInfos.Errors.Add(Properties.Resources.UiDoesNotContainAWebView);

        return null;
    }
}