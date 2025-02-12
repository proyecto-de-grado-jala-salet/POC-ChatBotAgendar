using System;
using Services.Interfaces;
using System.Globalization;
using System.Threading.Tasks;

namespace Services.Handler;

public class SpecialtyStateHandler : IConversationStateHandler
{
    public string State => "esperando_especialidad";
    private readonly IWhatsAppService _whatsAppService;
    private readonly IConversationManager _conversationManager;
    private readonly IAppointmentSlotManager _appointmentSlotManager;

    public SpecialtyStateHandler(IWhatsAppService whatsAppService,
                                 IConversationManager conversationManager,
                                 IAppointmentSlotManager appointmentSlotManager)
    {
        _whatsAppService = whatsAppService;
        _conversationManager = conversationManager;
        _appointmentSlotManager = appointmentSlotManager;
    }

    public async Task HandleMessageAsync(string phone, string message)
    {
        if (message == "1" || message == "2")
        {
            string specialty = message == "1" ? "Psicóloga" : "Fisioterapia";
            _conversationManager.SetSpecialty(phone, specialty);
            _conversationManager.SetState(phone, "esperando_horario");

            // Obtiene los horarios disponibles para la especialidad seleccionada.
            var availableSlots = _appointmentSlotManager.GetAvailableSlots(specialty);
            if (availableSlots.Count == 0)
            {
                await _whatsAppService.SendTextMessageAsync(phone,
                    $"Lo siento, no hay horarios disponibles en este momento para {specialty}.");
                _conversationManager.ClearState(phone);
                return;
            }

            string horariosDisponibles = "Estos son los horarios disponibles:\n";
            // Se muestran numerados desde 1 hasta el total de disponibles.
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