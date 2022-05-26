using System.Windows;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.GUI;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App {
    public static bool IsIntegrationTest { get; private set; }

    private void LaunchWindows(object sender, StartupEventArgs e) {
        IsIntegrationTest = e.Args.Any(a => a == "/UnitTest");
        var window = new VishizhukelNetWebView2Window { IsWindowUnderTest = true };
        window.Show();
    }
}