using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Entities;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Helpers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.TestCases;

public class CanFindBody : TestCaseBase, ITestCase {
    public string Guid => "BFC050B5-078D-44D5-886D-7798C318972F";
    public string Name => Properties.Resources.CanFindBody;

    public async Task<IErrorsAndInfos> RunAsync(ApplicationModel model, IGuiAndWebViewAppHandler<ApplicationModel> guiAndAppHandler,
            ISimpleLogger simpleLogger, ILogicalUrlRepository logicalUrlRepository, IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor) {
        var errorsAndInfos = new ErrorsAndInfos();
        await GoToUrlAsync("Rhönlamas", model, guiAndAppHandler,
            simpleLogger, logicalUrlRepository, methodNamesFromStackFramesExtractor, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) { return errorsAndInfos; }

        var scriptStatement = new ScriptStatement {
            Statement = "OustOccurrenceFinder.DoesDocumentHaveNthOccurrenceOfIdOrClass(\"body\", 1)"
        };
        ScriptCallResponse scriptCallResponse = await guiAndAppHandler.RunScriptAsync<ScriptCallResponse>(scriptStatement, false, true);
        scriptCallResponse = ScriptCallResponseUtilities.VerifyExpectedClasses(scriptCallResponse, [], "body", 1);
        return ScriptCallResponseUtilities.ToTestRunErrorsAndInfos(scriptCallResponse);
    }
}