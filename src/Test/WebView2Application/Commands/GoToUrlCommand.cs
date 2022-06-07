using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Commands;

public class GoToUrlCommand : ICommand {
    private readonly IApplicationModel _Model;
    private readonly IGuiAndWebViewAppHandler<ApplicationModel> _GuiAndWebViewAppHandler;

    public GoToUrlCommand(IApplicationModel model, IGuiAndWebViewAppHandler<ApplicationModel> guiAndWebViewAppHandler) {
        _Model = model;
        _GuiAndWebViewAppHandler = guiAndWebViewAppHandler;
    }

    public async Task ExecuteAsync() {
        if (!_Model.GoToUrl.Enabled) {
            return;
        }

        await _GuiAndWebViewAppHandler.NavigateToUrlAsync(_Model.WebViewUrl.Text);
    }

    public async Task<bool> ShouldBeEnabledAsync() {
        var enabled = _Model.WebViewUrl.Text.StartsWith("http", StringComparison.InvariantCulture);
        return await Task.FromResult(enabled);
    }
}