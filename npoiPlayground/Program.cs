using npoiPlayground.Process;
using npoiPlayground.Write;

var fileName = "MyExistingDocument.docx";
WordDocumentGenerator.GenerateStoryWithBookmarks(fileName);
BookmarkEditTester.ExecuteTest(fileName);