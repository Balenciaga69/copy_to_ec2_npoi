namespace npoiPlayground.Write;
using NPOI.XWPF.UserModel;
using NPOI.OpenXmlFormats.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml; // Required for XmlNode exceptions from NPOI
using System.Collections;

public class WordBookmarkEditor
{
    public XWPFDocument _document;
    public string _filePath;

    /// <summary>
    /// 初始化 WordBookmarkEditor 並載入指定的 Word 文件。
    /// </summary>
    /// <param name="filePath">要編輯的 Word 文件的路徑。</param>
    public WordBookmarkEditor(string filePath)
    {
        _filePath = filePath;
        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            _document = new XWPFDocument(fileStream);
        }
    }

    /// <summary>
    /// 將修改儲存到指定的路徑。如果未提供路徑，則儲存回原始檔案。
    /// </summary>
    /// <param name="newFilePath">可選。儲存修改後文件的新路徑。</param>
    public void Save(string? newFilePath = null)
    {
        string path = newFilePath ?? _filePath;
        using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            _document.Write(fs);
        }
        Console.WriteLine($"文件已儲存至: {path}");
    }

    /// <summary>
    /// 輔助方法：遍歷文件中的所有段落 (包括表格內的段落)。
    /// </summary>
    private void IterateParagraphs(Action<XWPFParagraph> paragraphAction)
    {
        // 主文件內容中的段落
        foreach (var p in _document.Paragraphs)
        {
            paragraphAction(p);
        }

        // 表格中的段落
        foreach (var table in _document.Tables)
        {
            foreach (var row in table.Rows)
            {
                foreach (var cell in row.GetTableCells())
                {
                    foreach (var p in cell.Paragraphs)
                    {
                        paragraphAction(p);
                    }
                }
            }
        }
        // 注意：為了完整性，還應考慮頁首、頁尾、註腳等其他位置的段落。
        // 此處為簡化範例，暫未包含。
    }

    /// <summary>
    /// 輔助方法：根據書籤名稱尋找書籤的開始和結束標記及其所在段落。
    /// </summary>
    private (CT_Bookmark? BmStart, CT_MarkupRange? BmEnd, XWPFParagraph? StartPara, XWPFParagraph? EndPara) FindBookmarkTags(string bookmarkName)
    {
        CT_Bookmark? foundBmStart = null;
        XWPFParagraph? startPara = null;
        string? targetId = null;

        // 1. 找到書籤的開始標記 (CT_Bookmark)
        IterateParagraphs(p =>
        {
            if (foundBmStart != null) return; // 已找到，停止搜索

            var ctp = p.GetCTP();
            if (ctp == null) return;

            foreach (var item in ctp.Items)
            {
                if (item is CT_Bookmark bm && bm.name == bookmarkName)
                {
                    foundBmStart = bm;
                    startPara = p;
                    targetId = bm.id;
                    return; // 找到開始標記
                }
            }
        });

        if (foundBmStart == null || targetId == null || startPara == null)
        {
            return (null, null, null, null); // 未找到開始標記
        }

        // 2. 根據 ID 找到對應的書籤結束標記 (CT_MarkupRange)
        //    確保結束標記在開始標記之後。
        CT_MarkupRange? foundBmEnd = null;
        XWPFParagraph? endPara = null;
        bool SUTFound = false; // SUT (Start Under Test) - 是否已遇到開始標記

        IterateParagraphs(p =>
        {
            if (foundBmEnd != null) return; // 已找到結束標記，停止搜索

            var ctp = p.GetCTP();
            if (ctp == null) return;

            for (int i = 0; i < ctp.Items.Count; i++)
            {
                object currentItem = ctp.Items[i];

                // 檢查是否到達了我們正在尋找的書籤的開始標記
                if (ReferenceEquals(p, startPara) && ReferenceEquals(currentItem, foundBmStart))
                {
                    SUTFound = true;
                    continue; // 從開始標記之後的項目開始尋找結束標記
                }

                // 如果已經過了開始標記 (無論是在同段落還是後續段落)
                if (SUTFound)
                {
                    if (currentItem is CT_MarkupRange bmEnd && bmEnd.id == targetId)
                    {
                        foundBmEnd = bmEnd;
                        endPara = p;
                        return; // 找到匹配的結束標記
                    }
                }
            }
            // 如果當前段落不是開始標記所在段落，且開始標記已找到，則整個段落都在開始標記之後
            if (!ReferenceEquals(p, startPara) && foundBmStart != null) SUTFound = true;

        });

        return (foundBmStart, foundBmEnd, startPara, endPara);
    }


    /// <summary>
    /// 輔助方法：獲取下一個可用的書籤 ID。
    /// 書籤 ID 必須是唯一的非負整數。
    /// </summary>
    private string GetNextAvailableBookmarkId()
    {
        int maxId = -1; // 從-1開始，這樣第一個ID將是0 (如果文件中沒有書籤) 或 maxId+1
        IterateParagraphs(p =>
        {
            var ctp = p.GetCTP();
            if (ctp == null) return;

            foreach (var item in ctp.Items)
            {
                string? currentIdStr = null;
                if (item is CT_Bookmark bmStart) currentIdStr = bmStart.id;
                // CT_MarkupRange 也有 id，但 CT_Bookmark 的 id 才是定義書籤的唯一性關鍵
                // else if (item is CT_MarkupRange bmEnd) currentIdStr = bmEnd.id; 

                if (currentIdStr != null && int.TryParse(currentIdStr, out int currentIdVal))
                {
                    if (currentIdVal > maxId) maxId = currentIdVal;
                }
            }
        });
        return (maxId + 1).ToString();
    }

    /// <summary>
    /// 獲取文件中所有書籤的名稱列表。
    /// </summary>
    public List<string> GetAllBookmarkNames()
    {
        var names = new List<string>();
        IterateParagraphs(p =>
        {
            var ctp = p.GetCTP();
            if (ctp == null) return;

            foreach (var item in ctp.Items)
            {
                if (item is CT_Bookmark bmStart && !string.IsNullOrEmpty(bmStart.name))
                {
                    if (!names.Contains(bmStart.name)) // 避免重複 (雖然書籤名應該唯一)
                    {
                        names.Add(bmStart.name);
                    }
                }
            }
        });
        return names;
    }

    /// <summary>
    /// 重新命名一個現有的書籤。
    /// </summary>
    /// <param name="oldName">書籤的原始名稱。</param>
    /// <param name="newName">書籤的新名稱。</param>
    /// <returns>如果成功重新命名則返回 true，否則返回 false。</returns>
    public bool RenameBookmark(string oldName, string newName)
    {
        if (string.IsNullOrEmpty(oldName) || string.IsNullOrEmpty(newName) || oldName == newName)
        {
            Console.WriteLine("舊名稱或新名稱無效。");
            return false;
        }

        // 檢查新名稱是否已存在 (可選，但建議)
        if (GetAllBookmarkNames().Contains(newName))
        {
            Console.WriteLine($"書籤名稱 '{newName}' 已存在。");
            return false;
        }

        var (bmStart, _, _, _) = FindBookmarkTags(oldName);

        if (bmStart != null)
        {
            bmStart.name = newName;
            Console.WriteLine($"書籤 '{oldName}' 已成功重新命名為 '{newName}'。");
            return true;
        }
        else
        {
            Console.WriteLine($"未找到名為 '{oldName}' 的書籤。");
            return false;
        }
    }
    /// <summary>
    /// 輔助方法：嘗試從 IList 中移除指定的物件。
    /// </summary>
    /// <param name="list">要從中移除物件的 IList。</param>
    /// <param name="itemToRemove">要移除的物件。</param>
    /// <returns>如果物件成功移除 (或 изначально 不存在於列表中) 則返回 true，否則返回 false。</returns>
    private bool TryRemoveItemFromList(IList list, object itemToRemove)
    {
        if (list == null || itemToRemove == null)
        {
            return false; // 無法操作
        }

        list.Remove(itemToRemove);
        return !list.Contains(itemToRemove); // 確認它確實被移除了
    }

    /// <summary>
    /// 刪除一個書籤 (僅刪除書籤標記，保留其包圍的文字)。
    /// </summary>
    /// <param name="bookmarkName">要刪除的書籤的名稱。</param>
    /// <returns>如果成功刪除則返回 true，否則返回 false。</returns>
    public bool DeleteBookmark(string bookmarkName)
    {
        var (bmStart, bmEnd, startPara, endPara) = FindBookmarkTags(bookmarkName);

        if (bmStart == null || startPara == null)
        {
            Console.WriteLine($"未找到名為 '{bookmarkName}' 的書籤開始標記。");
            return false;
        }

        bool startRemoved = false;
        var startCtp = startPara.GetCTP();

        if (startCtp?.Items != null)
        {
            try
            {
                startRemoved = TryRemoveItemFromList(startCtp.Items, bmStart);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"移除書籤開始標記 '{bookmarkName}' 時出錯: {ex.Message}");
                // startRemoved 保持 false
            }
        }
        else
        {
            string? paraTextPreview = startPara.Text?.Length > 20 ? startPara.Text.Substring(0, 20) + "..." : startPara.Text;
            Console.WriteLine($"段落 '{paraTextPreview}' 的 CTP 或 Items 為 null，無法移除書籤開始標記。");
        }

        if (!startRemoved)
        {
            Console.WriteLine($"未能從段落中移除書籤 '{bookmarkName}' 的開始標記。");
            return false; // 如果開始標記移除失敗，則不繼續
        }

        // 只有在成功移除開始標記後才嘗試移除結束標記
        if (bmEnd != null && endPara != null)
        {
            bool endRemoved = false;
            var endCtp = endPara.GetCTP();
            if (endCtp?.Items != null)
            {
                try
                {
                    endRemoved = TryRemoveItemFromList(endCtp.Items, bmEnd);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"移除書籤結束標記 (ID: {bmStart.id}) 時出錯: {ex.Message}");
                }

                if (!endRemoved)
                {
                    Console.WriteLine($"未能從段落中移除書籤 '{bookmarkName}' (ID: {bmStart.id}) 的結束標記。但開始標記可能已移除。");
                }
            }
            else
            {
                string? paraTextPreview = endPara.Text?.Length > 20 ? endPara.Text.Substring(0, 20) + "..." : endPara.Text;
                Console.WriteLine($"段落 '{paraTextPreview}' 的 CTP 或 Items 為 null，無法移除書籤結束標記。");
            }
        }
        else if (bmStart != null) // bmStart 被找到且已移除，但 bmEnd 在 FindBookmarkTags 中未找到
        {
            Console.WriteLine($"警告：書籤 '{bookmarkName}' (ID: {bmStart.id}) 的結束標記最初未找到或其段落訊息不完整。開始標記已成功移除。");
        }

        Console.WriteLine($"書籤 '{bookmarkName}' 的標記已嘗試移除。");
        return true; // 因為主要目標 (開始標記) 已移除，所以整體操作視為已執行。
    }

    /// <summary>
    /// 在指定段落的末尾添加一個點書籤。
    /// </summary>
    /// <param name="targetParagraph">要在其中添加書籤的段落。</param>
    /// <param name="bookmarkName">新書籤的名稱。</param>
    /// <returns>如果成功添加則返回 true。</returns>
    public bool AddPointBookmark(XWPFParagraph targetParagraph, string bookmarkName)
    {
        if (targetParagraph == null || string.IsNullOrEmpty(bookmarkName)) return false;

        string newId = GetNextAvailableBookmarkId();
        var ctp = targetParagraph.GetCTP();

        CT_Bookmark bmStart = ctp.AddNewBookmarkStart(); // 添加到末尾
        bmStart.id = newId;
        bmStart.name = bookmarkName;

        CT_MarkupRange bmEnd = ctp.AddNewBookmarkEnd(); // 添加到末尾
        bmEnd.id = newId;

        Console.WriteLine($"點書籤 '{bookmarkName}' 已添加到段落末尾。");
        return true;
    }

    /// <summary>
    /// 在指定段落的末尾添加包含新文字的書籤。
    /// </summary>
    /// <param name="targetParagraph">要在其中添加書籤和文字的段落。</param>
    /// <param name="bookmarkName">新書籤的名稱。</param>
    /// <param name="text">要包含在書籤內的文字。</param>
    /// <param name="formatRun">可選。用於格式化新文字的 Action。</param>
    /// <returns>如果成功添加則返回 true。</returns>
    public bool AddBookmarkWithNewText(XWPFParagraph targetParagraph, string bookmarkName, string text, Action<XWPFRun>? formatRun = null)
    {
        if (targetParagraph == null || string.IsNullOrEmpty(bookmarkName) || string.IsNullOrEmpty(text)) return false;

        string newId = GetNextAvailableBookmarkId();
        var ctp = targetParagraph.GetCTP();

        CT_Bookmark bmStart = ctp.AddNewBookmarkStart();
        bmStart.id = newId;
        bmStart.name = bookmarkName;

        XWPFRun run = targetParagraph.CreateRun(); // CreateRun 會將 Run 添加到段落末尾 (在 bmStart 之後)
        run.SetText(text);
        formatRun?.Invoke(run);

        CT_MarkupRange bmEnd = ctp.AddNewBookmarkEnd();
        bmEnd.id = newId;

        Console.WriteLine($"書籤 '{bookmarkName}' 及文字 '{text}' 已添加到段落末尾。");
        return true;
    }

    // --- 尚未實現的更複雜功能 ---
    // public bool UpdateBookmarkText(string bookmarkName, string newText) { /* ... */ }
    // public bool AddBookmarkAroundExistingText(...) { /* ... */ }
}