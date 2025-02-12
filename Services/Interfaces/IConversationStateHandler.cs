using System;

namespace Services.Interfaces;

public interface IConversationStateHandler
{
    string State { get; }
    Task HandleMessageAsync(string phone, string message);
}
