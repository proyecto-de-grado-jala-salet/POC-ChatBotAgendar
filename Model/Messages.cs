using System;
using POC_ChatBotAgendar.Model;

namespace Model;

public class Messages
{
    public string id { get; set; }
    public string from { get; set; }
    public string type { get; set; } = "";
    public Text text { get; set; }
}
