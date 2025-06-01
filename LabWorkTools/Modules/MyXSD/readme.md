這份文件旨在協助您了解如何使用 XML 相關工具，特別是 `XsdGenerator`、`XsdInferenceGenerator` 和 `XmlValidator`，以及如何在 XML、XSD 和 C# 類別之間進行轉換。

### 主要工具簡介

以下是我們將重點關注的三個主要工具類別及其功能：

- **`XsdGenerator`**: 此工具主要用於從 model.cs -> model.xsd
    
- **`XsdInferenceGenerator`**: 此工具的功能是從 modelExample.xml -> model.xsd
    
- **`XmlValidator`**: 此工具用於確認 xml 是否符合 xsd 規範

---

### xml.exe 使用

在 VS 2022 使用 開發人員 PowerShell 輸入以下指令
- `xsd ./sample.xml`：從xml生成xsd檔案 
- `xsd ./sample.xsd /classes` ：從xsd生成cs檔案
  

#### xml -> cs
先執行 `xsd ./sample.xml` 接著執行 `xsd ./sample.xsd /classes`

#### xml -> xsd
`xsd ./sample.xml`