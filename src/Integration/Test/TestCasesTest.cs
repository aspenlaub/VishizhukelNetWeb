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
        using var sut = await CreateWindowUnderTestAsync(nameof(VishizhukelNetWebView2Window));
        var process = await sut.FindIdleProcessAsync();
        var tasks = new List<ControllableProcessTask> {
            sut.CreateSetValueTask(process, nameof(ApplicationModel.SelectedTestCase), VishizhukelNetWebTestResources.AlwaysSucceeds),
            sut.CreatePressButtonTask(process, nameof(ApplicationModel.RunTestCase))
        };
        await sut.RemotelyProcessTaskListAsync(process, tasks);
    }
}