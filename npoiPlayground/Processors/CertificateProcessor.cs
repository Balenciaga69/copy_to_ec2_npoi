using npoiPlayground.Utils;

namespace npoiPlayground.Processors;

internal static class CertificateProcessor
{
    public static void Main()
    {
        string templateFileName = "GraduationCertificateTemplate.docx";
        string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "Certificates");
        Directory.CreateDirectory(outputDir);

        string templateFilePath = Path.Combine(outputDir, templateFileName);
        string filledCert1Path = Path.Combine(outputDir, "Student_A_Cert.docx");
        string filledCert2Path = Path.Combine(outputDir, "Student_B_Cert.docx");

        try
        {
            CertificateUtils.CreateCertificateTemplate(templateFilePath);
            CertificateUtils.FillCertificate("113", "Gucci", "謝古馳", templateFilePath, filledCert1Path);
            CertificateUtils.FillCertificate("113", "Dior", "張迪奧", templateFilePath, filledCert2Path);
            //var yes= CertificateUtils.CompareFilesBinary(filledCert2Path, filledCert1Path);
            //Console.WriteLine(yes);
        }
        catch (FileNotFoundException fnfEx)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {fnfEx.Message}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            Console.ResetColor();
        }
    }
}

