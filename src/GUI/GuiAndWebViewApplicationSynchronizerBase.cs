using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Vishizhukel.Interfaces.Application;
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

    protected GuiAndWebViewApplicationSynchronizerBase(TModel model, TWindow window, IApplicationLogger applicationLogger) : base(model, window, applicationLogger) {
        WebViewNavigatingHelper = new WebViewNavigatingHelper(Model, ApplicationLogger);
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
        if (modelWebView == null) {
            throw new ArgumentNullException(nameof(modelWebView));
        }

        if (!modelWebView.IsWired || modelWebView.Url == modelWebView.LastUrl) {
            return;
        }

        modelWebView.LastUrl = modelWebView.Url;
        ApplicationLogger.LogMessage($"Calling webView2.CoreWebView2.Navigate with '{modelWebView.Url}'");
        var minLastUpdateTime = DateTime.Now;
        webView2.CoreWebView2?.Navigate(modelWebView.Url);

        await WebViewNavigatingHelper.WaitUntilNotNavigatingAnymoreAsync(modelWebView.LastUrl, minLastUpdateTime);
    }

    public async Task<TResult> RunScriptAsync<TResult>(IScriptStatement scriptStatement) where TResult : IScriptCallResponse, new() {
        ApplicationLogger.LogMessage(Properties.Resources.ExecutingScript);
        var webView2Property = typeof(TModel).GetPropertiesAndInterfaceProperties().FirstOrDefault(p => p.Name == nameof(IWebViewApplicationModelBase.WebView));
        var webView2 = webView2Property == null || !ModelPropertyToWindowFieldMapping.ContainsKey(webView2Property)
            ? null
            : (WebView2)ModelPropertyToWindowFieldMapping[webView2Property].GetValue(Window);
        if (webView2 == null) {
            ApplicationLogger.LogMessage(Properties.Resources.UiDoesNotContainAWebView);
            return await Task.FromResult(new TResult { Success = new YesNoInconclusive { Inconclusive = false, YesNo = false }, ErrorMessage = Properties.Resources.UiDoesNotContainAWebView });
        }

        var json = await webView2.CoreWebView2.ExecuteScriptAsync(scriptStatement.Statement);
        ApplicationLogger.LogMessage(Properties.Resources.ScriptExecutedDeserializingResult);
        if (string.IsNullOrEmpty(json)) {
            ApplicationLogger.LogMessage(Properties.Resources.ScriptCallJsonResultIsEmpty);
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

        ApplicationLogger.LogMessage(Properties.Resources.CouldNotDeserializeScriptCallJsonResult);
        return await Task.FromResult(new TResult { Success = new YesNoInconclusive { Inconclusive = false, YesNo = false }, ErrorMessage = Properties.Resources.CouldNotDeserializeScriptCallJsonResult });
    }

    public async Task WaitUntilNotNavigatingAnymoreAsync() {
        await WebViewNavigatingHelper.WaitUntilNotNavigatingAnymoreAsync("", DateTime.MinValue);
    }
}