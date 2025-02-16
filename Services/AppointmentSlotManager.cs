using Model;
using Services.Interfaces;

namespace Services;

/// <summary>
/// This class manages the appointment slots. /// It holds a list of time slots for different specialties.
/// </summary>
public class AppointmentSlotManager : IAppointmentSlotManager
{
    public const string PSYCHOLOGY_SPECIALTY = "Psicóloga";
    public const string PHYSIOTHERAPY_SPECIALTY = "Fisioterapia";

    // Lista de horarios para cada especialidad (cada uno es independiente).
    private readonly List<AppointmentSlot> _slots = new List<AppointmentSlot>
    {
        // Horarios para Psicóloga
        new AppointmentSlot { StartTime = new DateTime(2025, 2, 10, 21, 0, 0, DateTimeKind.Local), Specialty = PSYCHOLOGY_SPECIALTY, Booked = false },
        new AppointmentSlot { StartTime = new DateTime(2025, 2, 10, 22, 0, 0, DateTimeKind.Local), Specialty = PSYCHOLOGY_SPECIALTY, Booked = false },
        new AppointmentSlot { StartTime = new DateTime(2025, 2, 10, 23, 0, 0, DateTimeKind.Local), Specialty = PSYCHOLOGY_SPECIALTY, Booked = false },
        new AppointmentSlot { StartTime = new DateTime(2025, 2, 11, 1, 0, 0, DateTimeKind.Local), Specialty = PSYCHOLOGY_SPECIALTY, Booked = false },
        new AppointmentSlot { StartTime = new DateTime(2025, 2, 15, 21, 58, 0, DateTimeKind.Local), Specialty = PSYCHOLOGY_SPECIALTY, Booked = false },

        // Horarios para Fisioterapia (pueden ser iguales o diferentes; son independientes)
        new AppointmentSlot { StartTime = new DateTime(2025, 2, 10, 21, 0, 0, DateTimeKind.Local), Specialty = PHYSIOTHERAPY_SPECIALTY, Booked = false },
        new AppointmentSlot { StartTime = new DateTime(2025, 2, 10, 22, 0, 0, DateTimeKind.Local), Specialty = PHYSIOTHERAPY_SPECIALTY, Booked = false },
        new AppointmentSlot { StartTime = new DateTime(2025, 2, 10, 23, 0, 0, DateTimeKind.Local), Specialty = PHYSIOTHERAPY_SPECIALTY, Booked = false },
        new AppointmentSlot { StartTime = new DateTime(2025, 2, 11, 1, 0, 0, DateTimeKind.Local), Specialty = PHYSIOTHERAPY_SPECIALTY, Booked = false },
    };

    /// <summary>
    /// Gets the available slots for a given specialty.
    /// </summary>
    /// <param name="specialty">The chosen specialty.</param>
    /// <returns>A list of slots that are not booked.</returns>
    public List<AppointmentSlot> GetAvailableSlots(string specialty)
    {
        // Filtra los horarios por la especialidad indicada y que no hayan sido reservados.
        return _slots.Where(s => s.Specialty.Equals(specialty, StringComparison.OrdinalIgnoreCase) && !s.Booked).ToList();
    }

    /// <summary>
    /// Books an appointment slot if it is available.
    /// </summary>
    /// <param name="slot">The slot to book.</param>
    /// <returns>True if the slot was booked; false otherwise.</returns>
    public bool BookSlot(AppointmentSlot slot)
    {
        var found = _slots.FirstOrDefault(s =>
            s.Specialty.Equals(slot.Specialty, StringComparison.OrdinalIgnoreCase) &&
            s.StartTime == slot.StartTime &&
            !s.Booked);

        if (found != null)
        {
            found.Booked = true;
            return true;
        }
        return false;
    }
}