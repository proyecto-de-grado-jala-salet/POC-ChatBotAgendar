using Model;
using System.Collections.Concurrent;

namespace Repositories;

public class InMemoryAppointmentRepository
{
    private static readonly ConcurrentDictionary<string, Appointment> _appointments = new();

    public void SaveAppointment(Appointment appointment)
    {
        // Aquí usamos el número de teléfono como llave (puedes optar por otro identificador)
        _appointments[appointment.Phone] = appointment;
    }

    public Appointment GetAppointment(string phone)
    {
        _appointments.TryGetValue(phone, out var appointment);
        return appointment;
    }

    public List<Appointment> GetAppointmentsInRange(DateTime start, DateTime end)
    {
        return _appointments.Values
            .Where(a => a.FechaHora >= start && a.FechaHora <= end)
            .ToList();
    }

    public void UpdateAppointment(Appointment appointment)
    {
        _appointments[appointment.Phone] = appointment;
    }
}
