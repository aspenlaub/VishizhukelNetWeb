using System.Diagnostics;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Entities;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Extensions;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Enums;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Handlers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Application;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Extensions;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Commands;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Handlers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Application;

public class Application(IButtonNameToCommandMapper buttonNameToCommandMapper, IToggleButtonNameToHandlerMapper toggleButtonNameToHandlerMapper,
    IGuiAndWebViewApplicationSynchronizer<ApplicationModel> guiAndApplicationSynchronizer,
    ApplicationModel model, ITashAccessor tashAccessor, ISimpleLogger simpleLogger, ILogicalUrlRepository logicalUrlRepository,
    IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor, IOucidLogAccessor oucidLogAccessor)
        : WebViewApplicationBase<IGuiAndWebViewApplicationSynchronizer<ApplicationModel>,ApplicationModel>(buttonNameToCommandMapper,
            toggleButtonNameToHandlerMapper, guiAndApplicationSynchronizer, model, simpleLogger,
            methodNamesFromStackFramesExtractor, oucidLogAccessor) {
    public IApplicationHandlers Handlers { get; private set; }
    public IApplicationCommands Commands { get; private set; }

    public ITashHandler<ApplicationModel> TashHandler { get; private set; }

    public override async Task OnLoadedAsync() {
        await base.OnLoadedAsync();
        await Handlers.TestCaseSelectorHandler.UpdateSelectableTestCasesAsync();

        var errorsAndInfos = new ErrorsAndInfos();
        string oustUtilitiesUrl = await logicalUrlRepository.GetUrlAsync("OustUtilitiesJs", errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            Model.Status.Type = StatusType.Error;
            Model.Status.Text = errorsAndInfos.ErrorsToString();
            return;
        }

        string jQueryUrl = await logicalUrlRepository.GetUrlAsync("JQuery", errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            Model.Status.Type = StatusType.Error;
            Model.Status.Text = errorsAndInfos.ErrorsToString();
            return;
        }

        Model.WebView.OnDocumentLoaded.AppendStatement(
            "var script = document.createElement(\"script\"); "
            + $"script.src = \"{oustUtilitiesUrl}\"; "
            + "document.head.appendChild(script);"
            + "if (typeof($jq) == 'undefined') {"
            + "var script = document.createElement(\"script\"); "
            + $"script.src = \"{jQueryUrl}\"; "
            + "document.head.appendChild(script);"
            + "}"
        );
    }

    protected override async Task EnableOrDisableButtonsAsync() {
        Model.GoToUrl.Enabled = await Commands.GoToUrlCommand.ShouldBeEnabledAsync();
        Model.RunJs.Enabled = await Commands.RunJsCommand.ShouldBeEnabledAsync();
        Model.RunTestCase.Enabled = await Commands.RunTestCaseCommand.ShouldBeEnabledAsync();
    }

    protected override void CreateCommandsAndHandlers() {
        Handlers = new ApplicationHandlers {
            WebViewUrlTextHandler = new WebViewUrlTextHandler(Model, this),
            WebViewContentSourceTextHandler = new WebViewContentSourceTextHandler(Model, this),
            TestCaseSelectorHandler = new TestCaseSelectorHandler(Model, this)
        };
        Commands = new ApplicationCommands {
            GoToUrlCommand = new GoToUrlCommand(Model, this),
            RunJsCommand = new RunJsCommand(Model, this),
            RunTestCaseCommand = new RunTestCaseCommand(Model, this, SimpleLogger, logicalUrlRepository, MethodNamesFromStackFramesExtractor)
        };
        var communicator = new TashCommunicatorBase<IApplicationModel>(tashAccessor, SimpleLogger, MethodNamesFromStackFramesExtractor);
        var selectors = new Dictionary<string, ISelector> {
            { nameof(IApplicationModel.SelectedTestCase), Model.SelectedTestCase }
        };
        var selectorHandler = new TashSelectorHandler(Handlers, SimpleLogger, communicator, selectors, MethodNamesFromStackFramesExtractor);
        var verifyAndSetHandler = new TashVerifyAndSetHandler(Handlers, SimpleLogger, selectorHandler, communicator, selectors, MethodNamesFromStackFramesExtractor);
        TashHandler = new TashHandler(tashAccessor, SimpleLogger, ButtonNameToCommandMapper, ToggleButtonNameToHandlerMapper, this, verifyAndSetHandler, selectorHandler, communicator, MethodNamesFromStackFramesExtractor);
    }

    public ITashTaskHandlingStatus<ApplicationModel> CreateTashTaskHandlingStatus() {
        return new TashTaskHandlingStatus<ApplicationModel>(Model, Process.GetCurrentProcess().Id);
    }
}