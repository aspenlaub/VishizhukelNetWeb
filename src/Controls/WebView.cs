using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Controls;

public class WebView : IWebView {
    public bool IsNavigating { get; set; }
    public bool HasValidDocument { get; set; }
    public IScriptStatement OnDocumentLoaded { get; set; }
    public bool IsWired { get; set; }

    public WebView() {
        IsNavigating = false;
        HasValidDocument = false;
        OnDocumentLoaded = new ScriptStatement();
        IsWired = false;
    }
}