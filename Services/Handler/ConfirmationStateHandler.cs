using Services.Interfaces;

namespace Services.Handler;

/// <summary>
/// This handler manages the conversation state where the system waits for confirmation.
/// </summary>
public class ConfirmationStateHandler : IConversationStateHandler
{
    /// <summary>
    /// The state name that this handler processes.
    /// </summary>
    public string State => "esperando_confirmacion";
    private readonly IWhatsAppService _whatsAppService;
    private readonly IConversationManager _conversationManager;

    /// <summary>
    /// This constructor sets up the handler.
    /// </summary>
    /// <param name="whatsAppService">Service to send WhatsApp messages.</param>
    /// <param name="conversationManager">Manager for conversation state.</param>
    public ConfirmationStateHandler(IWhatsAppService whatsAppService, IConversationManager conversationManager)
    {
        _whatsAppService = whatsAppService;
        _conversationManager = conversationManager;
    }

    /// <summary>
    /// This method handles a message when waiting for confirmation.
    /// It checks if the user said "yes" or "no" and responds accordingly.
    /// </summary>
    /// <param name="phone">The sender's phone number.</param>
    /// <param name="message">The received message.</param>
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