using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Helpers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.TestCases;

public class CanFindAnchor : TestCaseBase, ITestCase {
    public string Guid => "476BCC22-E629-4E09-A69D-FBB6EC0679DB";
    public string Name => Properties.Resources.CanFindAnchor;

    public async Task<IErrorsAndInfos> RunAsync(ApplicationModel model, IGuiAndWebViewAppHandler<ApplicationModel> guiAndAppHandler,
            ISimpleLogger simpleLogger, ILogicalUrlRepository logicalUrlRepository, IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor) {
        var errorsAndInfos = new ErrorsAndInfos();
        await GoToUrlAsync("Rhönlamas", model, guiAndAppHandler,
            simpleLogger, logicalUrlRepository, methodNamesFromStackFramesExtractor, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) { return errorsAndInfos; }

        var scriptStatement = new ScriptStatement {
            Statement = "OustOccurrenceFinder.DoesDocumentHaveNthOccurrenceOfIdOrClass(\".navbar-brand\", 1)"
        };
        ScriptCallResponse scriptCallResponse = await guiAndAppHandler.RunScriptAsync<ScriptCallResponse>(scriptStatement, false, true);
        scriptCallResponse = ScriptCallResponseUtilities.VerifyExpectedClasses(scriptCallResponse, ["navbar-brand"], "navbar-brand", 1);
        return ScriptCallResponseUtilities.ToTestRunErrorsAndInfos(scriptCallResponse);
    }
}