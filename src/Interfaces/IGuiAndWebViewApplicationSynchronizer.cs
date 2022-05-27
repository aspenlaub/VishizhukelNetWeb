using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;

public interface IGuiAndWebViewApplicationSynchronizer<out TModel>
    : IGuiAndApplicationSynchronizer<TModel>
    where TModel : IWebViewApplicationModelBase {
    Task<TResult> RunScriptAsync<TResult>(IScriptStatement scriptStatement) where TResult : IScriptCallResponse, new();
    Task WaitUntilNotNavigatingAnymoreAsync();
}