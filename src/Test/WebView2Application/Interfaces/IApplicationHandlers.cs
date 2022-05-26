using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;

public interface IApplicationHandlers {
    ISimpleTextHandler WebViewUrlTextHandler { get; set; }
    ISimpleTextHandler WebViewContentSourceTextHandler { get; set; }
}