using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.TestCases;

public class AlwaysFails : TestCaseBase, ITestCase {
    public string Guid => "D8236108-765F-42AB-B87A-2DED1BE7138E";
    public string Name => Properties.Resources.AlwaysFails;

    public async Task<IErrorsAndInfos> RunAsync(ApplicationModel model, IGuiAndWebViewAppHandler<ApplicationModel> guiAndAppHandler,
        ISimpleLogger simpleLogger, ILogicalUrlRepository logicalUrlRepository) {
        var errorsAndInfos = new ErrorsAndInfos();
        errorsAndInfos.Errors.Add(Properties.Resources.AlwaysFails);
        return await Task.FromResult(errorsAndInfos);
    }
}