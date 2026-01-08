using System.Threading.Tasks;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;

public interface IWebViewNavigatingHelper {
    int TimeoutInSeconds { get; }
    Task<bool> WaitUntilNotNavigatingAnymoreAsync(string url);
}