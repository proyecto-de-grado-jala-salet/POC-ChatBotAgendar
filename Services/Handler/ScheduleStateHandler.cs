using Services.Interfaces;
using System.Globalization;
using Model;
using Repositories;

/// <summary>
/// This handler manages the state where the system waits for the user to choose a time slot.
/// </summary>
namespace Services.Handler;

public class ScheduleStateHandler : IConversationStateHandler
{
    /// <summary>
    /// The state name that this handler processes.
    /// </summary>
    public string State => "esperando_horario";
    private readonly IWhatsAppService _whatsAppService;
    private readonly IGoogleCalendarService _googleCalendarService;
    private readonly IConversationManager _conversationManager;
    private readonly IAppointmentSlotManager _appointmentSlotManager;
    private readonly InMemoryAppointmentRepository _inMemoryAppointmentRepository;

    /// <summary>
    /// This constructor sets up the handler with all required services.
    /// </summary>
    /// <param name="whatsAppService">Service to send WhatsApp messages.</param>
    /// <param name="googleCalendarService">Service to create calendar events.</param>
    /// <param name="conversationManager">Manager for conversation state.</param>
    /// <param name="appointmentSlotManager">Manager for appointment slots.</param>
    /// <param name="inMemoryAppointmentRepository">Repository to save appointments.</param>
    public ScheduleStateHandler(
        IWhatsAppService whatsAppService,
        IGoogleCalendarService googleCalendarService,
        IConversationManager conversationManager,
        IAppointmentSlotManager appointmentSlotManager,
        InMemoryAppointmentRepository inMemoryAppointmentRepository)
    {
        _whatsAppService = whatsAppService;
        _googleCalendarService = googleCalendarService;
        _conversationManager = conversationManager;
        _appointmentSlotManager = appointmentSlotManager;
        _inMemoryAppointmentRepository = inMemoryAppointmentRepository;
    }

    /// <summary>
    /// This method handles the message when waiting for a time slot.
    /// It checks the user input and books the selected slot.
    /// </summary>
    /// <param name="phone">The sender's phone number.</param>
    /// <param name="message">The message with the chosen option.</param>
    public async Task HandleMessageAsync(string phone, string message)
    {
        var specialty = _conversationManager.GetSpecialty(phone);
        var slots = _appointmentSlotManager.GetAvailableSlots(specialty);

        if (!HasAvailableSlots(slots, phone)) return;

        if (TryParseOption(message, slots.Count, out int selectedIndex))
        {
            await ProcessSlotSelection(phone, slots[selectedIndex - 1], specialty);
        }
        else
        {
            await _whatsAppService.SendTextMessageAsync(phone, "Por favor, seleccione un horario válido de la lista.");
        }
    }

    /// <summary>
    /// Checks if there are any available slots.
    /// </summary>
    /// <param name="slots">List of available slots.</param>
    /// <param name="phone">The sender's phone number.</param>
    /// <returns>True if there is at least one slot; false otherwise.</returns>
    private bool HasAvailableSlots(List<AppointmentSlot> slots, string phone)
    {
        if (slots.Count > 0) return true;

        _whatsAppService.SendTextMessageAsync(phone, "Lo siento, no hay horarios disponibles en este momento.");
        _conversationManager.ClearState(phone);
        return false;
    }

    /// <summary>
    /// Tries to parse the user input to a valid slot option.
    /// </summary>
    /// <param name="message">The user message.</param>
    /// <param name="slotsCount">The total number of slots available.</param>
    /// <param name="selectedIndex">The parsed slot index.</param>
    /// <returns>True if the option is valid; false otherwise.</returns>
    private bool TryParseOption(string message, int slotsCount, out int selectedIndex)
    {
        return int.TryParse(message, out selectedIndex) && selectedIndex >= 1 && selectedIndex <= slotsCount;
    }

    /// <summary>
    /// Processes the slot selected by the user.
    /// </summary>
    /// <param name="phone">The sender's phone number.</param>
    /// <param name="selectedSlot">The chosen appointment slot.</param>
    /// <param name="specialty">The chosen specialty.</param>
    private async Task ProcessSlotSelection(string phone, AppointmentSlot selectedSlot, string specialty)
    {
        if (!_appointmentSlotManager.BookSlot(selectedSlot))
        {
            await _whatsAppService.SendTextMessageAsync(phone,
                "El horario seleccionado ya fue reservado. Por favor, seleccione otro horario.");
            return;
        }

        await ConfirmAppointment(phone, selectedSlot, specialty);
        await SaveAndSyncAppointment(phone, selectedSlot, specialty);
        _conversationManager.ClearState(phone);
    }

    /// <summary>
    /// Sends a confirmation message to the user with the chosen time.
    /// </summary>
    /// <param name="phone">The sender's phone number.</param>
    /// <param name="slot">The selected appointment slot.</param>
    /// <param name="specialty">The chosen specialty.</param>
    private async Task ConfirmAppointment(string phone, AppointmentSlot slot, string specialty)
    {
        string selectedTimeStr = slot.StartTime.ToString("H:mm, dd MMM yyyy", new CultureInfo("es-ES"));
        await _whatsAppService.SendTextMessageAsync(phone,
            $"Ha seleccionado el horario: {selectedTimeStr}. Su cita está siendo agendada.");
    }

    /// <summary>
    /// Saves the appointment and adds it to Google Calendar.
    /// </summary>
    /// <param name="phone">The sender's phone number.</param>
    /// <param name="slot">The selected appointment slot.</param>
    /// <param name="specialty">The chosen specialty.</param>
    private async Task SaveAndSyncAppointment(string phone, AppointmentSlot slot, string specialty)
    {
        var appointment = new Appointment
        {
            Phone = phone,
            FechaHora = slot.StartTime,
            Especialidad = specialty
        };

        _inMemoryAppointmentRepository.SaveAppointment(appointment);

        try
        {
            if (await _googleCalendarService.CreateEventAsync(slot.StartTime, specialty))
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
}