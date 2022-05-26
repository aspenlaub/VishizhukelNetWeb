using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Extensions;

public static class OucidResponsesExtensions {
    public static OucidResponse AggregateResponse(this OucidResponses oucidResponses) {
        if (oucidResponses.All(x => x.IsDefaultResponse)) {
            return oucidResponses.First();
        }

        var nonDefaultResponse = oucidResponses.Where(x => !x.IsDefaultResponse).ToList();
        return new OucidResponse {
            IsDefaultResponse = false,
            WaitForLocalhostLogs = nonDefaultResponse.Any(x => x.WaitForLocalhostLogs),
            WaitUntilNotNavigating = nonDefaultResponse.Any(x => x.WaitUntilNotNavigating),
            BasicValidation = nonDefaultResponse.Any(x => x.BasicValidation),
            WaitAgainForLocalhostLogs = nonDefaultResponse.Any(x => x.WaitAgainForLocalhostLogs),
            WaitAgainUntilNotNavigating = nonDefaultResponse.Any(x => x.WaitAgainUntilNotNavigating),
            MarkupValidation = nonDefaultResponse.Any(x => x.MarkupValidation)
          };
    }
}