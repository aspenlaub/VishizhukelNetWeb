using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Commands;

public class GoToUrlCommand : ICommand {
    private readonly IApplicationModel Model;
    private readonly IWebViewNavigationHelper WebViewNavigationHelper;

    public GoToUrlCommand(IApplicationModel model, IWebViewNavigationHelper webViewNavigationHelper) {
        Model = model;
        WebViewNavigationHelper = webViewNavigationHelper;
    }

    public async Task ExecuteAsync() {
        if (!Model.GoToUrl.Enabled) {
            return;
        }

        await WebViewNavigationHelper.NavigateToUrlAsync(Model.WebViewUrl.Text);
    }

    public async Task<bool> ShouldBeEnabledAsync() {
        var enabled = Model.WebViewUrl.Text.StartsWith("http", StringComparison.InvariantCulture);
        return await Task.FromResult(enabled);
    }
}