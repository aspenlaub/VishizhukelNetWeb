using System.Threading.Tasks;

// ReSharper disable UnusedMemberInSuper.Global

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;

public interface IWebViewNavigationHelper {
    Task<bool> NavigateToUrlAsync(string url);
    Task<bool> NavigateToUrlImprovedAsync(string url);
}