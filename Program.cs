using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        Console.Write("請輸入你的問題：");
        string question = Console.ReadLine() ?? "";

        // 取得 API 金鑰
        string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("請先在環境變數設定 OPENAI_API_KEY！");
            return;
        }

        // 建立一個 HttpClient 實例，用於發送 HTTP 請求
        using var http = new HttpClient();
        using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
        var body = new
        {
            model = "gpt-4o-mini",
            // 啟用串流模式，讓回應可以逐步傳回
            stream = true,
            messages = new object[]
            {
            new { role = "system", content = "你是一個友善的助教。" },
            new { role = "user",   content = question }
            }
        };

        // 將請求主體序列化為 JSON 字串
        string json = JsonSerializer.Serialize(body);
        req.Content = new StringContent(json, Encoding.UTF8, "application/json");
        // 發送請求並等待回應
        using var resp = await http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
        resp.EnsureSuccessStatusCode();

        // 以串流方式取得回應的內容
        await using var stream = await resp.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        Console.WriteLine("\n回答：\n");
        // 逐行讀取回應內容
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            // 忽略空行和非數據行
            if (line is null || !line.StartsWith("data: ")) continue;
            // 取得有效的 JSON 資料，並且檢查是否為結束標記
            var payload = line.Substring("data: ".Length);
            if (payload == "[DONE]") break;

            try
            {
                // 解析 JSON 資料
                using var doc = JsonDocument.Parse(payload);
                var root = doc.RootElement;
                // 取得 choices 陣列
                var choices = root.GetProperty("choices");
                // 取得第一個選項的 delta 屬性
                var delta = choices[0].GetProperty("delta");
                // 取得內容
                if (delta.TryGetProperty("content", out var contentElem))
                {
                    // 取得內容
                    Console.Write(contentElem.GetString());
                }
            }
            catch
            {
                // 解析錯誤...略
            }
        }

        Console.WriteLine("\n\n--- 完成 ---");
    }
}
