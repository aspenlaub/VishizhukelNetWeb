using System;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Urls = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Helpers.Urls;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Controls;

public class WebView : IWebView {
    public string Url { get; set; }
    public bool IsNavigating { get; set; }
    public DateTime LastNavigationStartedAt { get; set; }
    public string LastUrl { get; set; }
    public bool HasValidDocument { get; set; }
    public IScriptStatement OnDocumentLoaded { get; set; }
    public bool IsWired { get; set; }

    public WebView() {
        Url = Urls.None;
        IsNavigating = false;
        LastNavigationStartedAt = DateTime.MinValue;
        LastUrl = "";
        HasValidDocument = false;
        OnDocumentLoaded = new ScriptStatement();
        IsWired = false;
    }
}