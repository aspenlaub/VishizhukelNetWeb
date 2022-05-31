using System;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Enums;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using IWebViewApplicationModelBase = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces.IWebViewApplicationModelBase;
using IWebViewNavigatingHelper = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces.IWebViewNavigatingHelper;
using IWebViewNavigationHelper = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces.IWebViewNavigationHelper;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Helpers;

public class WebViewNavigationHelper<TModel> : IWebViewNavigationHelper where TModel : IWebViewApplicationModelBase {
    private readonly TModel Model;
    private readonly ISimpleLogger SimpleLogger;
    private readonly IGuiAndAppHandler<TModel> GuiAndAppHandler;
    private readonly IWebViewNavigatingHelper WebViewNavigatingHelper;
    private readonly IMethodNamesFromStackFramesExtractor MethodNamesFromStackFramesExtractor;

    public WebViewNavigationHelper(TModel model, ISimpleLogger simpleLogger, IGuiAndAppHandler<TModel> guiAndAppHandler,
            IWebViewNavigatingHelper webViewNavigatingHelper, IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor) {
        Model = model;
        SimpleLogger = simpleLogger;
        GuiAndAppHandler = guiAndAppHandler;
        WebViewNavigatingHelper = webViewNavigatingHelper;
        MethodNamesFromStackFramesExtractor = methodNamesFromStackFramesExtractor;
    }

    public async Task<bool> NavigateToUrlAsync(string url) {
        var methodNamesFromStack = MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
        SimpleLogger.LogInformationWithCallStack($"App navigating to '{url}'", methodNamesFromStack);

        if (!await WebViewNavigatingHelper.WaitUntilNotNavigatingAnymoreAsync(url, DateTime.MinValue)) {
            return false;
        }

        SimpleLogger.LogInformationWithCallStack(Properties.Resources.ResetModelUrlAndSync, methodNamesFromStack);
        Model.WebView.Url = Urls.AboutBlank;
        var minLastUpdateTime = DateTime.Now;
        await GuiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();

        if (!await WebViewNavigatingHelper.WaitUntilNotNavigatingAnymoreAsync(url, minLastUpdateTime)) {
            return false;
        }

        SimpleLogger.LogInformationWithCallStack(Properties.Resources.SetModelUrlAndAsync, methodNamesFromStack);
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