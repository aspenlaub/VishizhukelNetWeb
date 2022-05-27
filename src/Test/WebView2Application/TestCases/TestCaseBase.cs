using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Vishizhukel.Interfaces.Application;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Helpers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.TestCases;

public class TestCaseBase {
    protected async Task GoToUrlAsync(string logicalUrlName, ApplicationModel model, IGuiAndAppHandler<ApplicationModel> guiAndAppHandler,
            IApplicationLogger applicationLogger, ILogicalUrlRepository logicalUrlRepository, IErrorsAndInfos errorsAndInfos) {
        var url = await logicalUrlRepository.GetUrlAsync(logicalUrlName, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) { return; }

        if (model.WebView.Url == url) {
            await GoToUrlAsync(Urls.AboutBlank, model, guiAndAppHandler, applicationLogger, logicalUrlRepository, errorsAndInfos);
        }

        applicationLogger.LogMessage($"Go to '{url}'");
        model.WebView.Url = url;
        await guiAndAppHandler.SyncGuiAndAppAsync();
    }
}