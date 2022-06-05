using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Handlers;

public class WebViewUrlTextHandler : ISimpleTextHandler {
    private readonly IApplicationModel _Model;
    private readonly IGuiAndAppHandler<ApplicationModel> _GuiAndAppHandler;

    public WebViewUrlTextHandler(ApplicationModel model, IGuiAndAppHandler<ApplicationModel> guiAndAppHandler) {
        _Model = model;
        _GuiAndAppHandler = guiAndAppHandler;
    }

    public async Task TextChangedAsync(string text) {
        if (_Model.WebViewUrl.Text == text) { return; }

        _Model.WebViewUrl.Text = text;

        await _GuiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }
}