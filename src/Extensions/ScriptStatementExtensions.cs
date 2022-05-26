using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Extensions;

public static class ScriptStatementExtensions {
    public static IScriptStatement AppendStatement(this IScriptStatement scriptStatement, string statement) {
        scriptStatement.Statement = scriptStatement.Statement + (scriptStatement.Statement.Length > 0 ? " " : "") + statement;
        return scriptStatement;
    }

    public static bool Any(this IScriptStatement scriptStatement) {
        return scriptStatement.Statement.Length > 0;
    }

    public static void Reset(this IScriptStatement scriptStatement) {
        scriptStatement.Statement = "";
    }
}