using System;
using System.Globalization;
using Services.Interfaces;

namespace Services;

public class ChatBotService : IChatBotService
{
    private readonly IWhatsAppService _whatsAppService;
    private readonly IGoogleCalendarService _googleCalendarService;
    private readonly IConversationManager _conversationManager;

    public ChatBotService(IWhatsAppService whatsAppService,
                          IGoogleCalendarService googleCalendarService,
                          IConversationManager conversationManager)
    {
        _whatsAppService = whatsAppService;
        _googleCalendarService = googleCalendarService;
        _conversationManager = conversationManager;
    }

    public async Task ProcessMessageAsync(string phone, string message)
    {
        string lowerMessage = message.ToLower();

        // Respuesta inicial
        if (lowerMessage.Contains("hola"))
        {
            await _whatsAppService.SendTextMessageAsync(phone,
                "Hola, es el Centro de Atención E-Dukate. ¿Desea reservar una cita? (Responda 'Sí' o 'No')");
        }
        // Confirmación de reserva
        else if (lowerMessage.Contains("sí") || lowerMessage.Contains("si"))
        {
            _conversationManager.SetState(phone, "reserva");
            await _whatsAppService.SendTextMessageAsync(phone,
                "Por favor, seleccione la especialidad:\n1. Psicóloga\n2. Fisioterapia");
        }
        // Selección de especialidad
        else if (message == "1" || message == "2")
        {
            string specialty = message == "1" ? "Psicóloga" : "Fisioterapia";
            _conversationManager.SetSpecialty(phone, specialty);

            string horariosDisponibles = "Estos son los horarios disponibles:\n" +
                "1. 21:00, 10 feb 2025\n" +
                "2. 22:00, 10 feb 2025\n" +
                "3. 23:00, 10 feb 2025\n" +
                "4. 1:00, 11 feb 2025\n" +
                "5. 2:00, 11 feb 2025\n" +
                "6. 3:00, 11 feb 2025\n" +
                "7. 4:00, 11 feb 2025\n" +
                "8. 5:00, 11 feb 2025\n" +
                "9. 6:00, 11 feb 2025\n" +
                "Por favor, seleccione un horario (1-9).";

            await _whatsAppService.SendTextMessageAsync(phone, $"Ha seleccionado {specialty}. {horariosDisponibles}");
        }
        // Selección de horario
        else if (message.Length == 1 && char.IsDigit(message[0]) && message[0] >= '1' && message[0] <= '9')
        {
            int index = int.Parse(message) - 1;
            string[] horarios = {
                    "21:00, 10 feb 2025",
                    "22:00, 10 feb 2025",
                    "23:00, 10 feb 2025",
                    "1:00, 11 feb 2025",
                    "2:00, 11 feb 2025",
                    "3:00, 11 feb 2025",
                    "4:00, 11 feb 2025",
                    "5:00, 11 feb 2025",
                    "6:00, 11 feb 2025"
                };

            if (index < 0 || index >= horarios.Length)
            {
                await _whatsAppService.SendTextMessageAsync(phone, "El horario seleccionado no es válido.");
                return;
            }

            string selectedTime = horarios[index];
            await _whatsAppService.SendTextMessageAsync(phone, $"Ha seleccionado el horario: {selectedTime}. Su cita está siendo agendada.");

            try
            {
                var culture = new CultureInfo("es-ES");
                DateTime startTime = DateTime.ParseExact(selectedTime, "H:mm, dd MMM yyyy", culture);
                string specialty = _conversationManager.GetSpecialty(phone);
                bool eventCreated = await _googleCalendarService.CreateEventAsync(startTime, specialty);

                if (eventCreated)
                {
                    await _whatsAppService.SendTextMessageAsync(phone, "La cita se agendó correctamente en Google Calendar.");
                }
                else
                {
                    await _whatsAppService.SendTextMessageAsync(phone, "Ocurrió un error al agendar la cita en Google Calendar.");
                }
            }
            catch (Exception)
            {
                await _whatsAppService.SendTextMessageAsync(phone, "Ocurrió un error al procesar la fecha y hora seleccionada.");
            }
        }
        // Respuesta negativa
        else if (lowerMessage.Contains("no"))
        {
            await _whatsAppService.SendTextMessageAsync(phone, "Gracias por contactarnos. ¡Hasta luego!");
        }
        // Respuesta no reconocida
        else
        {
            await _whatsAppService.SendTextMessageAsync(phone, "Lo siento, no entendí su respuesta. Por favor, siga las instrucciones.");
        }
    }
}