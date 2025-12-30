using Aspenlaub.Net.GitHub.CSharp.Tash;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.GUI;
using VishizhukelNetWebTestResources = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.Properties.Resources;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Integration.Test;

[TestClass]
public class TestCasesTest : IntegrationTestBase {
    [TestMethod]
    public async Task CanRunAlwaysSucceedsTestCase() {
        await RunTestCaseAsync(VishizhukelNetWebTestResources.AlwaysSucceeds);
    }

    [TestMethod]
    public async Task CanRunCanFindTestCases() {
        var testCaseNames = new List<string> {
            VishizhukelNetWebTestResources.CanFindAnchor, VishizhukelNetWebTestResources.CanFindBody,
            VishizhukelNetWebTestResources.CanFindLamasMainByClass, VishizhukelNetWebTestResources.CannotFindAnchorWhichIsNotDivLike
        };

        await RunTestCasesAsync(testCaseNames);
    }

    [TestMethod]
    public async Task CanRunWebViewImprovementTestCases() {
        var testCaseNames = new List<string> {
            VishizhukelNetWebTestResources.CanWaitForStartOfNavigationWhenGoingToUrl, VishizhukelNetWebTestResources.CanNavigateToUrl,
            VishizhukelNetWebTestResources.CanGetOucidResponse
        };

        await RunTestCasesAsync(testCaseNames);
    }

    [TestMethod]
    public async Task CanRunAlwaysFailsTestCase() {
        using WindowUnderTest sut = await CreateWindowUnderTestAsync(nameof(VishizhukelNetWebView2Window));
        ControllableProcess process = await sut.FindIdleProcessAsync();
        string testCaseName = VishizhukelNetWebTestResources.AlwaysFails;
        var tasks = new List<ControllableProcessTask> {
            sut.CreateSetValueTask(process, nameof(ApplicationModel.SelectedTestCase), testCaseName),
        };
        await sut.RemotelyProcessTaskListAsync(process, tasks, false, (_, _) => Task.CompletedTask);
        await sut.RemotelyPressButtonAsync(process, nameof(ApplicationModel.RunTestCase), false);
        tasks.Clear();
        tasks.Add(sut.CreateVerifyValueTask(process, nameof(ApplicationModel.Status), testCaseName));
        await sut.RemotelyProcessTaskListAsync(process, tasks, false, (_, _) => Task.CompletedTask);
    }

    public async Task RunTestCaseAsync(string testCaseName) {
        await RunTestCasesAsync([testCaseName]);
    }

    public async Task RunTestCasesAsync(List<string> testCaseNames) {
        using WindowUnderTest sut = await CreateWindowUnderTestAsync(nameof(VishizhukelNetWebView2Window));
        ControllableProcess process = await sut.FindIdleProcessAsync();
        var tasks = new List<ControllableProcessTask>();
        foreach(string testCaseName in testCaseNames) {
            tasks.Add(sut.CreateSetValueTask(process, nameof(ApplicationModel.SelectedTestCase), testCaseName));
            tasks.Add(sut.CreatePressButtonTask(process, nameof(ApplicationModel.RunTestCase)));
        }

        await sut.RemotelyProcessTaskListAsync(process, tasks, false, (_, _) => Task.CompletedTask);
    }
}