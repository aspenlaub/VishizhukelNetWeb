using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.TestCases;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Handlers;

public class TestCaseSelectorHandler : ITestCaseSelectorHandler {
    private readonly IApplicationModel _Model;
    private readonly IGuiAndWebViewAppHandler<ApplicationModel> _GuiAndAppHandler;

    public TestCaseSelectorHandler(IApplicationModel model, IGuiAndWebViewAppHandler<ApplicationModel> guiAndAppHandler) {
        _Model = model;
        _GuiAndAppHandler = guiAndAppHandler;
    }

    public async Task UpdateSelectableTestCasesAsync() {
        var testCases = AllTestCases.Instance;
        var selectables = new List<Selectable>();
        selectables.AddRange(testCases.Select(t => new Selectable { Guid = t.Guid, Name = t.Name }));
        _Model.SelectedTestCase.UpdateSelectables(selectables);
        await _GuiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }

    public async Task TestCasesSelectedIndexChangedAsync(int selectedIndex, bool selectablesChanged) {
        if (_Model.SelectedTestCase.SelectedIndex == selectedIndex) { return; }

        _Model.SelectedTestCase.SelectedIndex = selectedIndex;
        await _GuiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }
}