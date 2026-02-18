using Aspenlaub.Net.GitHub.CSharp.Skladasu.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;

public class NavigationResult {
    public bool Succeeded { get; set; }
    public IErrorsAndInfos ErrorsAndInfos { get; set; }
    public OucidResponse OucidResponse { get; set; }

    public static NavigationResult Failure(IErrorsAndInfos errorsAndInfos) {
        return new NavigationResult { Succeeded = false, ErrorsAndInfos = errorsAndInfos };
    }

    public static NavigationResult Success(IErrorsAndInfos errorsAndInfos) {
        return Success(errorsAndInfos, new OucidResponse());
    }

    public static NavigationResult Success(IErrorsAndInfos errorsAndInfos, OucidResponse oucidResponse) {
        return new NavigationResult { Succeeded = true, ErrorsAndInfos = errorsAndInfos, OucidResponse = oucidResponse };
    }
}