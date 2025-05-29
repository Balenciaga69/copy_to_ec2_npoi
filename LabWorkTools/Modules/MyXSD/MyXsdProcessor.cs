using LabWorkTools.Modules.MyXSD.Generators;
using LabWorkTools.Modules.MyXSD.Models;
using System.Xml;
using System.Xml.Serialization;

namespace LabWorkTools.Modules.MyXSD;

internal class MyXsdProcessor
{
    public static void Main()
    {
        // 先執行 CreateSampleXml 才能生成 sample.xml
        // 有 sample.xml 才能 產生 inferred.xsd
        XsdGenerator.GenerateXsdFromClass<Company>("company.xsd");
        CreateSampleXml();

        XsdInferenceGenerator.InferXsdFromXml("sample.xml", "inferred.xsd");
        GenerateSampleXml();

        Console.WriteLine("XSD 和 XML 檔案已生成完成！");
    }

    private static void CreateSampleXml()
    {
        var company = new Company
        {
            Id = "COMP001",
            Name = "科技公司",
            EstablishedDate = new DateTime(2020, 1, 15),
            Address = new Address
            {
                Street = "台中市西屯區文心路100號",
                City = "台中市",
                PostalCode = "40701",
                Country = "台灣"
            },
            Employees = new List<Employee>
            {
                new Employee
                {
                    EmployeeId = "EMP001",
                    FirstName = "小明",
                    LastName = "王",
                    Department = "資訊部",
                    Salary = 50000,
                    HireDate = new DateTime(2022, 3, 1)
                },
                new Employee
                {
                    EmployeeId = "EMP002",
                    FirstName = "小華",
                    LastName = "李",
                    Department = "行銷部",
                    Salary = 45000,
                    HireDate = new DateTime(2023, 6, 15)
                }
            }
        };

        var xmlSerializer = new XmlSerializer(typeof(Company));
        using var writer = new StreamWriter("sample.xml");
        xmlSerializer.Serialize(writer, company);
    }

    private static void GenerateSampleXml()
    {
        var company = new Company
        {
            Id = "COMP002",
            Name = "軟體開發公司",
            EstablishedDate = DateTime.Now.AddYears(-5),
            Address = new Address
            {
                Street = "台北市信義區信義路200號",
                City = "台北市",
                PostalCode = "11049",
                Country = "台灣"
            },
            Employees = new List<Employee>
            {
                new Employee
                {
                    EmployeeId = "EMP003",
                    FirstName = "小美",
                    LastName = "陳",
                    Department = "開發部",
                    Salary = 60000,
                    HireDate = DateTime.Now.AddMonths(-18)
                }
            }
        };

        var xmlSerializer = new XmlSerializer(typeof(Company));
        using var writer = XmlWriter.Create("generated_company.xml", new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "  ",
            NewLineChars = "\n"
        });

        xmlSerializer.Serialize(writer, company);
    }
}
