using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Helpers;
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

        var helper = new WebViewNavigationHelper<ApplicationModel>(model, simpleLogger, guiAndAppHandler,
                                                                   new WebViewNavigatingHelper(model, simpleLogger, methodNamesFromStackFramesExtractor),
                                                                   methodNamesFromStackFramesExtractor);
        simpleLogger.LogInformationWithCallStack($"Go to '{url}'", methodNamesFromStack);
        await helper.NavigateToUrlAsync(url);
    }
}