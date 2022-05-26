namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;

public class OucidResponse {
    public bool IsDefaultResponse { get; set; } = true;
    public bool WaitForLocalhostLogs { get; set; } = true;
    public bool WaitUntilNotNavigating { get; set; } = true;
    public bool BasicValidation { get; set; } = true;
    public bool WaitAgainForLocalhostLogs { get; set; } = true;
    public bool WaitAgainUntilNotNavigating { get; set; } = true;
    public bool MarkupValidation { get; set; } = true;
}