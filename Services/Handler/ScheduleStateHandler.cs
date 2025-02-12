using System;
using Services.Interfaces;
using System.Globalization;

namespace Services.Handler;

public class ScheduleStateHandler : IConversationStateHandler
{
    public string State => "esperando_horario";
    private readonly IWhatsAppService _whatsAppService;
    private readonly IGoogleCalendarService _googleCalendarService;
    private readonly IConversationManager _conversationManager;
    private readonly IAppointmentSlotManager _appointmentSlotManager;

    public ScheduleStateHandler(IWhatsAppService whatsAppService,
                                IGoogleCalendarService googleCalendarService,
                                IConversationManager conversationManager,
                                IAppointmentSlotManager appointmentSlotManager)
    {
        _whatsAppService = whatsAppService;
        _googleCalendarService = googleCalendarService;
        _conversationManager = conversationManager;
        _appointmentSlotManager = appointmentSlotManager;
    }

    public async Task HandleMessageAsync(string phone, string message)
    {
        var specialty = _conversationManager.GetSpecialty(phone);
        var slots = _appointmentSlotManager.GetAvailableSlots(specialty);
        if (slots.Count == 0)
        {
            await _whatsAppService.SendTextMessageAsync(phone,
                "Lo siento, no hay horarios disponibles en este momento.");
            _conversationManager.ClearState(phone);
            return;
        }

        if (int.TryParse(message, out int opcion) && opcion >= 1 && opcion <= slots.Count)
        {
            // Se obtiene el horario seleccionado basado en el índice mostrado.
            var selectedSlot = slots[opcion - 1];
            bool booked = _appointmentSlotManager.BookSlot(selectedSlot);
            if (!booked)
            {
                await _whatsAppService.SendTextMessageAsync(phone,
                    "El horario seleccionado ya fue reservado. Por favor, seleccione otro horario.");
                return;
            }

            string selectedTimeStr = selectedSlot.StartTime.ToString("H:mm, dd MMM yyyy", new CultureInfo("es-ES"));
            await _whatsAppService.SendTextMessageAsync(phone,
                $"Ha seleccionado el horario: {selectedTimeStr}. Su cita está siendo agendada.");

            try
            {
                DateTime startTime = selectedSlot.StartTime;
                bool eventCreated = await _googleCalendarService.CreateEventAsync(startTime, specialty);

                if (eventCreated)
                {
                    await _whatsAppService.SendTextMessageAsync(phone,
                        "La cita se agendó correctamente en Google Calendar.");
                }
                else
                {
                    await _whatsAppService.SendTextMessageAsync(phone,
                        "Ocurrió un error al agendar la cita en Google Calendar.");
                }
            }
            catch (Exception)
            {
                await _whatsAppService.SendTextMessageAsync(phone,
                    "Ocurrió un error al procesar la fecha y hora seleccionada.");
            }
            _conversationManager.ClearState(phone);
        }
        else
        {
            await _whatsAppService.SendTextMessageAsync(phone,
                "Por favor, seleccione un horario válido de la lista.");
        }
    }
}