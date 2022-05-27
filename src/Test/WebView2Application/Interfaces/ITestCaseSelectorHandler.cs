namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;

public interface ITestCaseSelectorHandler {
    Task UpdateSelectableTestCasesAsync();
    Task TestCasesSelectedIndexChangedAsync(int testCasesSelectedIndex, bool selectablesChanged);
}