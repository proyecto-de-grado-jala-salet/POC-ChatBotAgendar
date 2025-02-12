using System;

namespace Model;

public class WebHookResponseModel
{
    public required Entry[] entry { get; set; }
}