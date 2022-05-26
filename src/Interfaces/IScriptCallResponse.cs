using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;

public interface IScriptCallResponse {
    YesNoInconclusive Success { get; set; }
    string ErrorMessage { get; set; }
}