using System;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Vishizhukel.Interfaces.Application;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Enums;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Helpers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using IWebViewApplicationModelBase = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces.IWebViewApplicationModelBase;
using IWebViewNavigatingHelper = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces.IWebViewNavigatingHelper;
using IWebViewNavigationHelper = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces.IWebViewNavigationHelper;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Helpers;

public class WebViewNavigationHelper<TModel> : IWebViewNavigationHelper where TModel : IWebViewApplicationModelBase {
    private readonly TModel Model;
    private readonly IApplicationLogger ApplicationLogger;
    private readonly IGuiAndAppHandler<TModel> GuiAndAppHandler;
    private readonly IWebViewNavigatingHelper WebViewNavigatingHelper;

    public WebViewNavigationHelper(TModel model, IApplicationLogger applicationLogger, IGuiAndAppHandler<TModel> guiAndAppHandler, IWebViewNavigatingHelper webViewNavigatingHelper) {
        Model = model;
        ApplicationLogger = applicationLogger;
        GuiAndAppHandler = guiAndAppHandler;
        WebViewNavigatingHelper = webViewNavigatingHelper;
    }

    public async Task<bool> NavigateToUrlAsync(string url) {
        ApplicationLogger.LogMessage($"App navigating to '{url}'");

        if (!await WebViewNavigatingHelper.WaitUntilNotNavigatingAnymoreAsync(url, DateTime.MinValue)) {
            return false;
        }

        ApplicationLogger.LogMessage(Properties.Resources.ResetModelUrlAndSync);
        Model.WebView.Url = Urls.AboutBlank;
        var minLastUpdateTime = DateTime.Now;
        await GuiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();

        if (!await WebViewNavigatingHelper.WaitUntilNotNavigatingAnymoreAsync(url, minLastUpdateTime)) {
            return false;
        }

        ApplicationLogger.LogMessage(Properties.Resources.SetModelUrlAndAsync);
        Model.WebView.Url = url;
        minLastUpdateTime = DateTime.Now;
        await GuiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();

        if (!await WebViewNavigatingHelper.WaitUntilNotNavigatingAnymoreAsync(url, minLastUpdateTime)) {
            return false;
        }

        Model.Status.Text = "";
        Model.Status.Type = StatusType.None;
        return true;
    }
}