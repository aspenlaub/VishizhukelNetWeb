﻿using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.GUI;
using Aspenlaub.Net.GitHub.CSharp.VishnetIntegrationTestTools;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Integration.Test;

public class WindowUnderTest : WindowUnderTestActions, IDisposable {
    private readonly IStarterAndStopper StarterAndStopper;
    public string WindowUnderTestClassName { get; set; } = nameof(VishizhukelNetWebView2Window);

    public WindowUnderTest(ITashAccessor tashAccessor, IStarterAndStopper starterAndStopper) : base(tashAccessor) {
        StarterAndStopper = starterAndStopper;
    }

    public void Initialize() {
        StarterAndStopper.Start(WindowUnderTestClassName);
    }

    public void Dispose() {
        StarterAndStopper.Stop();
    }
}