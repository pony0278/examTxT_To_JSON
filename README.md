# 考題文本轉 JSON 工具

這是一個用於將考題文本格式轉換為 JSON 格式的工具。

## 功能特點

- 支援解析標準格式的考題文本
- 自動識別題目編號、答案、選項和參考出處
- 輸出格式化的 JSON 文件
- 支援 UTF-8 編碼，完整支援中文

## 使用方法

1. 運行程式
2. 根據提示輸入題目文本文件路徑
3. 指定 JSON 輸出文件路徑
4. 程式將自動處理並生成 JSON 文件

## 系統需求

- .NET 8.0 或更高版本
- Windows/macOS/Linux

## 安裝說明

1. 克隆專案
```bash
git clone [你的專案URL]
```

2. 進入專案目錄
```bash
cd TXTTOJSON
```

3. 建置專案
```bash
dotnet build
```

4. 運行程式
```bash
dotnet run
```

## 輸入文本格式要求
輸入的文本文件需要符合以下格式：
```text
(A) 1.題目內容 (A)選項A (B)選項B (C)選項C (D)選項D (出處：參考資料)
```

## 輸出 JSON 格式
```json
{
  "QuestionNumber": 1,
  "CorrectAnswer": "A",
  "QuestionText": "題目內容",
  "Options": {
    "A": "選項A",
    "B": "選項B",
    "C": "選項C",
    "D": "選項D"
  },
  "Reference": "參考資料"
}
```