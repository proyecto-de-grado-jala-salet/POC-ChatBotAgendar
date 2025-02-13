using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using Services.Interfaces;

namespace Services;

public class WhatsAppService : IWhatsAppService
{
    private readonly HttpClient _httpClient;
    private readonly string _token;
    private readonly string _idCelphone;

    public WhatsAppService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _token = configuration["WhatsApp:Token"];
        _idCelphone = configuration["WhatsApp:IdCelphone"];
    }

    public async Task SendTextMessageAsync(string phone, string message)
    {
        var url = $"https://graph.facebook.com/v21.0/{_idCelphone}/messages";
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

        var payload = new
        {
            messaging_product = "whatsapp",
            recipient_type = "individual",
            to = phone,
            type = "text",
            text = new { body = message }
        };

        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}