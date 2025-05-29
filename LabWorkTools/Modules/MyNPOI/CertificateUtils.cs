using NPOI.XWPF.UserModel;

namespace LabWorkTools.Modules.MyNPOI;
public static class CertificateUtils
{
    public static void CreateCertificateTemplate(string filePath)
    {
        using (XWPFDocument doc = new XWPFDocument())
        {
            CreateAndSetRunText(doc, "[年] 年 [班] 班 [姓名] 同學 ... 畢業快樂");
            CreateAndSetRunText(doc, "<[年月日]>測試捕捉 <[年]> 年 <[月]> 月");
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                doc.Write(fs);
            }
        }
    }

    private static void CreateAndSetRunText(XWPFDocument doc, string text)
    {
        XWPFParagraph p = doc.CreateParagraph();
        XWPFRun r = p.CreateRun();
        r.SetText(text);
    }

    public static void FillCertificate(string year, string className, string studentName, string templateFilePath, string outputFilePath)
    {
        if (!File.Exists(templateFilePath))
        {
            throw new FileNotFoundException("Template file not found.", templateFilePath);
        }

        using (FileStream fsTemplate = new FileStream(templateFilePath, FileMode.Open, FileAccess.Read))
        {
            using (XWPFDocument doc = new XWPFDocument(fsTemplate))
            {
                foreach (var para in doc.Paragraphs)
                {
                    ReplaceTextInParagraph(para, "[年]", year);
                    ReplaceTextInParagraph(para, "[班]", className);
                    ReplaceTextInParagraph(para, "[姓名]", studentName);
                }
                using (FileStream fsOutput = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                {
                    doc.Write(fsOutput);
                }
            }
        }
    }

    private static void ReplaceTextInParagraph(XWPFParagraph para, string findText, string replaceText)
    {
        for (int i = 0; i < para.Runs.Count; i++)
        {
            XWPFRun run = para.Runs[i];
            string text = run.GetText(0);
            if (text != null && text.Contains(findText))
            {
                text = text.Replace(findText, replaceText);
                run.SetText(text, 0);
            }
        }
    }

    public static bool CompareFilesBinary(string filePathA, string filePathB)
    {
        if (!File.Exists(filePathA))
        {
            throw new FileNotFoundException("File A not found.", filePathA);
        }
        if (!File.Exists(filePathB))
        {
            throw new FileNotFoundException("File B not found.", filePathB);
        }

        byte[] fileABytes = File.ReadAllBytes(filePathA);
        byte[] fileBBytes = File.ReadAllBytes(filePathB);

        if (fileABytes.Length != fileBBytes.Length)
        {
            return false;
        }

        return fileABytes.SequenceEqual(fileBBytes);
    }
}
