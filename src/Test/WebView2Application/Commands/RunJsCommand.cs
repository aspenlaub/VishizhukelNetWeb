using System.Text.Json;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Commands;

public class RunJsCommand(ApplicationModel model, IGuiAndWebViewAppHandler<ApplicationModel> guiAndAppHandler) : ICommand {
    public async Task ExecuteAsync() {
        if (!model.RunJs.Enabled) {
            return;
        }

        var scriptCallResponse = new ScriptCallResponseBase {
            Success = new YesNoInconclusive { Inconclusive = false, YesNo = true }
        };
        string statement = "(function() { "
                           + "alert('A script has been run: ' + document.head.children[document.head.children.length - 1].outerHTML); "
                           + "return " + JsonSerializer.Serialize(scriptCallResponse)
                           + "})();";
        const string inconclusiveErrorMessage = "Script result is inconclusive";
        const string noSuccessErrorMessage = "Script call failed";
        var scriptStatement = new ScriptStatement { Statement = statement, InconclusiveErrorMessage = inconclusiveErrorMessage, NoSuccessErrorMessage = noSuccessErrorMessage };
        await guiAndAppHandler.RunScriptAsync<ScriptCallResponseBase>(scriptStatement, false, true);
    }

    public async Task<bool> ShouldBeEnabledAsync() {
        bool enabled = model.WebViewUrl.Text.StartsWith("http", StringComparison.InvariantCulture);
        return await Task.FromResult(enabled);
    }
}