using System;
using Services.Interfaces;

namespace Services.Handler;

public class ConfirmationStateHandler : IConversationStateHandler
{
    public string State => "esperando_confirmacion";
    private readonly IWhatsAppService _whatsAppService;
    private readonly IConversationManager _conversationManager;

    public ConfirmationStateHandler(IWhatsAppService whatsAppService, IConversationManager conversationManager)
    {
        _whatsAppService = whatsAppService;
        _conversationManager = conversationManager;
    }

    public async Task HandleMessageAsync(string phone, string message)
    {
        string lowerMessage = message.ToLower().Trim();

        if (lowerMessage == "sí" || lowerMessage == "si")
        {
            _conversationManager.SetState(phone, "esperando_especialidad");
            await _whatsAppService.SendTextMessageAsync(phone,
                "Por favor, seleccione la especialidad:\n1. Psicóloga\n2. Fisioterapia");
        }
        else if (lowerMessage == "no")
        {
            await _whatsAppService.SendTextMessageAsync(phone,
                "Gracias por contactarnos. ¡Hasta luego!");
            _conversationManager.ClearState(phone);
        }
        else
        {
            await _whatsAppService.SendTextMessageAsync(phone,
                "Responda 'Sí' para continuar o 'No' para finalizar.");
        }
    }
}