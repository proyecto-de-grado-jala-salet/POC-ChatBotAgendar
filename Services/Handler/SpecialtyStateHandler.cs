using Services.Interfaces;
using System.Globalization;

namespace Services.Handler;

/// <summary>
/// This handler manages the state where the system waits for the user to choose a specialty.
/// </summary>
public class SpecialtyStateHandler : IConversationStateHandler
{
    /// <summary>
    /// The state name that this handler processes.
    /// </summary>
    public string State => "esperando_especialidad";
    private readonly IWhatsAppService _whatsAppService;
    private readonly IConversationManager _conversationManager;
    private readonly IAppointmentSlotManager _appointmentSlotManager;

    /// <summary>
    /// This constructor sets up the handler.
    /// </summary>
    /// <param name="whatsAppService">Service to send WhatsApp messages.</param>
    /// <param name="conversationManager">Manager for conversation state.</param>
    /// <param name="appointmentSlotManager">Manager for appointment slots.</param>
    public SpecialtyStateHandler(IWhatsAppService whatsAppService,
                                 IConversationManager conversationManager,
                                 IAppointmentSlotManager appointmentSlotManager)
    {
        _whatsAppService = whatsAppService;
        _conversationManager = conversationManager;
        _appointmentSlotManager = appointmentSlotManager;
    }

    /// <summary>
    /// This method handles a message when waiting for the user to choose a specialty.
    /// It sets the specialty and then shows available time slots.
    /// </summary>
    /// <param name="phone">The sender's phone number.</param>
    /// <param name="message">The message with the chosen option.</param>
    public async Task HandleMessageAsync(string phone, string message)
    {
        if (message == "1" || message == "2")
        {
            string specialty = message == "1" ? "Psicóloga" : "Fisioterapia";
            _conversationManager.SetSpecialty(phone, specialty);
            _conversationManager.SetState(phone, "esperando_horario");

            var availableSlots = _appointmentSlotManager.GetAvailableSlots(specialty);
            if (availableSlots.Count == 0)
            {
                await _whatsAppService.SendTextMessageAsync(phone,
                    $"Lo siento, no hay horarios disponibles en este momento para {specialty}.");
                _conversationManager.ClearState(phone);
                return;
            }

            string horariosDisponibles = "Estos son los horarios disponibles:\n";
            
            for (int i = 0; i < availableSlots.Count; i++)
            {
                string timeStr = availableSlots[i].StartTime.ToString("H:mm, dd MMM yyyy", new CultureInfo("es-ES"));
                horariosDisponibles += $"{i + 1}. {timeStr}\n";
            }
            horariosDisponibles += $"Por favor, seleccione un horario (1-{availableSlots.Count}).";

            await _whatsAppService.SendTextMessageAsync(phone,
                $"Ha seleccionado {specialty}. {horariosDisponibles}");
        }
        else
        {
            await _whatsAppService.SendTextMessageAsync(phone,
                "Por favor, seleccione una especialidad válida (1 o 2).");
        }
    }
}