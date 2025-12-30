using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Commands;

public class GoToUrlCommand(IApplicationModel model, IGuiAndWebViewAppHandler<ApplicationModel> guiAndWebViewAppHandler)
        : ICommand {
    public async Task ExecuteAsync() {
        if (!model.GoToUrl.Enabled) {
            return;
        }

        await guiAndWebViewAppHandler.NavigateToUrlAsync(model.WebViewUrl.Text);
    }

    public async Task<bool> ShouldBeEnabledAsync() {
        bool enabled = model.WebViewUrl.Text.StartsWith("http", StringComparison.InvariantCulture);
        return await Task.FromResult(enabled);
    }
}