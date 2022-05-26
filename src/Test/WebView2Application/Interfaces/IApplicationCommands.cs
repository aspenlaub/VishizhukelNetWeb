using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;

public interface IApplicationCommands {
    ICommand GoToUrlCommand { get; set; }
    ICommand RunJsCommand { get; set; }
}