using System.Collections.Concurrent;
using Model;
using Services.Interfaces;

namespace Services;

public class ConversationManager : IConversationManager
{
    private readonly ConcurrentDictionary<string, ConversationState> _conversations = new();

    public ConversationState GetState(string phone)
    {
        return _conversations.GetOrAdd(phone, new ConversationState());
    }

    public void SetState(string phone, string state)
    {
        var conv = GetState(phone);
        conv.State = state;
    }

    public void SetSpecialty(string phone, string specialty)
    {
        var conv = GetState(phone);
        conv.Specialty = specialty;
    }

    public string GetSpecialty(string phone)
    {
        return GetState(phone).Specialty;
    }

    public void ClearState(string phone)
    {
        _conversations.TryRemove(phone, out _);
    }
}
