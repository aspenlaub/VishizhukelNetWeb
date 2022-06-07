using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.TestCases {
    public class CanUseImprovedNavigationToUrl : TestCaseBase, ITestCase {
        public string Guid => "426DF59B-4DE6-4D24-8C17-8C034B1B9397";
        public string Name => Properties.Resources.CanUseImprovedNavigationToUrl;

        public async Task<IErrorsAndInfos> RunAsync(ApplicationModel model, IGuiAndWebViewAppHandler<ApplicationModel> guiAndAppHandler,
                    ISimpleLogger simpleLogger, ILogicalUrlRepository logicalUrlRepository,
                    IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor) {
            var errorsAndInfos = new ErrorsAndInfos();
            var url = await logicalUrlRepository.GetUrlAsync("Rhönlamas", errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) { return errorsAndInfos; }

            await guiAndAppHandler.NavigateToUrlAsync(url);

            errorsAndInfos.Infos.Add(Properties.Resources.TestCaseSucceeded);
            return errorsAndInfos;
        }
    }
}
