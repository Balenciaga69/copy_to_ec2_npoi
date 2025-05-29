using System.Xml.Serialization;

namespace LabWorkTools.Modules.MyXSD.Models;

public class Address
{
    [XmlElement("Street")]
    public string Street { get; set; }

    [XmlElement("City")]
    public string City { get; set; }

    [XmlElement("PostalCode")]
    public string PostalCode { get; set; }

    [XmlElement("Country")]
    public string Country { get; set; }
}