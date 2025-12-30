using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;
// ReSharper disable UnusedMember.Global

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;

public class ScriptCallResponse : ScriptCallResponseBase {
    public DomElement DomElement { get; set; } = new();
    public Dictionary<string, string> Dictionary { get; set; } = [];
    public string InnerHtml { get; set; } = "";
}