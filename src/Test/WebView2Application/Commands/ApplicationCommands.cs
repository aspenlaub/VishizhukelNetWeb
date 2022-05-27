using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Commands;

public class ApplicationCommands : IApplicationCommands {
    public ICommand GoToUrlCommand { get; set; }
    public ICommand RunJsCommand { get; set; }
    public ICommand RunTestCaseCommand { get; set; }
}