using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces {
    public interface IWebViewApplicationModelBase : IApplicationModelBase {
        IWebView WebView { get; }
        ITextBox WebViewUrl { get; }
        ITextBox WebViewContentSource { get; }
    }
}
