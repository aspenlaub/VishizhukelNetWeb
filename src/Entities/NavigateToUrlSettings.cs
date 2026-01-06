namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;

public class NavigateToUrlSettings {
    public const int QuickSeconds = 5;
    public const int DefaultTimeoutInSeconds = 600;

    public bool StopAfterNavigationStarted { get; set; } = false;
    public bool StopAfterOucidResponse { get; set; } = false;
    public int TimeoutInSeconds { get; set; } = DefaultTimeoutInSeconds;
}