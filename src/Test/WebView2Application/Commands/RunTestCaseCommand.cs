using Aspenlaub.Net.GitHub.CSharp.Vishizhukel.Interfaces.Application;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Enums;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.TestCases;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Commands;

public class RunTestCaseCommand : ICommand {
    private readonly ApplicationModel Model;
    private readonly IGuiAndWebViewAppHandler<ApplicationModel> GuiAndAppHandler;
    private readonly IApplicationLogger ApplicationLogger;
    private readonly ILogicalUrlRepository LogicalUrlRepository;

    public RunTestCaseCommand(ApplicationModel model, IGuiAndWebViewAppHandler<ApplicationModel> guiAndAppHandler,
            IApplicationLogger applicationLogger, ILogicalUrlRepository logicalUrlRepository) {
        Model = model;
        GuiAndAppHandler = guiAndAppHandler;
        ApplicationLogger = applicationLogger;
        LogicalUrlRepository = logicalUrlRepository;
    }

    public async Task ExecuteAsync() {
        if (!Model.RunTestCase.Enabled) { return; }

        var testCase = AllTestCases.Instance.FirstOrDefault(t => t.Guid == Model.SelectedTestCase.SelectedItem.Guid);
        if (testCase == null) { return; }

        var errorsAndInfos = await testCase.RunAsync(Model, GuiAndAppHandler, ApplicationLogger, LogicalUrlRepository);

        if (errorsAndInfos.AnyErrors()) {
            Model.Status.Type = StatusType.Error;
            Model.Status.Text = string.Join("\r\n", errorsAndInfos.Errors);
        } else {
            Model.Status.Type = StatusType.Success;
            Model.Status.Text = string.Join("\r\n", errorsAndInfos.Infos);
        }
        await GuiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }

    public async Task<bool> ShouldBeEnabledAsync() {
        var enabled = Model.SelectedTestCase.SelectedIndex >= 0;
        return await Task.FromResult(enabled);
    }
}