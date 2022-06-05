using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tash;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Integration.Test;

public class IntegrationTestBase {
    protected readonly IContainer Container;

    public IntegrationTestBase() {
        Container = new ContainerBuilder().RegisterForIntegrationTest().Build();
    }

    protected async Task<WindowUnderTest> CreateWindowUnderTestAsync(string windowUnderTestClassName) {
        var sut = Container.Resolve<WindowUnderTest>();
        sut.WindowUnderTestClassName = windowUnderTestClassName;
        await sut.InitializeAsync();
        var process = await sut.FindIdleProcessAsync();
        var tasks = new List<ControllableProcessTask> {
            sut.CreateResetTask(process)
        };
        await sut.RemotelyProcessTaskListAsync(process, tasks);
        return sut;
    }

    protected List<ISimpleLogEntry> GetLogEntriesInFilesChangedAfter(IFolder logFolder, DateTime minChangedAt) {
        var logFileNames = Directory.GetFiles(logFolder.FullName, "*.log")
                                    .Where(f => File.GetLastWriteTime(f) > minChangedAt)
                                    .ToList();
        Assert.IsTrue(logFileNames.Any(), "No log files found");
        var reader = Container.Resolve<ISimpleLogReader>();
        var actualLogEntries = new List<ISimpleLogEntry>();
        foreach (var logFileName in logFileNames) {
            actualLogEntries.AddRange(reader.ReadLogFile(logFileName));
        }

        return actualLogEntries;
    }

    protected static void VerifyThatEachExpectedLineIsInReducedLog(List<string> expectedLines, IReadOnlyCollection<ISimpleLogEntry> reducedActualLogEntries, IReadOnlyList<ISimpleLogEntry> actualLogEntries) {
        foreach (var expectedLine in expectedLines) {
            var logEntry = reducedActualLogEntries.FirstOrDefault(l => l.Message.Contains(expectedLine));
            var index = actualLogEntries.Select((l, x) => new Tuple<string, int>(l.Message, x)).Where(t => t.Item1.Contains(expectedLine)).Select(t => t.Item2).First();
            var scopes = actualLogEntries[index].Stack;
            Assert.IsNotNull(logEntry, $"Line \"{expectedLine}\" not found in reduced list of log entries, scopes are {string.Join(';', scopes)}");
        }
    }
}