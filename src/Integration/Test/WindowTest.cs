using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tash;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.GUI;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Integration.Test;

[TestClass]
public class WindowTest : IntegrationTestBase {
    private readonly List<string> _MethodNames = new() {
        "ProcessPressButtonTaskAsync", "UpdateWebViewIfNecessaryAsync", "OnWebViewSourceChangedAsync", "OnWebViewNavigationCompletedAsync",
        "WaitUntilNotNavigatingAnymoreAsync"
    };

    [TestMethod]
    public async Task WebViewFlowIsCorrect() {
        var logFolder = new Folder(Path.GetTempPath()).SubFolder("AspenlaubLogs").SubFolder("VishizhukelNetWeb");
        var minChangedAt = DateTime.Now;
        await LaunchWindowAndGoToLocalhost(logFolder, minChangedAt);
        var allActualLogEntries = GetLogEntriesInFilesChangedAfter(logFolder, minChangedAt);
        allActualLogEntries = allActualLogEntries.OrderBy(l => l.LogTime).ToList();
        var actualLogEntries = allActualLogEntries.Where(l => l.Stack.Any(s => _MethodNames.Any(m => s.StartsWith(m)))).ToList();

        var expectedLines = GetExpectedLines();
        VerifyThatEachExpectedLineIsInReducedLog(expectedLines, actualLogEntries, allActualLogEntries);

        var actualLines = actualLogEntries.Select(l => l.Message).ToList();

        Assert.IsTrue(expectedLines.Count <= actualLines.Count, $"Expected {expectedLines.Count} log lines, got {actualLines.Count}");
        for (var i = 0; i < expectedLines.Count; i++) {
            Assert.AreEqual(expectedLines[i], actualLines[i], $"Difference in log line {i + 1}: expected '{expectedLines[i]}', got '{actualLines[i]}'");
        }

        const int maxSeconds = 3;
        var elapsedSeconds = (actualLogEntries[^1].LogTime - actualLogEntries[0].LogTime).TotalSeconds;
        Assert.IsTrue(elapsedSeconds < maxSeconds, $"Expected navigation to take less than {maxSeconds} seconds, it was {elapsedSeconds}");
    }

    private async Task LaunchWindowAndGoToLocalhost(IFolder logFolder, DateTime minChangedAt) {
        using var sut = await CreateWindowUnderTestAsync(nameof(VishizhukelNetWebView2Window));
        var process = await sut.FindIdleProcessAsync();
        var logFileNames = Directory.GetFiles(logFolder.FullName, "*.log")
                                    .Where(f => File.GetLastWriteTime(f) > minChangedAt)
                                    .ToList();
        logFileNames.ForEach(f => File.Delete(f));
        var tasks = new List<ControllableProcessTask> {
            sut.CreateSetValueTask(process, nameof(ApplicationModel.WebViewUrl), "http://localhost/"), sut.CreatePressButtonTask(process, nameof(ApplicationModel.GoToUrl))
        };
        await sut.RemotelyProcessTaskListAsync(process, tasks);
    }

    private List<string> GetExpectedLines() {
        var folderName = GetType().Assembly.Location;
        folderName = folderName.Substring(0, folderName.LastIndexOf('\\'));
        var masterLogFileName = new Folder(folderName).FullName + @"\ExpectedWebViewFlowLog.txt";
        var expectedLines = File.ReadLines(masterLogFileName).ToList();
        return expectedLines;
    }

}