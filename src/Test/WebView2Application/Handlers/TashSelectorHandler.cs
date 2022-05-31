using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tash;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Handlers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;
using IApplicationModel = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces.IApplicationModel;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Handlers;

public class TashSelectorHandler : TashSelectorHandlerBase<IApplicationModel> {
    // ReSharper disable once NotAccessedField.Local
    private readonly IApplicationHandlers ApplicationHandlers;

    public TashSelectorHandler(IApplicationHandlers applicationHandlers, ISimpleLogger simpleLogger, ITashCommunicator<IApplicationModel> tashCommunicator,
            Dictionary<string, ISelector> selectors, IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor)
        : base(simpleLogger, tashCommunicator, selectors, methodNamesFromStackFramesExtractor) {
        ApplicationHandlers = applicationHandlers;
    }

    protected override async Task SelectedIndexChangedAsync(ITashTaskHandlingStatus<IApplicationModel> status, string controlName, int selectedIndex, bool selectablesChanged) {
        if (selectedIndex < 0) { return; }

        var methodNamesFromStack = MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
        SimpleLogger.LogInformationWithCallStack($"Changing selected index for {controlName} to {selectedIndex}", methodNamesFromStack);
        switch (controlName) {
            case nameof(status.Model.SelectedTestCase):
                await ApplicationHandlers.TestCaseSelectorHandler.TestCasesSelectedIndexChangedAsync(selectedIndex, selectablesChanged);
                break;
            default:
                var errorMessage = $"Do not know how to select for {status.TaskBeingProcessed.ControlName}";
                SimpleLogger.LogInformationWithCallStack(
                    $"Communicating 'BadRequest' to remote controlling process ({errorMessage})", methodNamesFromStack);
                await TashCommunicator.ChangeCommunicateAndShowProcessTaskStatusAsync(status,
                    ControllableProcessTaskStatus.BadRequest, false, "", errorMessage);
                break;
        }
    }

    public override async Task ProcessSelectComboOrResetTaskAsync(ITashTaskHandlingStatus<IApplicationModel> status) {
        var methodNamesFromStack = MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
        var controlName = status.TaskBeingProcessed.ControlName;
        if (!Selectors.ContainsKey(controlName)) {
            var errorMessage = $"Unknown selector control {controlName}";
            SimpleLogger.LogInformationWithCallStack($"Communicating 'BadRequest' to remote controlling process ({errorMessage})", methodNamesFromStack);
            await TashCommunicator.ChangeCommunicateAndShowProcessTaskStatusAsync(status, ControllableProcessTaskStatus.BadRequest, false, "", errorMessage);
            return;
        }

        SimpleLogger.LogInformationWithCallStack($"{controlName} is a valid selector", methodNamesFromStack);
        var selector = Selectors[controlName];

        await SelectedIndexChangedAsync(status, controlName, -1, false);
        if (status.TaskBeingProcessed.Status == ControllableProcessTaskStatus.BadRequest) { return; }

        var itemToSelect = status.TaskBeingProcessed.Text;
        await SelectItemAsync(status, selector, itemToSelect, controlName);
    }
}