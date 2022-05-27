using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Vishizhukel.Interfaces.Application;
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
        IApplicationLogger applicationLogger, ILogicalUrlRepository logicalUrlRepository) {
        var errorsAndInfos = new ErrorsAndInfos();
        await GoToUrlAsync("Rhönlamas", model, guiAndAppHandler,
            applicationLogger, logicalUrlRepository, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) { return errorsAndInfos; }

        var scriptStatement = new ScriptStatement {
            Statement = "OustOccurrenceFinder.DoesDocumentHaveDivLikeWithIdOrNthOccurrenceOfClass(\"outrappage\", 1)"
        };
        var scriptCallResult = await guiAndAppHandler.RunScriptAsync<ScriptCallResponse>(scriptStatement, false, true);
        scriptCallResult = ScriptCallResponseUtilities.VerifyExpectedClasses(scriptCallResult, new List<string> { "outrappage", "container" }, "outrappage", 1);
        return ScriptCallResponseUtilities.ToTestRunErrorsAndInfos(scriptCallResult);
    }
}