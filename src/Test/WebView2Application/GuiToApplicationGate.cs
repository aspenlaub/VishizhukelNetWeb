using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.GUI;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using VishizhukelWebView2Application = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Application.Application;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application;

public class GuiToApplicationGate : GuiToWebViewApplicationGateBase<Application.Application, ApplicationModel> {
    public GuiToApplicationGate(IBusy busy, VishizhukelWebView2Application application, IOucidLogAccessor oucidLogAccessor) : base(busy, application, oucidLogAccessor) {
    }
}