using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;
// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;

public interface IOucidLogAccessor {
    Task WriteOucidAsync(string oucid, OucidResponses oucidResponses, IErrorsAndInfos errorsAndInfos);
    Task<OucidResponse> ReadAndDeleteOucidAsync(string oucid, IErrorsAndInfos errorsAndInfos);
    string AppendOucidToUrl(string url, string oucid, IErrorsAndInfos errorsAndInfos);
    string RemoveOucidFromUrl(string url);
    Task<string> GenerateOucidAsync(IErrorsAndInfos errorsAndInfos);
}