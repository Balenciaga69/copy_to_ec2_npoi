# XML 與 XSD 處理工具集(示範用)

此專案包含一系列用於處理 XML 檔案和 XSD (XML Schema Definition) 的 C# 工具。它可以協助開發者在 C# 環境中生成 XML、從類別生成 XSD、從 XML 推斷 XSD 以及驗證 XML 是否符合指定的 XSD。

## 主要功能
-   **從 C# 類別生成 XSD**：能夠將 C# 的資料模型類別轉換為對應的 XSD 結構描述檔案。例如，`XsdGenerator.GenerateXsdFromClass<Company>("company.xsd")` 會根據 `Company` 類別產生 "company.xsd" 
-   **生成範例 XML 檔案**：根據定義的 C# 物件模型，序列化並產生 XML 檔案 
    -   `CreateSampleXml()` 方法會建立一個名為 "sample.xml" 的檔案，其內容來自一個預設的 `Company` 物件 
    -   `GenerateSampleXml()` 方法會建立一個名為 "generated_company.xml" 的檔案，並使用自訂的縮排格式 
-   **從 XML 推斷 XSD**：能夠讀取現有的 XML 檔案，並從其結構推斷出對應的 XSD 結構描述檔案。例如，`XsdInferenceGenerator.InferXsdFromXml("sample.xml", "inferred.xsd")` 會分析 "sample.xml" 並產生 "inferred.xsd" [cite: 2, 28]。
-   **XML 驗證**：提供 XML 檔案針對 XSD 結構描述的驗證功能。`XmlValidator.ValidateXmlAgainstXsd` 方法可以用來檢查 XML 檔案是否符合指定的 XSD 規範 

## 專案結構與核心元件
-   **`MyXsdProcessor.cs`**: 包含主要的執行邏輯 (`Main` 方法)，演示如何使用各項功能，例如生成 XSD、建立 XML，以及從 XML 推斷 XSD 
-   **`Generators/`**
    -   `XsdGenerator.cs`:  負責從 C# 類別生成 XSD 檔案 
    -   `XsdInferenceGenerator.cs`:  負責從 XML 檔案推斷並生成 XSD 檔案 
-   **`Models/`**: 定義了用於 XML 序列化/反序列化的資料模型。
    -   `Address.cs`:  定義地址的結構，包含街道、城市、郵遞區號和國家 
    -   `Company.cs`: 定義公司的結構，包含 ID、名稱、地址、員工列表和成立日期 
    -   `Employee.cs` (檔案名稱為 `Empolyee.cs`): 定義員工的結構，包含員工 ID、名字、姓氏、部門、薪水和雇用日期 
-   **`XmlValidator.cs`**: 提供將 XML 檔案與 XSD 結構描述進行比較的驗證功能 

## 使用範例 (主要流程)
在 `MyXsdProcessor` 的 `Main` 方法中
1.  首先，從 `Company` 類別生成 `company.xsd` 檔案 
2.  接著，呼叫 `CreateSampleXml()` 方法來生成一個名為 `sample.xml` 的範例檔案 
3.  然後，使用 `sample.xml` 來推斷並生成 `inferred.xsd` 檔案 
4.  最後，呼叫 `GenerateSampleXml()` 方法來生成另一個範例 XML 檔案 `generated_company.xml` 
此工具集旨在簡化與 XML 和 XSD 相關的常見開發任務。