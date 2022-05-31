using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Enums;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.TestCases;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Commands;

public class RunTestCaseCommand : ICommand {
    private readonly ApplicationModel Model;
    private readonly IGuiAndWebViewAppHandler<ApplicationModel> GuiAndAppHandler;
    private readonly ISimpleLogger SimpleLogger;
    private readonly ILogicalUrlRepository LogicalUrlRepository;
    private readonly IMethodNamesFromStackFramesExtractor MethodNamesFromStackFramesExtractor;

    public RunTestCaseCommand(ApplicationModel model, IGuiAndWebViewAppHandler<ApplicationModel> guiAndAppHandler,
            ISimpleLogger simpleLogger, ILogicalUrlRepository logicalUrlRepository, IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor) {
        Model = model;
        GuiAndAppHandler = guiAndAppHandler;
        SimpleLogger = simpleLogger;
        LogicalUrlRepository = logicalUrlRepository;
        MethodNamesFromStackFramesExtractor = methodNamesFromStackFramesExtractor;
    }

    public async Task ExecuteAsync() {
        if (!Model.RunTestCase.Enabled) { return; }

        var testCase = AllTestCases.Instance.FirstOrDefault(t => t.Guid == Model.SelectedTestCase.SelectedItem.Guid);
        if (testCase == null) { return; }

        var errorsAndInfos = await testCase.RunAsync(Model, GuiAndAppHandler, SimpleLogger, LogicalUrlRepository, MethodNamesFromStackFramesExtractor);

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