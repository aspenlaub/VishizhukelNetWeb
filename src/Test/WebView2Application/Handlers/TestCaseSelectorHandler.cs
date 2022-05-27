using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.TestCases;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Handlers;

public class TestCaseSelectorHandler : ITestCaseSelectorHandler {
    private readonly IApplicationModel Model;
    private readonly IGuiAndWebViewAppHandler<ApplicationModel> GuiAndAppHandler;

    public TestCaseSelectorHandler(IApplicationModel model, IGuiAndWebViewAppHandler<ApplicationModel> guiAndAppHandler) {
        Model = model;
        GuiAndAppHandler = guiAndAppHandler;
    }

    public async Task UpdateSelectableTestCasesAsync() {
        var testCases = AllTestCases.Instance;
        var selectables = new List<Selectable>();
        selectables.AddRange(testCases.Select(t => new Selectable { Guid = t.Guid, Name = t.Name }));
        Model.SelectedTestCase.UpdateSelectables(selectables);
        await GuiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }

    public async Task TestCasesSelectedIndexChangedAsync(int selectedIndex, bool selectablesChanged) {
        if (Model.SelectedTestCase.SelectedIndex == selectedIndex) { return; }

        Model.SelectedTestCase.SelectedIndex = selectedIndex;
        await GuiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }
}