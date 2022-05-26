using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;

public class ScriptCallResponseBase : IScriptCallResponse {
    public YesNoInconclusive Success { get; set; }
    public string ErrorMessage { get; set; } = "";
}