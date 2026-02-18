using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Seoa.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Components;
using Autofac;

[assembly: DoNotParallelize]
namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test;

[TestClass]
public class LogicalUrlRepositoryTest {
    [TestMethod]
    public async Task CanGetLogicalUrls() {
        IContainer container = new ContainerBuilder().UsePegh("VishizhukelNetWeb").Build();
        ISecretRepository secretRepository = container.Resolve<ISecretRepository>();
        var sut = new LogicalUrlRepository(secretRepository);
        var errorsAndInfos = new ErrorsAndInfos();
        string url = await sut.GetUrlAsync("Localhost", errorsAndInfos);
        Assert.That.ThereWereNoErrors(errorsAndInfos);
        Assert.AreEqual("http://localhost", url);
    }
}