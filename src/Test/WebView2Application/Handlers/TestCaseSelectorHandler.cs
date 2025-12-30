using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.TestCases;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Handlers;

public class TestCaseSelectorHandler(IApplicationModel model, IGuiAndWebViewAppHandler<ApplicationModel> guiAndAppHandler) : ITestCaseSelectorHandler {
    public async Task UpdateSelectableTestCasesAsync() {
        IList<ITestCase> testCases = AllTestCases.Instance;
        var selectables = new List<Selectable>();
        selectables.AddRange(testCases.Select(t => new Selectable { Guid = t.Guid, Name = t.Name }));
        model.SelectedTestCase.UpdateSelectables(selectables);
        await guiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }

    public async Task TestCasesSelectedIndexChangedAsync(int selectedIndex, bool selectablesChanged) {
        if (model.SelectedTestCase.SelectedIndex == selectedIndex) { return; }

        model.SelectedTestCase.SelectedIndex = selectedIndex;
        await guiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }
}