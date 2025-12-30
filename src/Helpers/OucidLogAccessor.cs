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

public class OucidLogAccessor(IFolderResolver folderResolver) : IOucidLogAccessor {
    protected IFolder OucidLogFolder;

    protected async Task<bool> SetOucidLogFolderIfNecessaryAsync(IErrorsAndInfos errorsAndInfos) {
        if (OucidLogFolder != null) { return true; }

        OucidLogFolder = await folderResolver.ResolveAsync(@"$(OucidLog)", errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) { return false; }

        OucidLogFolder.CreateIfNecessary();
        return true;
    }

    public async Task WriteOucidAsync(string oucid, OucidResponses oucidResponses, IErrorsAndInfos errorsAndInfos) {
        if (!await SetOucidLogFolderIfNecessaryAsync(errorsAndInfos)) { return; }

        string fileName = OucidFileName(oucid);
        await File.WriteAllTextAsync(fileName, JsonSerializer.Serialize(oucidResponses, new JsonSerializerOptions { WriteIndented = true }));
    }

    public async Task<OucidResponse> ReadAndDeleteOucidAsync(string oucid, IErrorsAndInfos errorsAndInfos) {
        if (!await SetOucidLogFolderIfNecessaryAsync(errorsAndInfos)) { return new OucidResponse(); }

        string fileName = OucidFileName(oucid);
        if (!File.Exists(fileName)) {
            return new OucidResponse();
        }

        var result = new OucidResponse();
        try {
            result = JsonSerializer.Deserialize<OucidResponses>(await File.ReadAllTextAsync(fileName)).AggregateResponse();
        } catch {
            errorsAndInfos.Errors.Add(Properties.Resources.CouldNotDeserializeOucidResponse);
        }

        File.Delete(fileName);
        return result;
    }

    public string AppendOucidToUrl(string url, string oucid, IErrorsAndInfos errorsAndInfos) {
        if (url.Contains("oucid=")) {
            errorsAndInfos.Errors.Add(Properties.Resources.OucidAlreadyPresentInUrl);
            return url;
        }

        if (!url.StartsWith("http://localhost")) {
            return url;
        }

        url = url + (url.Contains('?') ? '&' : '?') + "oucid=" + oucid;
        return url;
    }

    public string RemoveOucidFromUrl(string url) {
        if (!url.Contains("oucid")) { return url; }

        int pos = url.IndexOf("oucid=", StringComparison.InvariantCulture);
        return pos >= 0 ? url[..(pos - 1)] : url;
    }

    public async Task<string> GenerateOucidAsync(IErrorsAndInfos errorsAndInfos) {
        string oucid = Guid.NewGuid().ToString();
        oucid = oucid[..oucid.IndexOf('-')];

        var oucidResponses = new OucidResponses { new() };

        await WriteOucidAsync(oucid, oucidResponses, errorsAndInfos);

        return oucid;
    }

    protected string OucidFileName(string oucid) {
        return OucidLogFolder.FullName + $"\\OUCID-{oucid}.json";
    }
}