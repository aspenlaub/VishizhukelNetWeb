using System.Collections.Generic;
using System.Xml.Serialization;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities; 

[XmlRoot(nameof(LogicalUrls), Namespace = "http://www.aspenlaub.net")]
public class LogicalUrls : List<LogicalUrl>, ISecretResult<LogicalUrls> {
    public LogicalUrls Clone() {
        var clone = new LogicalUrls();
        clone.AddRange(this);
        return clone;
    }
}