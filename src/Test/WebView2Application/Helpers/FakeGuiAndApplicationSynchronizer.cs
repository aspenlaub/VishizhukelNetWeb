using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Helpers;

public class FakeGuiAndApplicationSynchronizer : IGuiAndWebViewApplicationSynchronizer<ApplicationModel> {
    public ApplicationModel Model { get; }

    public FakeGuiAndApplicationSynchronizer(ApplicationModel model) {
        Model = model;
    }

    public async Task OnModelDataChangedAsync() {
        await Task.CompletedTask;
    }

    public void IndicateBusy(bool _) {
    }

    public async Task<TResult> RunScriptAsync<TResult>(IScriptStatement _) where TResult : IScriptCallResponse, new() {
        return await Task.FromResult(new TResult { Success = new YesNoInconclusive { Inconclusive = true, YesNo = false } });
    }

    public async Task WaitUntilNotNavigatingAnymoreAsync() {
        await Task.CompletedTask;
    }

    public async Task NavigateToUrlAndWaitForStartOfNavigationAsync(string url, IErrorsAndInfos errorsAndInfos) {
        await Task.CompletedTask;
    }
}