using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.TestCases;

public class AlwaysSucceeds : TestCaseBase, ITestCase {
    public string Guid => "E757A16D-E846-4BC3-89FD-74E6D123D6A6";
    public string Name => Properties.Resources.AlwaysSucceeds;

    public async Task<IErrorsAndInfos> RunAsync(ApplicationModel model, IGuiAndWebViewAppHandler<ApplicationModel> guiAndAppHandler,
        ISimpleLogger simpleLogger, ILogicalUrlRepository logicalUrlRepository) {
        var errorsAndInfos = new ErrorsAndInfos();
        errorsAndInfos.Infos.Add(Properties.Resources.AlwaysSucceeds);
        return await Task.FromResult(errorsAndInfos);
    }
}