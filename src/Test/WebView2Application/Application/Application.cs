using System.Diagnostics;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
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

public class Application : WebViewApplicationBase<IGuiAndWebViewApplicationSynchronizer<ApplicationModel>, ApplicationModel> {
    public IApplicationHandlers Handlers { get; private set; }
    public IApplicationCommands Commands { get; private set; }

    public ITashHandler<ApplicationModel> TashHandler { get; private set; }
    private readonly ITashAccessor TashAccessor;
    private readonly ILogicalUrlRepository LogicalUrlRepository;

    public Application(IButtonNameToCommandMapper buttonNameToCommandMapper, IToggleButtonNameToHandlerMapper toggleButtonNameToHandlerMapper,
            IGuiAndWebViewApplicationSynchronizer<ApplicationModel> guiAndApplicationSynchronizer, ApplicationModel model,
            ITashAccessor tashAccessor, ISimpleLogger simpleLogger, ILogicalUrlRepository logicalUrlRepository)
        : base(buttonNameToCommandMapper, toggleButtonNameToHandlerMapper, guiAndApplicationSynchronizer, model, simpleLogger) {
        TashAccessor = tashAccessor;
        LogicalUrlRepository = logicalUrlRepository;
    }

    public override async Task OnLoadedAsync() {
        await base.OnLoadedAsync();
        await Handlers.TestCaseSelectorHandler.UpdateSelectableTestCasesAsync();

        var errorsAndInfos = new ErrorsAndInfos();
        var oustUrl = await LogicalUrlRepository.GetUrlAsync("Oust", errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            Model.Status.Type = StatusType.Error;
            Model.Status.Text = errorsAndInfos.ErrorsToString();
            return;
        }

        var oustUtilitiesUrl = oustUrl + "oustutilitiesjs.php";
        var jQueryUrl = oustUrl + "jquery.php";
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
            GoToUrlCommand = new GoToUrlCommand(Model, WebViewNavigationHelper),
            RunJsCommand = new RunJsCommand(Model, this),
            RunTestCaseCommand = new RunTestCaseCommand(Model, this, SimpleLogger, LogicalUrlRepository)
        };
        var communicator = new TashCommunicatorBase<IApplicationModel>(TashAccessor, SimpleLogger);
        var selectors = new Dictionary<string, ISelector> {
            { nameof(IApplicationModel.SelectedTestCase), Model.SelectedTestCase }
        };
        var selectorHandler = new TashSelectorHandler(Handlers, SimpleLogger, communicator, selectors);
        var verifyAndSetHandler = new TashVerifyAndSetHandler(Handlers, SimpleLogger, selectorHandler, communicator, selectors);
        TashHandler = new TashHandler(TashAccessor, SimpleLogger, ButtonNameToCommandMapper, ToggleButtonNameToHandlerMapper, this, verifyAndSetHandler, selectorHandler, communicator);
    }

    public ITashTaskHandlingStatus<ApplicationModel> CreateTashTaskHandlingStatus() {
        return new TashTaskHandlingStatus<ApplicationModel>(Model, Process.GetCurrentProcess().Id);
    }
}