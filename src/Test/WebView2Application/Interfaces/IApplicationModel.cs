using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Controls;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;

public interface IApplicationModel : IWebViewApplicationModelBase {
    Button GoToUrl { get; }
    Button RunJs { get; }
    ISelector SelectedTestCase { get; }
    Button RunTestCase { get; }
}