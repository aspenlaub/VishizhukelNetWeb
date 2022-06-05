using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.GUI;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Helpers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Extensions;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Autofac;
using Microsoft.Web.WebView2.Core;
using IContainer = Autofac.IContainer;
using WindowsApplication = System.Windows.Application;

[assembly:InternalsVisibleTo("Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Integration.Test")]
namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.GUI;

// ReSharper disable once UnusedMember.Global
public partial class VishizhukelNetWebView2Window : IAsyncDisposable {
    private static IContainer Container { get; set; }

    private Application.Application _Application;
    private ApplicationModel _ApplicationModel;
    private ITashTimer<ApplicationModel> _TashTimer;

    public bool IsWindowUnderTest { get; set; }

    public VishizhukelNetWebView2Window() {
        InitializeComponent();
#pragma warning disable CS4014
        InitializeBrowserAsync();
#pragma warning restore CS4014
    }

    private async Task InitializeBrowserAsync() {
        var cacheFolder = new Folder(Path.GetTempPath()).SubFolder("AspenlaubTemp").SubFolder("WebViewCache");
        cacheFolder.CreateIfNecessary();
        var webView2Environment = await CoreWebView2Environment.CreateAsync(null, cacheFolder.FullName);
        await WebView.EnsureCoreWebView2Async(webView2Environment);
    }

    private async Task BuildContainerIfNecessaryAsync() {
        if (Container != null) { return; }

        var builder = await new ContainerBuilder().UseApplicationAsync(this);
        Container = builder.Build();
    }

    private async void OnLoadedAsync(object sender, RoutedEventArgs e) {
        await BuildContainerIfNecessaryAsync();

        _Application = Container.Resolve<Application.Application>();
        _ApplicationModel = Container.Resolve<ApplicationModel>();

        const string url = "https://www.viperfisch.de/js/bootstrap.min-v24070.js";
        _ApplicationModel.WebView
            .OnDocumentLoaded.AppendStatement($"var script = document.createElement(\"script\"); script.src = \"{url}\"; document.head.appendChild(script);");

        await _Application.OnLoadedAsync();

        var commands = _Application.Commands;

        var guiToAppGate = Container.Resolve<IGuiToWebViewApplicationGate>();
        var buttonNameToCommandMapper = Container.Resolve<IButtonNameToCommandMapper>();

        guiToAppGate.RegisterAsyncTextBoxCallback(WebViewUrl, t => _Application.Handlers.WebViewUrlTextHandler.TextChangedAsync(t));
        guiToAppGate.RegisterAsyncTextBoxCallback(WebViewContentSource, t => _Application.Handlers.WebViewContentSourceTextHandler.TextChangedAsync(t));

        guiToAppGate.WireButtonAndCommand(GoToUrl, commands.GoToUrlCommand, buttonNameToCommandMapper);
        guiToAppGate.WireButtonAndCommand(RunJs, commands.RunJsCommand, buttonNameToCommandMapper);
        guiToAppGate.WireButtonAndCommand(RunTestCase, commands.RunTestCaseCommand, buttonNameToCommandMapper);

        guiToAppGate.WireWebView(WebView);

        var handlers = _Application.Handlers;
        guiToAppGate.RegisterAsyncSelectorCallback(SelectedTestCase, i => handlers.TestCaseSelectorHandler.TestCasesSelectedIndexChangedAsync(i, false));

        if (IsWindowUnderTest) {
            _TashTimer = new TashTimer<ApplicationModel>(Container.Resolve<ITashAccessor>(), _Application.TashHandler, guiToAppGate);
            if (!await _TashTimer.ConnectAndMakeTashRegistrationReturnSuccessAsync(Properties.Resources.WebViewWindowTitle)) {
                Close();
            }

            _TashTimer.CreateAndStartTimer(_Application.CreateTashTaskHandlingStatus());
        }

        await ExceptionHandler.RunAsync(WindowsApplication.Current, TimeSpan.FromSeconds(5));
    }

    public async ValueTask DisposeAsync() {
        if (_TashTimer == null) { return; }

        await _TashTimer.StopTimerAndConfirmDeadAsync(false);
    }

    private async void OnClosing(object sender, CancelEventArgs e) {
        if (_TashTimer == null) { return; }

        e.Cancel = true;

        await _TashTimer.StopTimerAndConfirmDeadAsync(false);

        WindowsApplication.Current.Shutdown();
    }

    private void OnStateChanged(object sender, EventArgs e) {
        _Application.OnWindowStateChanged(WindowState);
    }
}