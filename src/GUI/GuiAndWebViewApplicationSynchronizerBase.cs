using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Extensions;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.GUI;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Helpers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
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
                await UpdateWebViewIfNecessaryAsync((IWebView)modelProperty.GetValue(Model),
                    (WebView2)windowField.GetValue(Window));
                break;
            default:
                await base.UpdateFieldIfNecessaryAsync(windowField, modelProperty);
                break;
        }
    }

    private async Task UpdateWebViewIfNecessaryAsync(IWebView modelWebView, WebView2 webView2) {
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
            var minLastUpdateTime = DateTime.Now;
            webView2.CoreWebView2?.Navigate(modelWebView.Url);

            await WebViewNavigatingHelper.WaitUntilNotNavigatingAnymoreAsync(modelWebView.LastUrl, minLastUpdateTime);
        }
    }

    public async Task<TResult> RunScriptAsync<TResult>(IScriptStatement scriptStatement) where TResult : IScriptCallResponse, new() {
        using (SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(RunScriptAsync) + "Base"))) {
            var methodNamesFromStack = MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            SimpleLogger.LogInformationWithCallStack(Properties.Resources.ExecutingScript, methodNamesFromStack);
            var webView2Property = typeof(TModel).GetPropertiesAndInterfaceProperties().FirstOrDefault(p => p.Name == nameof(IWebViewApplicationModelBase.WebView));
            var webView2 = webView2Property == null || !ModelPropertyToWindowFieldMapping.ContainsKey(webView2Property)
                ? null
                : (WebView2)ModelPropertyToWindowFieldMapping[webView2Property].GetValue(Window);
            if (webView2 == null) {
                SimpleLogger.LogInformationWithCallStack(Properties.Resources.UiDoesNotContainAWebView, methodNamesFromStack);
                return await Task.FromResult(new TResult { Success = new YesNoInconclusive { Inconclusive = false, YesNo = false }, ErrorMessage = Properties.Resources.UiDoesNotContainAWebView });
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
        await WebViewNavigatingHelper.WaitUntilNotNavigatingAnymoreAsync("", DateTime.MinValue);
    }
}