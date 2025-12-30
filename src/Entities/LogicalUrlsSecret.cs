using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;

public class LogicalUrlsSecret : ISecret<LogicalUrls> {
    private LogicalUrls _LogicalFolders;
    public LogicalUrls DefaultValue => _LogicalFolders ??= [
        new() { Name = "Localhost", Url = "http://localhost" }
    ];

    public string Guid => "7240F3DC9-F171-4968-A115-CA7624FEE988";
}