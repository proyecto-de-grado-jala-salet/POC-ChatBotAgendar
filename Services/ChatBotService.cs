using System;
using System.Globalization;
using Services.Interfaces;

namespace Services;

public class ChatBotService : IChatBotService
    {
        private readonly IWhatsAppService _whatsAppService;
        private readonly IConversationManager _conversationManager;
        private readonly IDictionary<string, IConversationStateHandler> _handlers;

        public ChatBotService(IWhatsAppService whatsAppService,
                              IConversationManager conversationManager,
                              IEnumerable<IConversationStateHandler> handlers)
        {
            _whatsAppService = whatsAppService;
            _conversationManager = conversationManager;
            // Construimos un diccionario para acceder al handler según el estado
            _handlers = handlers.ToDictionary(h => h.State, StringComparer.OrdinalIgnoreCase);
        }

        public async Task ProcessMessageAsync(string phone, string message)
        {
            string lowerMessage = message.ToLower().Trim();
            var currentState = _conversationManager.GetState(phone).State;

            // Estado inicial: se espera que el usuario escriba "hola".
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

            // Delegamos el procesamiento al handler correspondiente al estado actual.
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