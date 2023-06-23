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

        Assert.IsTrue(expectedLines.Count < actualLines.Count, $"Expected {expectedLines.Count} log lines, got {actualLines.Count}");
        int expectedLineIndex = 0, actualLineIndex = 0, optionalsFound = 0;
        string actualLine;
        while (expectedLineIndex < expectedLines.Count) {
            var expectedLine = expectedLines[expectedLineIndex];
            actualLine = actualLines[actualLineIndex];
            if (expectedLine.StartsWith('{')) {
                expectedLine = expectedLine.Substring(1, expectedLine.Length - 2);
                if (actualLine != expectedLine) {
                    expectedLineIndex ++;
                    continue;
                }

                optionalsFound ++;
                Assert.IsTrue(optionalsFound < 2, "More than two optional lines found, this is unexpected");
            }
            Assert.AreEqual(expectedLine, actualLine,
                $"Difference in log line {expectedLineIndex + 1}: expected '{expectedLine}', got '{actualLine}'");

            expectedLineIndex ++;
            actualLineIndex ++;
        }

        actualLine = actualLines[actualLineIndex];
        Assert.AreEqual("Communicating 'Completed' to remote controlling process", actualLine, $"Extra line found: {actualLine}");

        const double maxSeconds = 3;
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
        await sut.RemotelyProcessTaskListAsync(process, tasks, false, (_, _) => Task.CompletedTask);
    }

    private List<string> GetExpectedLines() {
        var folderName = GetType().Assembly.Location;
        folderName = folderName.Substring(0, folderName.LastIndexOf('\\'));
        var masterLogFileName = new Folder(folderName).FullName + @"\ExpectedWebViewFlowLog.txt";
        var expectedLines = File.ReadLines(masterLogFileName).ToList();
        return expectedLines;
    }

}