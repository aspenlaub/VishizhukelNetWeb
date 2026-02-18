using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Enums;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.TestCases;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Commands;

public class RunTestCaseCommand(ApplicationModel model, IGuiAndWebViewAppHandler<ApplicationModel> guiAndAppHandler,
        ISimpleLogger simpleLogger, ILogicalUrlRepository logicalUrlRepository,
        IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor)
            : ICommand {
    public async Task ExecuteAsync() {
        if (!model.RunTestCase.Enabled) { return; }

        ITestCase testCase = AllTestCases.Instance.FirstOrDefault(t => t.Guid == model.SelectedTestCase.SelectedItem.Guid);
        if (testCase == null) { return; }

        IErrorsAndInfos errorsAndInfos = await testCase.RunAsync(model, guiAndAppHandler, simpleLogger, logicalUrlRepository, methodNamesFromStackFramesExtractor);

        if (errorsAndInfos.AnyErrors()) {
            model.Status.Type = StatusType.Error;
            model.Status.Text = string.Join("\r\n", errorsAndInfos.Errors);
        } else {
            model.Status.Type = StatusType.Success;
            model.Status.Text = string.Join("\r\n", errorsAndInfos.Infos);
        }
        await guiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }

    public async Task<bool> ShouldBeEnabledAsync() {
        bool enabled = model.SelectedTestCase.SelectedIndex >= 0;
        return await Task.FromResult(enabled);
    }
}