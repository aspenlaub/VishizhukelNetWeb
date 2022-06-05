using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Handlers;

internal class WebViewContentSourceTextHandler : ISimpleTextHandler {
    private readonly ApplicationModel _Model;
    private readonly IGuiAndAppHandler<ApplicationModel> _GuiAndAppHandler;

    public WebViewContentSourceTextHandler(ApplicationModel model, IGuiAndAppHandler<ApplicationModel> guiAndAppHandler) {
        _Model = model;
        _GuiAndAppHandler = guiAndAppHandler;
    }

    public async Task TextChangedAsync(string text) {
        if (_Model.WebViewContentSource.Text == text) { return; }

        _Model.WebViewContentSource.Text = text;

        await _GuiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }
}