using System;
using Model;
using Services.Interfaces;

namespace Services;

public class AppointmentSlotManager : IAppointmentSlotManager
{
    // Lista de horarios para cada especialidad (cada uno es independiente).
    private static List<AppointmentSlot> _slots = new List<AppointmentSlot>
        {
            // Horarios para Psicóloga
            new AppointmentSlot { StartTime = new DateTime(2025, 2, 10, 21, 0, 0), Specialty = "Psicóloga", Booked = false },
            new AppointmentSlot { StartTime = new DateTime(2025, 2, 10, 22, 0, 0), Specialty = "Psicóloga", Booked = false },
            new AppointmentSlot { StartTime = new DateTime(2025, 2, 10, 23, 0, 0), Specialty = "Psicóloga", Booked = false },
            new AppointmentSlot { StartTime = new DateTime(2025, 2, 11, 1, 0, 0), Specialty = "Psicóloga", Booked = false },
            new AppointmentSlot { StartTime = new DateTime(2025, 2, 11, 2, 0, 0), Specialty = "Psicóloga", Booked = false },
            new AppointmentSlot { StartTime = new DateTime(2025, 2, 11, 3, 0, 0), Specialty = "Psicóloga", Booked = false },
            new AppointmentSlot { StartTime = new DateTime(2025, 2, 11, 4, 0, 0), Specialty = "Psicóloga", Booked = false },
            new AppointmentSlot { StartTime = new DateTime(2025, 2, 11, 5, 0, 0), Specialty = "Psicóloga", Booked = false },
            new AppointmentSlot { StartTime = new DateTime(2025, 2, 11, 6, 0, 0), Specialty = "Psicóloga", Booked = false },

            // Horarios para Fisioterapia (pueden ser iguales o diferentes; son independientes)
            new AppointmentSlot { StartTime = new DateTime(2025, 2, 10, 21, 0, 0), Specialty = "Fisioterapia", Booked = false },
            new AppointmentSlot { StartTime = new DateTime(2025, 2, 10, 22, 0, 0), Specialty = "Fisioterapia", Booked = false },
            new AppointmentSlot { StartTime = new DateTime(2025, 2, 10, 23, 0, 0), Specialty = "Fisioterapia", Booked = false },
            new AppointmentSlot { StartTime = new DateTime(2025, 2, 11, 1, 0, 0), Specialty = "Fisioterapia", Booked = false },
            new AppointmentSlot { StartTime = new DateTime(2025, 2, 11, 2, 0, 0), Specialty = "Fisioterapia", Booked = false },
            new AppointmentSlot { StartTime = new DateTime(2025, 2, 11, 3, 0, 0), Specialty = "Fisioterapia", Booked = false },
            new AppointmentSlot { StartTime = new DateTime(2025, 2, 11, 4, 0, 0), Specialty = "Fisioterapia", Booked = false },
            new AppointmentSlot { StartTime = new DateTime(2025, 2, 11, 5, 0, 0), Specialty = "Fisioterapia", Booked = false },
            new AppointmentSlot { StartTime = new DateTime(2025, 2, 11, 6, 0, 0), Specialty = "Fisioterapia", Booked = false },
        };

    public List<AppointmentSlot> GetAvailableSlots(string specialty)
    {
        // Filtra los horarios por la especialidad indicada y que no hayan sido reservados.
        return _slots.Where(s => s.Specialty.Equals(specialty, StringComparison.OrdinalIgnoreCase) && !s.Booked).ToList();
    }

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