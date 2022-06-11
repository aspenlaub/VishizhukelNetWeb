namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;

public interface IWebView {
    bool IsNavigating { get; set; }
    bool HasValidDocument { get; set; }
    bool IsWired { get; set; }

    IScriptStatement OnDocumentLoaded { get; set; }
}