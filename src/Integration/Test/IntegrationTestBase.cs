using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tash;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Integration.Test;

public class IntegrationTestBase {
    protected readonly IContainer Container = new ContainerBuilder().RegisterForIntegrationTest().Build();

    protected async Task<WindowUnderTest> CreateWindowUnderTestAsync(string windowUnderTestClassName) {
        WindowUnderTest sut = Container.Resolve<WindowUnderTest>();
        sut.WindowUnderTestClassName = windowUnderTestClassName;
        await sut.InitializeAsync();
        ControllableProcess process = await sut.FindIdleProcessAsync();
        var tasks = new List<ControllableProcessTask> {
            sut.CreateResetTask(process)
        };
        await sut.RemotelyProcessTaskListAsync(process, tasks, false, (_, _) => Task.CompletedTask);
        return sut;
    }

    protected List<ISimpleLogEntry> GetLogEntriesInFilesChangedAfter(IFolder logFolder, DateTime minChangedAt) {
        var logFileNames = Directory.GetFiles(logFolder.FullName, "*.log")
                                    .Where(f => File.GetLastWriteTime(f) > minChangedAt)
                                    .ToList();
        Assert.IsTrue(logFileNames.Any(), "No log files found");
        ISimpleLogReader reader = Container.Resolve<ISimpleLogReader>();
        var actualLogEntries = new List<ISimpleLogEntry>();
        foreach (string logFileName in logFileNames) {
            actualLogEntries.AddRange(reader.ReadLogFile(logFileName));
        }

        return actualLogEntries;
    }

    protected static void VerifyThatEachExpectedLineIsInReducedLog(List<string> expectedLines, IReadOnlyCollection<ISimpleLogEntry> reducedActualLogEntries, IReadOnlyList<ISimpleLogEntry> actualLogEntries) {
        foreach (string expectedLine in expectedLines.Where(l => !l.StartsWith('{'))) {
            ISimpleLogEntry logEntry = reducedActualLogEntries.FirstOrDefault(l => l.Message.Contains(expectedLine));
            int index = actualLogEntries.Select((l, x) => new Tuple<string, int>(l.Message, x)).Where(t => t.Item1.Contains(expectedLine)).Select(t => t.Item2).FirstOrDefault();
            if (index >= 0) {
                List<string> scopes = actualLogEntries[index].Stack;
                Assert.IsNotNull(logEntry, $"Line \"{expectedLine}\" not found in reduced list of log entries, scopes are {string.Join(';', scopes)}");
            } else {
                Assert.IsNotNull(logEntry, $"Line \"{expectedLine}\" not found in reduced list of log entries");
            }
        }
    }
}