using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Helpers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Microsoft.Extensions.Logging;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.TestCases;

public class TestCaseBase {
    protected async Task GoToUrlAsync(string logicalUrlName, ApplicationModel model, IGuiAndAppHandler<ApplicationModel> guiAndAppHandler,
            ISimpleLogger simpleLogger, ILogicalUrlRepository logicalUrlRepository, IErrorsAndInfos errorsAndInfos) {
        var url = await logicalUrlRepository.GetUrlAsync(logicalUrlName, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) { return; }

        if (model.WebView.Url == url) {
            await GoToUrlAsync(Urls.AboutBlank, model, guiAndAppHandler, simpleLogger, logicalUrlRepository, errorsAndInfos);
        }

        simpleLogger.LogInformation($"Go to '{url}'");
        model.WebView.Url = url;
        await guiAndAppHandler.SyncGuiAndAppAsync();
    }
}