using System.Threading.Tasks;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;

public interface IWebViewNavigatingHelper {
    Task<bool> WaitUntilNotNavigatingAnymoreAsync(string url, int timeoutInSeconds);
}