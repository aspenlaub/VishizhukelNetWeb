using System.Windows;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tash;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Enums;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Handlers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using IApplicationModel = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces.IApplicationModel;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Handlers;

public class TashHandler : TashHandlerBase<ApplicationModel> {
    public TashHandler(ITashAccessor tashAccessor, ISimpleLogger simpleLogger, ILogConfiguration logConfiguration,
        IButtonNameToCommandMapper buttonNameToCommandMapper, IToggleButtonNameToHandlerMapper toggleButtonNameToHandlerMapper, IGuiAndWebViewAppHandler<ApplicationModel> guiAndAppHandler,
        ITashVerifyAndSetHandler<IApplicationModel> tashVerifyAndSetHandler, ITashSelectorHandler<IApplicationModel> tashSelectorHandler, ITashCommunicator<IApplicationModel> tashCommunicator)
        : base(tashAccessor, simpleLogger, logConfiguration, buttonNameToCommandMapper, toggleButtonNameToHandlerMapper, guiAndAppHandler, tashVerifyAndSetHandler, tashSelectorHandler, tashCommunicator) {
    }

    protected override async Task ProcessSingleTaskAsync(ITashTaskHandlingStatus<ApplicationModel> status) {
        SimpleLogger.LogInformation($"Processing a task of type {status.TaskBeingProcessed.Type} in {nameof(TashHandler)}");

        switch (status.TaskBeingProcessed.Type) {
            case ControllableProcessTaskType.Reset:
                await TashCommunicator.CommunicateAndShowCompletedOrFailedAsync(status, false, "");
                break;
            default:
                await base.ProcessSingleTaskAsync(status);
                break;
        }
    }
}