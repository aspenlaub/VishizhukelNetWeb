using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Handlers;

public class ApplicationHandlers : IApplicationHandlers {
    public ISimpleTextHandler WebViewUrlTextHandler { get; set; }
    public ISimpleTextHandler WebViewContentSourceTextHandler { get; set; }
    public ITestCaseSelectorHandler TestCaseSelectorHandler { get; set; }
}