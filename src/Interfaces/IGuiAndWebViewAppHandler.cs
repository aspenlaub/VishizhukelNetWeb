using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
// ReSharper disable UnusedMember.Global

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;

public interface IGuiAndWebViewAppHandler<out TModel> : IGuiAndAppHandler<TModel> where TModel : IWebViewApplicationModelBase {
    Task OnWebViewSourceChangedAsync(string uri);
    Task OnWebViewNavigationCompletedAsync(string contentSource, bool isSuccess);

    Task<bool> NavigateToUrlAsync(string url);
    Task<TResult> RunScriptAsync<TResult>(IScriptStatement scriptStatement, bool mayFail, bool maySucceed) where TResult : IScriptCallResponse, new();

    Task WaitUntilNotNavigatingAnymoreAsync();
    Task NavigateToUrlAndWaitForStartOfNavigationAsync(string url, IErrorsAndInfos errorsAndInfos);
}