using Aspenlaub.Net.GitHub.CSharp.Tash;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.GUI;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VishizhukelNetWebTestResources = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.Properties.Resources;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Integration.Test;

[TestClass]
public class TestCasesTest : IntegrationTestBase {
    [TestMethod]
    public async Task CanRunAlwaysSucceedsTestCase() {
        await RunTestCaseAsync(VishizhukelNetWebTestResources.AlwaysSucceeds, "");
    }

    [TestMethod]
    public async Task CanRunAlwaysFailsTestCase() {
        await RunTestCaseAsync(VishizhukelNetWebTestResources.AlwaysFails, VishizhukelNetWebTestResources.AlwaysFails);
    }

    public async Task RunTestCaseAsync(string testCaseName, string errorMessage) {
        using var sut = await CreateWindowUnderTestAsync(nameof(VishizhukelNetWebView2Window));
        var process = await sut.FindIdleProcessAsync();
        var tasks = new List<ControllableProcessTask> {
            sut.CreateSetValueTask(process, nameof(ApplicationModel.SelectedTestCase), testCaseName),
        };
        if (string.IsNullOrEmpty(errorMessage)) {
            tasks.Add(sut.CreatePressButtonTask(process, nameof(ApplicationModel.RunTestCase)));
        }
        await sut.RemotelyProcessTaskListAsync(process, tasks);
        if (!string.IsNullOrEmpty(errorMessage)) {
            await sut.RemotelyPressButtonAsync(process, nameof(ApplicationModel.RunTestCase), false);

            tasks.Clear();
            tasks.Add(sut.CreateVerifyValueTask(process, nameof(ApplicationModel.Status), errorMessage));
            await sut.RemotelyProcessTaskListAsync(process, tasks);
        }
    }
}