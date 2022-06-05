using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Commands;

public class GoToUrlCommand : ICommand {
    private readonly IApplicationModel _Model;
    private readonly IWebViewNavigationHelper _WebViewNavigationHelper;

    public GoToUrlCommand(IApplicationModel model, IWebViewNavigationHelper webViewNavigationHelper) {
        _Model = model;
        _WebViewNavigationHelper = webViewNavigationHelper;
    }

    public async Task ExecuteAsync() {
        if (!_Model.GoToUrl.Enabled) {
            return;
        }

        await _WebViewNavigationHelper.NavigateToUrlAsync(_Model.WebViewUrl.Text);
    }

    public async Task<bool> ShouldBeEnabledAsync() {
        var enabled = _Model.WebViewUrl.Text.StartsWith("http", StringComparison.InvariantCulture);
        return await Task.FromResult(enabled);
    }
}