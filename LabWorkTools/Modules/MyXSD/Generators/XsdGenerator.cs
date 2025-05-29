using System.Xml.Schema;
using System.Xml.Serialization;

namespace LabWorkTools.Modules.MyXSD.Generators;

public class XsdGenerator
{
    public static void GenerateXsdFromClass<T>(string outputPath)
    {
        var schemas = new XmlSchemas();
        var exporter = new XmlSchemaExporter(schemas);
        var mapping = new XmlReflectionImporter().ImportTypeMapping(typeof(T));
        exporter.ExportTypeMapping(mapping);

        using var writer = new StreamWriter(outputPath);
        foreach (XmlSchema schema in schemas)
        {
            schema.Write(writer);
        }
    }
}
