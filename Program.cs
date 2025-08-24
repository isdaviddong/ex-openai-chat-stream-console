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

        string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("請先在環境變數設定 OPENAI_API_KEY！");
            return;
        }

        using var http = new HttpClient();
        using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        var body = new
        {
            model = "gpt-4o-mini",
            stream = true,
            messages = new object[]
            {
                new { role = "system", content = "你是一個友善的助教。" },
                new { role = "user",   content = question }
            }
        };

        string json = JsonSerializer.Serialize(body);
        req.Content = new StringContent(json, Encoding.UTF8, "application/json");

        using var resp = await http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
        resp.EnsureSuccessStatusCode();

        await using var stream = await resp.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        Console.WriteLine("\n回答：\n");

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (line is null || !line.StartsWith("data: ")) continue;

            var payload = line.Substring("data: ".Length);
            if (payload == "[DONE]") break;

            try
            {
                using var doc = JsonDocument.Parse(payload);
                var root = doc.RootElement;

                var choices = root.GetProperty("choices");
                var delta = choices[0].GetProperty("delta");
                if (delta.TryGetProperty("content", out var contentElem))
                {
                    Console.Write(contentElem.GetString());
                }
            }
            catch { }
        }

        Console.WriteLine("\n\n--- 完成 ---");
    }
}
