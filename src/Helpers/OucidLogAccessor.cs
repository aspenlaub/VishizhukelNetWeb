using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Extensions;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Helpers;

public class OucidLogAccessor : IOucidLogAccessor {
    private readonly IFolderResolver _FolderResolver;
    protected IFolder OucidLogFolder;

    public OucidLogAccessor(IFolderResolver folderResolver) {
        _FolderResolver = folderResolver;
    }

    protected async Task<bool> SetOucidLogFolderIfNecessaryAsync(IErrorsAndInfos errorsAndInfos) {
        if (OucidLogFolder != null) { return true; }

        OucidLogFolder = await _FolderResolver.ResolveAsync(@"$(OucidLog)", errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) { return false; }

        OucidLogFolder.CreateIfNecessary();
        return true;
    }

    public async Task WriteOucidAsync(string oucid, OucidResponses oucidResponses, IErrorsAndInfos errorsAndInfos) {
        if (!await SetOucidLogFolderIfNecessaryAsync(errorsAndInfos)) { return; }

        var fileName = OucidFileName(oucid);
        await File.WriteAllTextAsync(fileName, JsonSerializer.Serialize(oucidResponses, new JsonSerializerOptions { WriteIndented = true }));
    }

    public async Task<OucidResponse> ReadAndDeleteOucidAsync(string oucid, IErrorsAndInfos errorsAndInfos) {
        if (!await SetOucidLogFolderIfNecessaryAsync(errorsAndInfos)) { return new OucidResponse(); }

        var fileName = OucidFileName(oucid);
        if (!File.Exists(fileName)) {
            return new OucidResponse();
        }

        var result = new OucidResponse();
        try {
            result = JsonSerializer.Deserialize<OucidResponses>(await File.ReadAllTextAsync(fileName)).AggregateResponse();
            // ReSharper disable once EmptyGeneralCatchClause
        } catch {
        }

        File.Delete(fileName);
        return result;
    }

    public string AppendOucidToUrl(string url, string oucid, IErrorsAndInfos errorsAndInfos) {
        if (url.Contains("oucid=")) {
            errorsAndInfos.Errors.Add(Properties.Resources.OucidAlreadyPresentInUrl);
            return url;
        }

        url = url + (url.Contains('?') ? '&' : '?') + "oucid=" + oucid;
        return url;
    }

    public async Task<string> GenerateOucidAsync(IErrorsAndInfos errorsAndInfos) {
        var oucid = Guid.NewGuid().ToString();
        oucid = oucid.Substring(0, oucid.IndexOf('-'));

        var oucidResponses = new OucidResponses { new() };

        await WriteOucidAsync(oucid, oucidResponses, errorsAndInfos);

        return oucid;
    }

    protected string OucidFileName(string oucid) {
        return OucidLogFolder.FullName + $"\\OUCID-{oucid}.json";
    }
}