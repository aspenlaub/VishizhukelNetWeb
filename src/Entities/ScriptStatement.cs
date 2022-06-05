using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;

public class ScriptStatement : IScriptStatement {
    private string _PrivateStatement = "";

    public string Statement {
        get => _PrivateStatement;
        set {
            _PrivateStatement = value;
            NoSuccessErrorMessage = EnhanceErrorMessageIfNecessary(NoSuccessErrorMessage, Properties.Resources.ScriptCallFailed);
            NoFailureErrorMessage = EnhanceErrorMessageIfNecessary(NoFailureErrorMessage, Properties.Resources.ScriptCallFailureExpected);
        }
    }

    public string NoSuccessErrorMessage { get; set; } = Properties.Resources.ScriptCallFailed;
    public string InconclusiveErrorMessage { get; set; } = "";
    public string NoFailureErrorMessage { get; set; } = Properties.Resources.ScriptCallFailureExpected;

    private string EnhanceErrorMessageIfNecessary(string errorMessage, string defaultErrorMessage) {
        if (errorMessage != defaultErrorMessage) { return errorMessage; }

        var shortStatement = _PrivateStatement;
        if (shortStatement.Length > 20) {
            shortStatement = shortStatement.Substring(0, 20) + "..";
        }

        return defaultErrorMessage + ": " + shortStatement;
    }
}