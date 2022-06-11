using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;

// ReSharper disable UnusedMember.Global

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;

public interface IGuiAndWebViewAppHandler<out TModel> : IGuiAndAppHandler<TModel> where TModel : IWebViewApplicationModelBase {
    Task OnWebViewSourceChangedAsync(string uri);
    Task OnWebViewNavigationCompletedAsync(string contentSource, bool isSuccess);

    Task<bool> NavigateToUrlAsync(string url);
    Task<bool> NavigateToUrlAsync(string url, NavigateToUrlSettings settings, IErrorsAndInfos errorsAndInfos);
    Task<TResult> RunScriptAsync<TResult>(IScriptStatement scriptStatement, bool mayFail, bool maySucceed) where TResult : IScriptCallResponse, new();

    Task WaitUntilNotNavigatingAnymoreAsync();
}