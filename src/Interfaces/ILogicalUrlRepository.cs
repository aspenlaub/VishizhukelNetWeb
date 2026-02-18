using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;

public interface ILogicalUrlRepository {
    Task<string> GetUrlAsync(string name, IErrorsAndInfos errorsAndInfos);
}