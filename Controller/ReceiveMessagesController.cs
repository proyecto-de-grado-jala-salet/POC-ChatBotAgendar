using Microsoft.AspNetCore.Mvc;
using Model;
using Services.Interfaces;

namespace Controller;

[ApiController]
[Route("webhook")]
public class ReceiveMessagesController : ControllerBase
{
    private readonly IChatBotService _chatBotService;

    public ReceiveMessagesController(IChatBotService chatBotService)
    {
        _chatBotService = chatBotService;
    }

    // Endpoint GET para la validación del webhook
    [HttpGet]
    public IActionResult GetWebhook(
        [FromQuery(Name = "hub.mode")] string mode,
        [FromQuery(Name = "hub.challenge")] string challenge,
        [FromQuery(Name = "hub.verify_token")] string verifyToken)
    {
        if (verifyToken == "hola")
        {
            return Ok(challenge);
        }
        return Unauthorized();
    }

    // Endpoint POST para el procesamiento de mensajes entrantes
    [HttpPost]
    public async Task<IActionResult> PostWebhook([FromBody] WebHookResponseModel webhookResponse)
    {
        if (webhookResponse?.Entry == null || webhookResponse.Entry.Length == 0)
        {
            return BadRequest();
        }

        var entry = webhookResponse.Entry.FirstOrDefault();
        var change = entry?.Changes?.FirstOrDefault();
        
        var message = change?.value?.Messages.FirstOrDefault();

        if (message == null)
        {
            return BadRequest();
        }

        // string incomingMessage = message.Text?.Body;
        string incomingMessage = message.Text.body;
        string fromPhone = message.From;

        if (!string.IsNullOrEmpty(incomingMessage))
        {
            await _chatBotService.ProcessMessageAsync(fromPhone, incomingMessage);
        }

        return Ok();
    }
}