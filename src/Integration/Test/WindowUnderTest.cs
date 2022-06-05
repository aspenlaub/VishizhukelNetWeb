using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.GUI;
using Aspenlaub.Net.GitHub.CSharp.VishnetIntegrationTestTools;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Integration.Test;

public class WindowUnderTest : WindowUnderTestActions, IDisposable {
    private readonly IStarterAndStopper _StarterAndStopper;
    public string WindowUnderTestClassName { get; set; } = nameof(VishizhukelNetWebView2Window);

    public WindowUnderTest(ITashAccessor tashAccessor, IStarterAndStopper starterAndStopper) : base(tashAccessor) {
        _StarterAndStopper = starterAndStopper;
    }

    public override async Task InitializeAsync() {
        await base.InitializeAsync();
        _StarterAndStopper.Start(WindowUnderTestClassName);
    }

    public void Dispose() {
        _StarterAndStopper.Stop();
    }
}