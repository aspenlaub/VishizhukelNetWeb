using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.TestCases;

public class TestCaseBase {
    protected async Task GoToUrlAsync(string logicalUrlName, ApplicationModel model, IGuiAndWebViewAppHandler<ApplicationModel> guiAndAppHandler,
            ISimpleLogger simpleLogger, ILogicalUrlRepository logicalUrlRepository, IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor,
            IErrorsAndInfos errorsAndInfos) {
        var methodNamesFromStack = methodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
        var url = await logicalUrlRepository.GetUrlAsync(logicalUrlName, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) { return; }

        simpleLogger.LogInformationWithCallStack($"Go to '{url}'", methodNamesFromStack);
        await guiAndAppHandler.NavigateToUrlAsync(url);
    }
}