using Services.Interfaces;

namespace Services;

/// <summary>
/// This class processes chat messages and routes them to the correct handler
/// based on the current conversation state.
/// </summary>
public class ChatBotService : IChatBotService
{
    private readonly IWhatsAppService _whatsAppService;
    private readonly IConversationManager _conversationManager;
    private readonly IDictionary<string, IConversationStateHandler> _handlers;

    /// <summary>
    /// This constructor sets up the chat bot service with the needed services.
    /// </summary>
    /// <param name="whatsAppService">Service to send WhatsApp messages.</param>
    /// <param name="conversationManager">Manager for conversation state.</param>
    /// <param name="handlers">A list of handlers for each conversation state.</param>
    public ChatBotService(IWhatsAppService whatsAppService,
                          IConversationManager conversationManager,
                          IEnumerable<IConversationStateHandler> handlers)
    {
        _whatsAppService = whatsAppService;
        _conversationManager = conversationManager;
        // Construimos un diccionario para acceder al handler según el estado
        _handlers = handlers.ToDictionary(h => h.State, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Processes an incoming message and sends it to the right state handler.
    /// </summary>
    /// <param name="phone">The sender's phone number.</param>
    /// <param name="message">The message to process.</param>
    public async Task ProcessMessageAsync(string phone, string message)
    {
        string lowerMessage = message.ToLower().Trim();
        var currentState = _conversationManager.GetState(phone).State;

        if (string.IsNullOrEmpty(currentState))
        {
            if (lowerMessage.Contains("hola"))
            {
                await _whatsAppService.SendTextMessageAsync(phone,
                    "Hola, es el Centro de Atención E-Dukate. ¿Desea reservar una cita? (Responda 'Sí' o 'No')");
                _conversationManager.SetState(phone, "esperando_confirmacion");
            }
            else
            {
                await _whatsAppService.SendTextMessageAsync(phone,
                    "Por favor, inicie la conversación escribiendo 'Hola'.");
            }
            return;
        }

        if (_handlers.TryGetValue(currentState, out var handler))
        {
            await handler.HandleMessageAsync(phone, message);
        }
        else
        {
            await _whatsAppService.SendTextMessageAsync(phone,
                "No entiendo su mensaje. Por favor, inicie la conversación escribiendo 'Hola' y siga las instrucciones.");
        }
    }
}