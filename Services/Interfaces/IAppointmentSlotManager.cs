using System;
using Model;

namespace Services.Interfaces;

public interface IAppointmentSlotManager
{
    List<AppointmentSlot> GetAvailableSlots(string specialty);
    bool BookSlot(AppointmentSlot slot);
}