using System.Xml.Serialization;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Entities;

public class LogicalUrl {
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("url")]
    public string Url { get; set; }
}