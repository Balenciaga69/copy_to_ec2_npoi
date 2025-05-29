using System.Xml;
using System.Xml.Schema;

namespace LabWorkTools.Modules.MyXSD.Generators;

public class XsdInferenceGenerator
{
    public static void InferXsdFromXml(string xmlFilePath, string xsdOutputPath)
    {
        var schemaSet = new XmlSchemaSet();
        var schema = new XmlSchema();

        var inference = new XmlSchemaInference();
        var xmlReader = XmlReader.Create(xmlFilePath);
        var inferredSchemas = inference.InferSchema(xmlReader);

        using var writer = new FileStream(xsdOutputPath, FileMode.Create);
        foreach (XmlSchema inferredSchema in inferredSchemas.Schemas())
        {
            inferredSchema.Write(writer);
        }
    }
}