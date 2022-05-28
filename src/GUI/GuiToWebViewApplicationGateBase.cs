using System.Text.RegularExpressions;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.GUI;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Extensions;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using IGuiToWebViewApplicationGate = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces.IGuiToWebViewApplicationGate;
using IWebViewApplicationModelBase = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces.IWebViewApplicationModelBase;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.GUI;

public abstract class GuiToWebViewApplicationGateBase<TApplication, TModel>
        : GuiToApplicationGateBase<TApplication, TModel>, IGuiToWebViewApplicationGate
            where TApplication : class, Interfaces.IGuiAndWebViewAppHandler<TModel>
            where TModel : IWebViewApplicationModelBase {
    protected GuiToWebViewApplicationGateBase(IBusy busy, TApplication application) : base(busy, application) {
    }

    public void WireWebView(WebView2 webView) {
        webView.SourceChanged += OnWebViewOnSourceChangedAsync;
        webView.NavigationStarting += OnNavigationStartingAsync;
        webView.NavigationCompleted += OnNavigationCompletedAsync;
        ApplicationModel.WebView.IsWired = true;
    }

    private async void OnNavigationStartingAsync(object sender, CoreWebView2NavigationStartingEventArgs e) {
        if (Application == null) { return; }

        var webView = sender as WebView2;
        if (webView == null) { return; }

        await Application.OnWebViewSourceChangedAsync(e.Uri);
    }

    private async void OnWebViewOnSourceChangedAsync(object sender, CoreWebView2SourceChangedEventArgs e) {
        if (Application == null) { return; }

        var webView = sender as WebView2;
        if (webView == null) { return; }

        if (ApplicationModel.WebView.IsNavigating && ApplicationModel.WebView.Url == webView.CoreWebView2.Source) {
            return;
        }

        await Application.OnWebViewSourceChangedAsync(webView.CoreWebView2.Source);
    }

    private async void OnNavigationCompletedAsync(object sender, CoreWebView2NavigationCompletedEventArgs e) {
        if (Application == null) { return; }

        var webView = sender as WebView2;
        if (webView == null) { return; }

        if (ApplicationModel.WebView.OnDocumentLoaded.Any()) {
            await webView.CoreWebView2.ExecuteScriptAsync(ApplicationModel.WebView.OnDocumentLoaded.Statement);
        }

        var source = await webView.CoreWebView2.ExecuteScriptAsync("document.documentElement.innerHTML");
        source = Regex.Unescape(source);
        source = source.Substring(1, source.Length - 2);
        await Application.OnWebViewNavigationCompletedAsync(source, e.IsSuccess);
    }
}