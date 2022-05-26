namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;

public interface IScriptStatement {
    string Statement { get; set; }
    string NoSuccessErrorMessage { get; set; }
    string InconclusiveErrorMessage { get; set; }
    string NoFailureErrorMessage { get; set; }
}