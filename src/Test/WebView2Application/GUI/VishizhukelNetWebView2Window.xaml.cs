using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.GUI;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Helpers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Extensions;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Autofac;
using Moq;
using IContainer = Autofac.IContainer;
using WindowsApplication = System.Windows.Application;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.GUI;

// ReSharper disable once UnusedMember.Global
public partial class VishizhukelNetWebView2Window : IAsyncDisposable {
    private static IContainer Container { get; set; }

    private Application.Application Application;
    private ApplicationModel ApplicationModel;
    private ITashTimer<ApplicationModel> TashTimer;

    public bool IsWindowUnderTest { get; set; }

    public VishizhukelNetWebView2Window() {
        InitializeComponent();
    }

    private async Task BuildContainerIfNecessaryAsync() {
        if (Container != null) { return; }

        var logConfigurationMock = new Mock<ILogConfiguration>();
        logConfigurationMock.SetupGet(lc => lc.LogSubFolder).Returns(@"AspenlaubLogs\" + nameof(VishizhukelNetWebView2Window));
        logConfigurationMock.SetupGet(lc => lc.LogId).Returns($"{DateTime.Today:yyyy-MM-dd}-{Process.GetCurrentProcess().Id}");
        logConfigurationMock.SetupGet(lc => lc.DetailedLogging).Returns(true);
        var builder = await new ContainerBuilder().UseApplicationAsync(this, logConfigurationMock.Object);
        Container = builder.Build();
    }

    private async void OnLoadedAsync(object sender, RoutedEventArgs e) {
        await BuildContainerIfNecessaryAsync();

        Application = Container.Resolve<Application.Application>();
        ApplicationModel = Container.Resolve<ApplicationModel>();

        const string url = "https://www.viperfisch.de/js/bootstrap.min-v24070.js";
        ApplicationModel.WebView
            .OnDocumentLoaded.AppendStatement($"var script = document.createElement(\"script\"); script.src = \"{url}\"; document.head.appendChild(script);");

        await Application.OnLoadedAsync();

        var commands = Application.Commands;

        var guiToAppGate = Container.Resolve<IGuiToWebViewApplicationGate>();
        var buttonNameToCommandMapper = Container.Resolve<IButtonNameToCommandMapper>();

        guiToAppGate.RegisterAsyncTextBoxCallback(WebViewUrl, t => Application.Handlers.WebViewUrlTextHandler.TextChangedAsync(t));
        guiToAppGate.RegisterAsyncTextBoxCallback(WebViewContentSource, t => Application.Handlers.WebViewContentSourceTextHandler.TextChangedAsync(t));

        guiToAppGate.WireButtonAndCommand(GoToUrl, commands.GoToUrlCommand, buttonNameToCommandMapper);
        guiToAppGate.WireButtonAndCommand(RunJs, commands.RunJsCommand, buttonNameToCommandMapper);

        guiToAppGate.WireWebView(WebView);

        if (IsWindowUnderTest) {
            TashTimer = new TashTimer<ApplicationModel>(Container.Resolve<ITashAccessor>(), Application.TashHandler, guiToAppGate);
            if (!await TashTimer.ConnectAndMakeTashRegistrationReturnSuccessAsync(Properties.Resources.WebViewWindowTitle)) {
                Close();
            }

            TashTimer.CreateAndStartTimer(Application.CreateTashTaskHandlingStatus());
        }

        await ExceptionHandler.RunAsync(WindowsApplication.Current, TimeSpan.FromSeconds(5));
    }

    public async ValueTask DisposeAsync() {
        if (TashTimer == null) { return; }

        await TashTimer.StopTimerAndConfirmDeadAsync(false);
    }

    private async void OnClosing(object sender, CancelEventArgs e) {
        if (TashTimer == null) { return; }

        e.Cancel = true;

        await TashTimer.StopTimerAndConfirmDeadAsync(false);

        WindowsApplication.Current.Shutdown();
    }

    private void OnStateChanged(object sender, EventArgs e) {
        Application.OnWindowStateChanged(WindowState);
    }
}