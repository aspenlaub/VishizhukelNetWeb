using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Microsoft.Web.WebView2.Wpf;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;

public interface IGuiToWebViewApplicationGate : IGuiToApplicationGate {
    Task WireWebViewAsync(WebView2 webView);
}