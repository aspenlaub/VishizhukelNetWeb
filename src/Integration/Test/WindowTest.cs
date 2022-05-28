using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Tash;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.GUI;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Integration.Test;

[TestClass]
public class WindowTest : IntegrationTestBase {
    [TestMethod]
    public async Task WebViewFlowIsCorrect() {
        var logFolder = new Folder(Path.GetTempPath()).SubFolder("AspenlaubLogs").SubFolder("VishizhukelNetWeb");
        var minChangedAt = DateTime.Now;
        using var sut = await CreateWindowUnderTestAsync(nameof(VishizhukelNetWebView2Window));
        var process = await sut.FindIdleProcessAsync();
        var logFileNames = Directory.GetFiles(logFolder.FullName, "*.log")
            .Where(f => File.GetLastWriteTime(f) > minChangedAt)
            .ToList();
        logFileNames.ForEach(f => File.Delete(f));
        minChangedAt = DateTime.Now;
        var tasks = new List<ControllableProcessTask> {
            sut.CreateSetValueTask(process, nameof(ApplicationModel.WebViewUrl), "http://localhost/"),
            sut.CreatePressButtonTask(process, nameof(ApplicationModel.GoToUrl))
        };
        await sut.RemotelyProcessTaskListAsync(process, tasks);
        logFileNames = Directory.GetFiles(logFolder.FullName, "*.log")
            .Where(f => File.GetLastWriteTime(f) > minChangedAt && File.ReadAllText(f).Contains("view"))
            .ToList();
        Assert.AreEqual(1, logFileNames.Count);
        var logFileName = logFileNames[0];
        var actualLines = File.ReadLines(logFileName).ToList();
        const string tag = "App navigating to";
        var index = actualLines.Select((l, x) => new Tuple<string, int>(l, x)).Where(t => t.Item1.Contains(tag)).Select(t => t.Item2).First();
        Assert.IsTrue(index > 20);
        var index2 = actualLines[index].IndexOf(tag, StringComparison.InvariantCulture);
        Assert.IsTrue(index2 > 50);
        actualLines = actualLines.Select((l, x) => new Tuple<string, int>(l, x))
            .Where(t => t.Item2 >= index && t.Item1.Length > index2 && t.Item1.Substring(25, index2 - 25) == actualLines[index].Substring(25, index2 - 25))
            .Select(t => t.Item1)
            .ToList();
        Assert.IsTrue(actualLines.Count > 2, $"Got only {actualLines.Count} actual log line/-s");
        var startTime = DateTimeFromLogLine(actualLines[0]);
        var endTime = DateTimeFromLogLine(actualLines[^1]);
        var elapsedSeconds = (endTime - startTime).TotalSeconds;
        actualLines = actualLines.Select(s => s.Substring(s.LastIndexOf("\t", StringComparison.CurrentCulture) + 1)).ToList();
        var folderName = GetType().Assembly.Location;
        folderName = folderName.Substring(0, folderName.LastIndexOf('\\'));
        var masterLogFileName = new Folder(folderName).FullName + @"\ExpectedWebViewFlowLog.txt";
        var expectedLines = File.ReadLines(masterLogFileName).ToList();
        Assert.IsTrue(expectedLines.Count <= actualLines.Count, $"Expected {expectedLines.Count} log lines, got {actualLines.Count}");
        for (var i = 0; i < expectedLines.Count; i++) {
            Assert.AreEqual(expectedLines[i], actualLines[i], $"Difference in log line {i + 1}: expected '{expectedLines[i]}', got '{actualLines[i]}'");
        }
        const int maxSeconds = 7;
        Assert.IsTrue(elapsedSeconds < maxSeconds, $"Expected navigation to take less than {maxSeconds} seconds, it was {elapsedSeconds}");
    }

    private DateTime DateTimeFromLogLine(string logLine) {
        var s = logLine.Substring(0, logLine.IndexOf("\t", StringComparison.InvariantCulture));
        s = s + " " + logLine.Substring(s.Length + 1, logLine.Substring(s.Length + 1).IndexOf("\t", StringComparison.InvariantCulture));
        return DateTime.Parse(s);
    }
}