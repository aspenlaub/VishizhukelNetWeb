using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.GUI;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.GUI;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application;

public class GuiAndApplicationSynchronizer : GuiAndWebViewApplicationSynchronizerBase<ApplicationModel, VishizhukelNetWebView2Window> {
    public GuiAndApplicationSynchronizer(ApplicationModel model, VishizhukelNetWebView2Window window, ISimpleLogger simpleLogger) : base(model, window, simpleLogger) {
    }
}