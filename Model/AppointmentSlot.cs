using System;

namespace Model;

public class AppointmentSlot
{
    public DateTime StartTime { get; set; }
    public string Specialty { get; set; } // Por ejemplo: "Psicóloga" o "Fisioterapia"
    public bool Booked { get; set; }
}