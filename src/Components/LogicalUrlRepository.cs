using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Helpers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Components {
    public class LogicalUrlRepository : ILogicalUrlRepository {
        private readonly ISecretRepository SecretRepository;
        private Dictionary<string, string> NameToUrl;

        public LogicalUrlRepository(ISecretRepository secretRepository) {
            SecretRepository = secretRepository;
            NameToUrl = null;
        }

        public async Task<string> GetUrlAsync(string name, IErrorsAndInfos errorsAndInfos) {
            if (name == Urls.AboutBlank) { return name; }

            if (NameToUrl == null) {
                var logicalUrlsSecret = new LogicalUrlsSecret();
                var logicalUrls = await SecretRepository.GetAsync(logicalUrlsSecret, errorsAndInfos);
                if (errorsAndInfos.AnyErrors()) { return ""; }

                NameToUrl = logicalUrls.ToDictionary(x => x.Name, x => x.Url);
            }

            if (!NameToUrl.ContainsKey(name)) {
                errorsAndInfos.Errors.Add($"Logical URL '{name}' not defined");
                return "";
            }

            return NameToUrl[name];
        }
    }
}
