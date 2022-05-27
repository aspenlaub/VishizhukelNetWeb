using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Controls;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using IWebView = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces.IWebView;
using IWebViewApplicationModelBase = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces.IWebViewApplicationModelBase;
using WebView = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Controls.WebView;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;

public class WebViewApplicationModelBase : ApplicationModelBase, IWebViewApplicationModelBase {
    public IWebView WebView { get; } = new WebView();

    public ITextBox WebViewUrl { get; } = new TextBox();
    public ITextBox WebViewContentSource { get; } = new TextBox();
}