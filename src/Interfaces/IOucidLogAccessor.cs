using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;

public interface IOucidLogAccessor {
    Task WriteOucidAsync(string oucid, OucidResponses oucidResponses, IErrorsAndInfos errorsAndInfos);
    Task<OucidResponse> ReadAndDeleteOucidAsync(string oucid, IErrorsAndInfos errorsAndInfos);
    string AppendOucidToUrl(string url, string oucid, IErrorsAndInfos errorsAndInfos);
    Task<string> GenerateOucidAsync(IErrorsAndInfos errorsAndInfos);
}