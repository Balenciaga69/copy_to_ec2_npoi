namespace npoiPlayground.Write;

using NPOI.OpenXmlFormats.Wordprocessing; // 用於底層 OOXML 元素，如 CT_Bookmark
using NPOI.XWPF.UserModel; // 用於 .docx 文件的主要命名空間
using System; // For Action<T>
using System.IO;

public class WordDocumentGenerator
{
    private static int bookmarkIdCounter = 0; // 用於生成唯一的書籤 ID

    /// <summary>
    /// 向指定的段落添加書籤。
    /// </summary>
    /// <param name="paragraph">要在其中添加書籤的段落。</param>
    /// <param name="bookmarkName">書籤的名稱 (在 Word 中顯示的名稱)。</param>
    /// <param name="textToBookmark">可選。要被書籤包圍的文字。如果為 null 或空，則創建一個點書籤。</param>
    /// <param name="formatRun">可選。一個 Action，用於格式化書籤內的文字 (如果 textToBookmark 非空)。</param>
    private static void AddBookmark(XWPFParagraph paragraph, string bookmarkName, string? textToBookmark = null, Action<XWPFRun>? formatRun = null)
    {
        bookmarkIdCounter++;
        string currentBookmarkId = bookmarkIdCounter.ToString();

        // 書籤開始
        CT_Bookmark bookmarkStart = paragraph.GetCTP().AddNewBookmarkStart();
        bookmarkStart.id = currentBookmarkId;
        bookmarkStart.name = bookmarkName;

        // 如果有指定書籤內的文字
        if (!string.IsNullOrEmpty(textToBookmark))
        {
            XWPFRun bookmarkedRun = paragraph.CreateRun();
            bookmarkedRun.SetText(textToBookmark);
            formatRun?.Invoke(bookmarkedRun); // 應用自訂格式
        }

        // 書籤結束
        CT_MarkupRange bookmarkEnd = paragraph.GetCTP().AddNewBookmarkEnd();
        bookmarkEnd.id = currentBookmarkId; // ID 必須與 bookmarkStart 匹配
    }

    public static void GenerateStoryWithBookmarks(string filePath)
    {
        // 1. 重置計數器 (如果此方法可能被多次呼叫且希望每次ID從1開始)
        //    或者，如果您希望ID在應用程式生命週期內持續增加，則移除此行。
        //    對於此範例，假設每次生成文件時ID都從新開始。
        bookmarkIdCounter = 0;

        // 2. 建立一個新的 Word 文件 (.docx)
        XWPFDocument doc = new XWPFDocument();

        // 加入故事標題
        XWPFParagraph titleParagraph = doc.CreateParagraph();
        titleParagraph.Alignment = ParagraphAlignment.CENTER;
        XWPFRun titleRun = titleParagraph.CreateRun();
        titleRun.SetText("一個漫長的故事");
        titleRun.IsBold = true;
        titleRun.FontSize = 16;
        titleRun.AddCarriageReturn(); // 換行

        // --- 故事第一部分與第一個書籤 ---
        XWPFParagraph p1 = doc.CreateParagraph();
        XWPFRun r1_1 = p1.CreateRun();
        r1_1.SetText("很久很久以前，在一個遙遠的國度，高聳的");

        // 使用輔助方法添加書籤 "CastleLocation" 並包圍文字 "城堡"
        AddBookmark(p1, "CastleLocation", "城堡", run => run.IsBold = true);

        XWPFRun r1_2 = p1.CreateRun();
        r1_2.SetText("矗立在青翠的山谷中。是國王和王后的家。他們有一個勇敢的王子。");
        p1.CreateRun().AddCarriageReturn(); // 段落後換行

        // --- 故事第二部分 ---
        XWPFParagraph p2 = doc.CreateParagraph();
        XWPFRun r2 = p2.CreateRun();
        r2.SetText("王子喜歡冒險，他聽說在遙遠的東方有一條惡龍，守護著無盡的寶藏。於是，王子決定踏上征途，去尋找那條惡龍。這段旅程充滿了未知與危險，但他毫不畏懼。");
        p2.CreateRun().AddCarriageReturn();

        // --- 故事第三部分與多個書籤 ---
        XWPFParagraph p3 = doc.CreateParagraph();
        XWPFRun r3_1 = p3.CreateRun();
        r3_1.SetText("經過了長途跋涉，王子終於來到了一座巨大的山洞前，洞口噴著硫磺的氣息。");

        // 使用輔助方法添加點書籤 "DragonLairEntrance"
        AddBookmark(p3, "DragonLairEntrance");

        XWPFRun r3_2 = p3.CreateRun();
        r3_2.SetText("他知道，");

        // 使用輔助方法添加書籤 "TreasureClue" 並包圍文字 "傳說中的寶藏"
        AddBookmark(p3, "TreasureClue", "傳說中的寶藏", run => run.IsItalic = true);

        XWPFRun r3_3 = p3.CreateRun();
        r3_3.SetText("就在裡面。他握緊了劍，深吸一口氣，走進了山洞...");
        p3.CreateRun().AddCarriageReturn();


        // 4. 將文件寫入檔案
        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            doc.Write(fs);
        }

        Console.WriteLine($"成功生成 Word 文件並包含書籤: {filePath}");
        Console.WriteLine($"共建立了 {bookmarkIdCounter} 個書籤。");
    }
}

// 如何呼叫:
// WordDocumentGenerator.GenerateStoryWithBookmarks("我的故事書籤版_Refactored.docx");