using System.Xml.Serialization;

namespace LabWorkTools.Modules.MyXSD.Models;
public class Employee
{
    [XmlAttribute("employeeId")]
    public string EmployeeId { get; set; }

    [XmlElement("FirstName")]
    public string FirstName { get; set; }

    [XmlElement("LastName")]
    public string LastName { get; set; }

    [XmlElement("Department")]
    public string Department { get; set; }

    [XmlElement("Salary")]
    public decimal Salary { get; set; }

    [XmlElement("HireDate")]
    public DateTime HireDate { get; set; }
}
