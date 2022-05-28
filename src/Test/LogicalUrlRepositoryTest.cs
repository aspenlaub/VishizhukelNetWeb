using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Components;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test;

[TestClass]
public class LogicalUrlRepositoryTest {
    [TestMethod]
    public async Task CanGetLogicalUrls() {
        var container = new ContainerBuilder().UsePegh("VishizhukelNetWeb", new DummyCsArgumentPrompter()).Build();
        var secretRepository = container.Resolve<ISecretRepository>();
        var sut = new LogicalUrlRepository(secretRepository);
        var errorsAndInfos = new ErrorsAndInfos();
        var url = await sut.GetUrlAsync("Localhost", errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.AreEqual("http://localhost", url);
    }
}