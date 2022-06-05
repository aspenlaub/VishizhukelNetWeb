using System;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Enums;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using IWebViewApplicationModelBase = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces.IWebViewApplicationModelBase;
using IWebViewNavigatingHelper = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces.IWebViewNavigatingHelper;
using IWebViewNavigationHelper = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces.IWebViewNavigationHelper;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Helpers;

public class WebViewNavigationHelper<TModel> : IWebViewNavigationHelper where TModel : IWebViewApplicationModelBase {
    private readonly TModel _Model;
    private readonly ISimpleLogger _SimpleLogger;
    private readonly IGuiAndWebViewAppHandler<TModel> _GuiAndAppHandler;
    private readonly IWebViewNavigatingHelper _WebViewNavigatingHelper;
    private readonly IMethodNamesFromStackFramesExtractor _MethodNamesFromStackFramesExtractor;

    public WebViewNavigationHelper(TModel model, ISimpleLogger simpleLogger, IGuiAndWebViewAppHandler<TModel> guiAndAppHandler,
            IWebViewNavigatingHelper webViewNavigatingHelper, IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor) {
        _Model = model;
        _SimpleLogger = simpleLogger;
        _GuiAndAppHandler = guiAndAppHandler;
        _WebViewNavigatingHelper = webViewNavigatingHelper;
        _MethodNamesFromStackFramesExtractor = methodNamesFromStackFramesExtractor;
    }

    public async Task<bool> NavigateToUrlAsync(string url) {
        var methodNamesFromStack = _MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
        _SimpleLogger.LogInformationWithCallStack($"App navigating to '{url}'", methodNamesFromStack);

        if (!await _WebViewNavigatingHelper.WaitUntilNotNavigatingAnymoreAsync(url, DateTime.MinValue)) {
            return false;
        }

        _SimpleLogger.LogInformationWithCallStack(Properties.Resources.ResetModelUrlAndSync, methodNamesFromStack);
        _Model.WebView.Url = Urls.AboutBlank;
        var minLastUpdateTime = DateTime.Now;
        await _GuiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();

        if (!await _WebViewNavigatingHelper.WaitUntilNotNavigatingAnymoreAsync(url, minLastUpdateTime)) {
            return false;
        }

        _SimpleLogger.LogInformationWithCallStack(Properties.Resources.SetModelUrlAndAsync, methodNamesFromStack);
        _Model.WebView.Url = url;
        minLastUpdateTime = DateTime.Now;
        await _GuiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();

        if (!await _WebViewNavigatingHelper.WaitUntilNotNavigatingAnymoreAsync(url, minLastUpdateTime)) {
            return false;
        }

        _Model.Status.Text = "";
        _Model.Status.Type = StatusType.None;
        return true;
    }

    public async Task<bool> NavigateToUrlImprovedAsync(string url) {
        var methodNamesFromStack = _MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
        _SimpleLogger.LogInformationWithCallStack($"App navigating to '{url}'", methodNamesFromStack);

        if (!await _WebViewNavigatingHelper.WaitUntilNotNavigatingAnymoreAsync(url, DateTime.MinValue)) {
            return false;
        }

        _SimpleLogger.LogInformationWithCallStack(Properties.Resources.NavigatingToUrl, methodNamesFromStack);
        var errorsAndInfos = new ErrorsAndInfos();
        await _GuiAndAppHandler.NavigateToUrlAndWaitForStartOfNavigationAsync(url, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            return false;
        }

        await _GuiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();

        if (!await _WebViewNavigatingHelper.WaitUntilNotNavigatingAnymoreAsync(url, DateTime.MinValue)) {
            return false;
        }

        _Model.Status.Text = "";
        _Model.Status.Type = StatusType.None;
        return true;
    }

}