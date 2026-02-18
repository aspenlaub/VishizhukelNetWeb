using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Entities;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Helpers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.TestCases;

public class CannotFindAnchorWhichIsNotDivLike : TestCaseBase, ITestCase {
    public string Guid => "99500DAA-2DE1-47A6-8875-97902F39E80C";
    public string Name => Properties.Resources.CannotFindAnchorWhichIsNotDivLike;

    public async Task<IErrorsAndInfos> RunAsync(ApplicationModel model, IGuiAndWebViewAppHandler<ApplicationModel> guiAndAppHandler,
            ISimpleLogger simpleLogger, ILogicalUrlRepository logicalUrlRepository, IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor) {
        var errorsAndInfos = new ErrorsAndInfos();
        await GoToUrlAsync("Rhönlamas", model, guiAndAppHandler,
            simpleLogger, logicalUrlRepository, methodNamesFromStackFramesExtractor, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) { return errorsAndInfos; }

        var scriptStatement = new ScriptStatement {
            Statement = "OustOccurrenceFinder.DoesDocumentHaveDivLikeWithIdOrNthOccurrenceOfClass(\"navbar-brand\", 1)"
        };
        ScriptCallResponse scriptCallResult = await guiAndAppHandler.RunScriptAsync<ScriptCallResponse>(scriptStatement, true, false);
        scriptCallResult = ScriptCallResponseUtilities.Invert(scriptCallResult, "Could not find first div-like element with id or class 'navbar-brand'");
        return ScriptCallResponseUtilities.ToTestRunErrorsAndInfos(scriptCallResult);
    }
}