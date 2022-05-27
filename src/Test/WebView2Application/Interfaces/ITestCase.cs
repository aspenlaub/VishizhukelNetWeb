using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Vishizhukel.Interfaces.Application;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;

public interface ITestCase : IGuid {
    string Name { get; }

    Task<IErrorsAndInfos> RunAsync(ApplicationModel model, IGuiAndWebViewAppHandler<ApplicationModel> guiAndAppHandler,
        IApplicationLogger applicationLogger, ILogicalUrlRepository logicalUrlRepository);
}