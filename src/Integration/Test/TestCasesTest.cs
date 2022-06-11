using Aspenlaub.Net.GitHub.CSharp.Tash;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.GUI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            VishizhukelNetWebTestResources.CanWaitForStartOfNavigationWhenGoingToUrl, VishizhukelNetWebTestResources.CanUseImprovedNavigationToUrl,
            VishizhukelNetWebTestResources.CanGetOucidResponse
        };

        await RunTestCasesAsync(testCaseNames);
    }

    [TestMethod]
    public async Task CanRunAlwaysFailsTestCase() {
        using var sut = await CreateWindowUnderTestAsync(nameof(VishizhukelNetWebView2Window));
        var process = await sut.FindIdleProcessAsync();
        var testCaseName = VishizhukelNetWebTestResources.AlwaysFails;
        var tasks = new List<ControllableProcessTask> {
            sut.CreateSetValueTask(process, nameof(ApplicationModel.SelectedTestCase), testCaseName),
        };
        await sut.RemotelyProcessTaskListAsync(process, tasks);
        await sut.RemotelyPressButtonAsync(process, nameof(ApplicationModel.RunTestCase), false);
        tasks.Clear();
        tasks.Add(sut.CreateVerifyValueTask(process, nameof(ApplicationModel.Status), testCaseName));
        await sut.RemotelyProcessTaskListAsync(process, tasks);
    }

    public async Task RunTestCaseAsync(string testCaseName) {
        await RunTestCasesAsync(new List<string> { testCaseName });
    }

    public async Task RunTestCasesAsync(List<string> testCaseNames) {
        using var sut = await CreateWindowUnderTestAsync(nameof(VishizhukelNetWebView2Window));
        var process = await sut.FindIdleProcessAsync();
        var tasks = new List<ControllableProcessTask>();
        foreach(var testCaseName in testCaseNames) {
            tasks.Add(sut.CreateSetValueTask(process, nameof(ApplicationModel.SelectedTestCase), testCaseName));
            tasks.Add(sut.CreatePressButtonTask(process, nameof(ApplicationModel.RunTestCase)));
        }

        await sut.RemotelyProcessTaskListAsync(process, tasks);
    }
}