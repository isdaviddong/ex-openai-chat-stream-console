# ChatCompletionStreamResponse

本專案是一個簡易 C# 主控台程式，透過 OpenAI API 以串流方式取得 Chat Completion 回覆。

## 功能簡介

- 讓使用者輸入問題，並即時串流顯示 GPT-4o-mini 的回答。
- 需先設定 OpenAI API 金鑰於環境變數。

## 使用方式

1. **設定環境變數**  
   請將你的 OpenAI API 金鑰設定到 `OPENAI_API_KEY` 環境變數。
   - Windows 指令範例：  
     ```
     setx OPENAI_API_KEY "你的API金鑰"
     ```

2. **編譯與執行**  
   - 使用 .NET 6 或以上版本。
   - 編譯：
     ```
     dotnet build
     ```
   - 執行：
     ```
     dotnet run
     ```

3. **互動流程**  
   - 執行程式後，輸入你的問題，程式會即時顯示 AI 回覆。

## 注意事項

- 請確保已安裝 .NET 6 或更新版本。
- 若未設定 API 金鑰，程式將提示錯誤並結束。

## 檔案說明

- `Program.cs`：主程式，負責與 OpenAI API 串接並顯示回覆。
- `readme.md`：本說明文件。

---
