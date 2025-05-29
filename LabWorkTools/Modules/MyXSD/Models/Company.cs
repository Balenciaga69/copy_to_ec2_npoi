using NPOI.SS.Formula.Functions;
using System.Xml.Serialization;

namespace LabWorkTools.Modules.MyXSD.Models;

[XmlRoot("Company")]
public class Company
{
    [XmlAttribute("id")]
    public string Id { get; set; }

    [XmlElement("Name")]
    public string Name { get; set; }

    [XmlElement("Address")]
    public Address Address { get; set; }

    [XmlArray("Employees")]
    [XmlArrayItem("Employee")]
    public List<Employee> Employees { get; set; } = new List<Employee>();

    [XmlElement("EstablishedDate")]
    public DateTime EstablishedDate { get; set; }
}