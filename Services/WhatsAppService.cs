using System.Text;
using System.Text.Json;

namespace POC_ChatBotAgendar.Services;

public static class WhatsAppService
{
    // Reemplaza estos valores con tus datos
    private static readonly string Token = "EAANdb8fxt1QBO8C9qahRoYXZCBNnZAugzarmqE1aQywKaCet9isPuZAubTRt551Qu3hO0aV5vOnMuh4EV4cJDjHNwE4ZCSZAfgDmrQrnNTa7jJ68VySvkvcADA5H7tnyl8rq6Aguar5RHBv0ceDZAVGfvOB59YQPNZCcb1sW9vXZAZBeJ6ZAhZCca9DRRFGgpZADNxyAZAwZDZD";
    private static readonly string IdCelphone = "511685075369811";
    private static readonly HttpClient _client = new HttpClient();

    public static async Task SendTextMessage(string celphone, string text)
    {
        var payload = new
        {
            messaging_product = "whatsapp",
            recipient_type = "individual",
            to = celphone,
            type = "text",
            text = new { body = text }
        };

        Console.WriteLine(celphone);

        string json = JsonSerializer.Serialize(payload);
        var request = new HttpRequestMessage(HttpMethod.Post, $"https://graph.facebook.com/v21.0/{IdCelphone}/messages");
        request.Headers.Add("Authorization", "Bearer " + Token);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        await _client.SendAsync(request);
    }
}