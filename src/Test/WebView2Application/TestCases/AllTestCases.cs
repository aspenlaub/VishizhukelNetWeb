using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.TestCases;

public class AllTestCases {
    public static IList<ITestCase> Instance { get; } = new List<ITestCase> {
        new AlwaysSucceeds(), new AlwaysFails(),
        new CanFindLamasMainByClass(), new CannotFindAnchorWhichIsNotDivLike(),
        new CanFindBody(), new CanFindAnchor()
    };
}