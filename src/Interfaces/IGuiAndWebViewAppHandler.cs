using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;

// ReSharper disable UnusedMember.Global

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;

public interface IGuiAndWebViewAppHandler<out TModel> : IGuiAndAppHandler<TModel> where TModel : IWebViewApplicationModelBase {
    Task OnWebViewSourceChangedAsync(string url);
    Task OnWebViewNavigationCompletedAsync(string contentSource, bool isSuccess);

    Task<NavigationResult> NavigateToUrlAsync(string url);
    Task<NavigationResult> NavigateToUrlAsync(string url, NavigateToUrlSettings settings);
    Task<TResult> RunScriptAsync<TResult>(IScriptStatement scriptStatement, bool mayFail, bool maySucceed) where TResult : IScriptCallResponse, new();

    Task WaitUntilNotNavigatingAnymoreAsync();
}