using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Enums;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.TestCases;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Commands;

public class RunTestCaseCommand : ICommand {
    private readonly ApplicationModel _Model;
    private readonly IGuiAndWebViewAppHandler<ApplicationModel> _GuiAndAppHandler;
    private readonly ISimpleLogger _SimpleLogger;
    private readonly ILogicalUrlRepository _LogicalUrlRepository;
    private readonly IMethodNamesFromStackFramesExtractor _MethodNamesFromStackFramesExtractor;

    public RunTestCaseCommand(ApplicationModel model, IGuiAndWebViewAppHandler<ApplicationModel> guiAndAppHandler,
            ISimpleLogger simpleLogger, ILogicalUrlRepository logicalUrlRepository, IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor) {
        _Model = model;
        _GuiAndAppHandler = guiAndAppHandler;
        _SimpleLogger = simpleLogger;
        _LogicalUrlRepository = logicalUrlRepository;
        _MethodNamesFromStackFramesExtractor = methodNamesFromStackFramesExtractor;
    }

    public async Task ExecuteAsync() {
        if (!_Model.RunTestCase.Enabled) { return; }

        var testCase = AllTestCases.Instance.FirstOrDefault(t => t.Guid == _Model.SelectedTestCase.SelectedItem.Guid);
        if (testCase == null) { return; }

        var errorsAndInfos = await testCase.RunAsync(_Model, _GuiAndAppHandler, _SimpleLogger, _LogicalUrlRepository, _MethodNamesFromStackFramesExtractor);

        if (errorsAndInfos.AnyErrors()) {
            _Model.Status.Type = StatusType.Error;
            _Model.Status.Text = string.Join("\r\n", errorsAndInfos.Errors);
        } else {
            _Model.Status.Type = StatusType.Success;
            _Model.Status.Text = string.Join("\r\n", errorsAndInfos.Infos);
        }
        await _GuiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }

    public async Task<bool> ShouldBeEnabledAsync() {
        var enabled = _Model.SelectedTestCase.SelectedIndex >= 0;
        return await Task.FromResult(enabled);
    }
}