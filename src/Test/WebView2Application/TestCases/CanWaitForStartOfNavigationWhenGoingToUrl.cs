using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Entities;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.TestCases;

public class CanWaitForStartOfNavigationWhenGoingToUrl : TestCaseBase, ITestCase {
    public string Guid => "723BC21F-CDCD-42D6-B0CB-38C92B191681";
    public string Name => Properties.Resources.CanWaitForStartOfNavigationWhenGoingToUrl;

    public async Task<IErrorsAndInfos> RunAsync(ApplicationModel model, IGuiAndWebViewAppHandler<ApplicationModel> guiAndAppHandler, ISimpleLogger simpleLogger, ILogicalUrlRepository logicalUrlRepository,
                                                IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor) {
        NavigationResult result = await guiAndAppHandler.NavigateToUrlAsync("http://localhost", new NavigateToUrlSettings { StopAfterNavigationStarted = true });
        if (!result.Succeeded) { return result.ErrorsAndInfos; }

        var errorsAndInfos = new ErrorsAndInfos();
        errorsAndInfos.Infos.Add(Properties.Resources.TestCaseSucceeded);
        return errorsAndInfos;
    }
}