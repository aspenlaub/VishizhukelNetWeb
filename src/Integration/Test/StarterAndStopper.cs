using Aspenlaub.Net.GitHub.CSharp.VishnetIntegrationTestTools;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Integration.Test;

public class StarterAndStopper : StarterAndStopperBase {
    protected override string ProcessName => "Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Test";
    protected override List<string> AdditionalProcessNamesToStop => new();
    protected override string ExecutableFile() {
        return typeof(WindowUnderTest).Assembly.Location
            .Replace(@"\Integration\Test\", @"\Test\")
            .Replace("Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Integration.Test.dll", ProcessName + ".exe");
    }
}