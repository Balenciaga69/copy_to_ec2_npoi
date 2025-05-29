using System.Xml;

namespace LabWorkTools.Modules.MyXSD;

internal class XmlValidator
{
    public static bool ValidateXmlAgainstXsd(string xmlFilePath, string xsdFilePath)
    {
        try
        {
            var settings = new XmlReaderSettings();
            settings.Schemas.Add(null, xsdFilePath);
            settings.ValidationType = ValidationType.Schema;

            bool isValid = true;
            settings.ValidationEventHandler += (sender, e) =>
            {
                Console.WriteLine($"驗證錯誤: {e.Message}");
                isValid = false;
            };

            using var reader = XmlReader.Create(xmlFilePath, settings);
            while (reader.Read()) { }

            return isValid;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"驗證過程發生錯誤: {ex.Message}");
            return false;
        }
    }
}
