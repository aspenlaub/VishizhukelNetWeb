using System;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;

public interface IWebView {
    string Url { get; set; }
    bool IsNavigating { get; set; }
    DateTime LastNavigationStartedAt { get; set; }
    string LastUrl { get; set; }
    bool HasValidDocument { get; set; }
    bool IsWired { get; set; }

    IScriptStatement OnDocumentLoaded { get; set; }
}