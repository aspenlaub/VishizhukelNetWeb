using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Handlers;

internal class WebViewContentSourceTextHandler : ISimpleTextHandler {
    private readonly ApplicationModel Model;
    private readonly IGuiAndAppHandler<ApplicationModel> GuiAndAppHandler;

    public WebViewContentSourceTextHandler(ApplicationModel model, IGuiAndAppHandler<ApplicationModel> guiAndAppHandler) {
        Model = model;
        GuiAndAppHandler = guiAndAppHandler;
    }

    public async Task TextChangedAsync(string text) {
        if (Model.WebViewContentSource.Text == text) { return; }

        Model.WebViewContentSource.Text = text;

        await GuiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }
}