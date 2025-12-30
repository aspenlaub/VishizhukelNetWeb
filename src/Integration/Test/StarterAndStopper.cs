using Aspenlaub.Net.GitHub.CSharp.VishnetIntegrationTestTools;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Integration.Test;

public class StarterAndStopper : StarterAndStopperBase {
    protected override string ProcessName => "Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test";
    protected override List<string> AdditionalProcessNamesToStop => [];
    protected override string ExecutableFile() {
        string result = typeof(WindowUnderTest).Assembly.Location
                                               .Replace(@"\Integration\Test\", @"\Test\")
                                               .Replace("Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Integration.Test.dll", ProcessName + ".exe");
        if (!File.Exists(result)) {
            throw new FileNotFoundException(result);
        }
        return result;
    }
}