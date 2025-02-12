using System;
using Model;

namespace Services.Interfaces;

public interface IConversationManager
{
    ConversationState GetState(string phone);
    void SetState(string phone, string state);
    void SetSpecialty(string phone, string specialty);
    string GetSpecialty(string phone);
    void ClearState(string phone);
}
