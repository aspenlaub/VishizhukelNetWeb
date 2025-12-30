using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.TestCases;

internal class CanGetOucidResponse : TestCaseBase, ITestCase {
    public string Guid => "243B1174-D048-4AC0-BF63-C5239C0AD173";
    public string Name => Properties.Resources.CanGetOucidResponse;

    public async Task<IErrorsAndInfos> RunAsync(ApplicationModel model, IGuiAndWebViewAppHandler<ApplicationModel> guiAndAppHandler, ISimpleLogger simpleLogger, ILogicalUrlRepository logicalUrlRepository,
                                                IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor) {
        var errorsAndInfos = new ErrorsAndInfos();
        string url = await logicalUrlRepository.GetUrlAsync("CreateIntegrationTestDatabase", errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) { return errorsAndInfos; }

        NavigationResult result = await guiAndAppHandler.NavigateToUrlAsync(url, new NavigateToUrlSettings { StopAfterOucidResponse = true });
        if (!result.Succeeded) { return result.ErrorsAndInfos; }

        if (result.OucidResponse == null) {
            errorsAndInfos.Errors.Add(Properties.Resources.NoOucidResponseReceived);
            return errorsAndInfos;
        }

        if (result.OucidResponse.WaitForLocalhostLogs || result.OucidResponse.WaitUntilNotNavigating || result.OucidResponse.MarkupValidation
                    || result.OucidResponse.WaitAgainForLocalhostLogs || result.OucidResponse.WaitAgainUntilNotNavigating) {
            errorsAndInfos.Errors.Add(Properties.Resources.OucidResponseSaysFurtherAction);
            return errorsAndInfos;
        }

        errorsAndInfos = new ErrorsAndInfos();
        errorsAndInfos.Infos.Add(Properties.Resources.TestCaseSucceeded);
        return errorsAndInfos;
    }
}