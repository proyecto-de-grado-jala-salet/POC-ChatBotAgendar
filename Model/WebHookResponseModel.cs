using System;

namespace Model;

public class WebHookResponseModel
{
    public Entry[] Entry { get; set; } = Array.Empty<Entry>();
}