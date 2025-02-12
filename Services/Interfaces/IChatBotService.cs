using System;

namespace Services.Interfaces;

public interface IChatBotService
{
    Task ProcessMessageAsync(string phone, string message);
}
