using System;
using System.Collections.Concurrent;

namespace POC_ChatBotAgendar.Model;

public class ConversationManager
{
    // Usamos ConcurrentDictionary para evitar problemas de concurrencia
        private static ConcurrentDictionary<string, ConversationState> _conversations = new();

        public static ConversationState GetState(string phone)
        {
            return _conversations.GetOrAdd(phone, new ConversationState());
        }

        public static void SetState(string phone, string state)
        {
            var conv = GetState(phone);
            conv.State = state;
        }

        public static void SetSpecialty(string phone, string specialty)
        {
            var conv = GetState(phone);
            conv.Specialty = specialty;
        }

        public static string GetSpecialty(string phone)
        {
            return GetState(phone).Specialty;
        }

        public static void ClearState(string phone)
        {
            _conversations.TryRemove(phone, out _);
        }
}
