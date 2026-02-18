using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Entities;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Helpers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.TestCases;

public class CanFindLamasMainByClass : TestCaseBase, ITestCase {
    public string Guid => "FB4E1F37-0B24-4665-8B4B-0A5BAC66D87A";
    public string Name => Properties.Resources.CanFindLamasMainByClass;

    public async Task<IErrorsAndInfos> RunAsync(ApplicationModel model, IGuiAndWebViewAppHandler<ApplicationModel> guiAndAppHandler,
            ISimpleLogger simpleLogger, ILogicalUrlRepository logicalUrlRepository, IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor) {
        var errorsAndInfos = new ErrorsAndInfos();
        await GoToUrlAsync("Rhönlamas", model, guiAndAppHandler,
            simpleLogger, logicalUrlRepository, methodNamesFromStackFramesExtractor, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) { return errorsAndInfos; }

        var scriptStatement = new ScriptStatement {
            Statement = "OustOccurrenceFinder.DoesDocumentHaveDivLikeWithIdOrNthOccurrenceOfClass(\"outrappage\", 1)"
        };
        ScriptCallResponse scriptCallResult = await guiAndAppHandler.RunScriptAsync<ScriptCallResponse>(scriptStatement, false, true);
        scriptCallResult = ScriptCallResponseUtilities.VerifyExpectedClasses(scriptCallResult, ["outrappage", "container"], "outrappage", 1);
        return ScriptCallResponseUtilities.ToTestRunErrorsAndInfos(scriptCallResult);
    }
}