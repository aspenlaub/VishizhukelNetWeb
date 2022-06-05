using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Helpers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Components {
    public class LogicalUrlRepository : ILogicalUrlRepository {
        private readonly ISecretRepository _SecretRepository;
        private Dictionary<string, string> _NameToUrl;

        public LogicalUrlRepository(ISecretRepository secretRepository) {
            _SecretRepository = secretRepository;
            _NameToUrl = null;
        }

        public async Task<string> GetUrlAsync(string name, IErrorsAndInfos errorsAndInfos) {
            if (name == Urls.AboutBlank) { return name; }

            if (_NameToUrl == null) {
                var logicalUrlsSecret = new LogicalUrlsSecret();
                var logicalUrls = await _SecretRepository.GetAsync(logicalUrlsSecret, errorsAndInfos);
                if (errorsAndInfos.AnyErrors()) { return ""; }

                _NameToUrl = logicalUrls.ToDictionary(x => x.Name, x => x.Url);
            }

            if (_NameToUrl.ContainsKey(name)) {
                return _NameToUrl[name];
            }

            errorsAndInfos.Errors.Add($"Logical URL '{name}' not defined");
            return "";

        }
    }
}
