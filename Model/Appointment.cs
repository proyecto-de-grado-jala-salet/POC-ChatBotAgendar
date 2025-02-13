using System;

namespace Model;

public class Appointment
{
    public string Phone { get; set; }
    public DateTime FechaHora { get; set; }
    public string Especialidad { get; set; }
    public bool NotificacionEnviada { get; set; } = false;
    public bool AsistenciaConfirmada { get; set; } = false;
}