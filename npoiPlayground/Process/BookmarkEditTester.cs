using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;
using npoiPlayground.Write;

namespace npoiPlayground.Process;

public class BookmarkEditTester
{
    public static void ExecuteTest(string docPath)
    {
        if (!File.Exists(docPath))
        {
            Console.WriteLine($"範例文件 {docPath} 不存在，請先準備一個或使用 WordDocumentGenerator 生成。");
            // 為方便測試，這裡創建一個簡單的
            CreateSampleDocForEditing(docPath);
        }


        WordBookmarkEditor editor = new WordBookmarkEditor(docPath);

        Console.WriteLine("--- 目前文件中的書籤 ---");
        List<string> initialBookmarks = editor.GetAllBookmarkNames();
        if (initialBookmarks.Any())
        {
            initialBookmarks.ForEach(Console.WriteLine);
        }
        else
        {
            Console.WriteLine("文件中沒有書籤。");
        }

        Console.WriteLine("\n--- 測試重新命名書籤 ---");
        if (initialBookmarks.Contains("CastleLocation"))
        {
            editor.RenameBookmark("CastleLocation", "OldCastleLocation");
        }
        else if (initialBookmarks.Any())
        {
            editor.RenameBookmark(initialBookmarks.First(), initialBookmarks.First() + "_Renamed");
        }


        Console.WriteLine("\n--- 測試刪除書籤 ---");
        if (editor.GetAllBookmarkNames().Contains("DragonLairEntrance")) // 假設 "DragonLairEntrance" 存在
        {
            editor.DeleteBookmark("DragonLairEntrance");
        }
        else if (editor.GetAllBookmarkNames().Count > 1)
        { // 刪除第二個，如果存在
            editor.DeleteBookmark(editor.GetAllBookmarkNames()[1]);
        }


        Console.WriteLine("\n--- 測試添加新書籤 ---");
        // 獲取文件中的第一個段落來添加書籤 (如果存在)
        var firstParagraph = editor._document.Paragraphs.FirstOrDefault(); // 這裡直接訪問了私有成員，僅為範例簡化
                                                                           // 更好的做法是 WordBookmarkEditor 提供獲取段落的方法
        if (firstParagraph != null)
        {
            editor.AddPointBookmark(firstParagraph, "NewPointBookmarkAtEnd");
            editor.AddBookmarkWithNewText(firstParagraph, "NewTextBookmark", "這是新的書籤文字!", run =>
            {
                run.IsBold = true;
                run.SetColor("FF0000"); // 紅色
            });
        }
        else
        {
            XWPFParagraph newP = editor._document.CreateParagraph(); // 如果沒有段落，創建一個
            editor.AddPointBookmark(newP, "NewPointBookmarkInNewPara");
        }


        Console.WriteLine("\n--- 修改後的書籤列表 ---");
        editor.GetAllBookmarkNames().ForEach(Console.WriteLine);

        // 儲存變更到新文件
        editor.Save("MyExistingDocument_Edited.docx");
        // 或者儲存回原文件: editor.Save(); (請小心!)

        Console.WriteLine("\n操作完成。");
    }

    // 輔助方法，用於創建一個簡單的 Word 文件以供編輯器測試
    private static void CreateSampleDocForEditing(string filePath)
    {
        XWPFDocument doc = new XWPFDocument();
        XWPFParagraph p1 = doc.CreateParagraph();
        XWPFRun r1 = p1.CreateRun();
        r1.SetText("這是第一段，包含一個");

        CT_Bookmark bm1Start = p1.GetCTP().AddNewBookmarkStart();
        bm1Start.id = "0";
        bm1Start.name = "SampleBookmark1";
        XWPFRun r1_bm = p1.CreateRun();
        r1_bm.SetText("書籤文字");
        CT_MarkupRange bm1End = p1.GetCTP().AddNewBookmarkEnd();
        bm1End.id = "0";
        r1.AppendText("。"); // AppendText 可能有問題，直接用 CreateRun().SetText()
        p1.CreateRun().SetText("。");


        XWPFParagraph p2 = doc.CreateParagraph();
        p2.CreateRun().SetText("這是第二段。");
        CT_Bookmark bm2Start = p2.GetCTP().AddNewBookmarkStart();
        bm2Start.id = "1";
        bm2Start.name = "PointBookmark";
        CT_MarkupRange bm2End = p2.GetCTP().AddNewBookmarkEnd();
        bm2End.id = "1";
        p2.CreateRun().SetText(" 一個點書籤在此之前。");

        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            doc.Write(fs);
        }
        Console.WriteLine($"已創建範例文件: {filePath}");
    }
}
