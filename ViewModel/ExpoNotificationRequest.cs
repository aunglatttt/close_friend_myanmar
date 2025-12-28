using System;
using System.Text.Json.Serialization;

namespace CloseFriendMyanamr.ViewModel;

public class ExpoNotificationRequest
{
    [JsonPropertyName("to")]
    public List<string> To { get; set; } = new List<string>();

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public object? Data { get; set; }

    [JsonPropertyName("sound")]
    public string Sound { get; set; } = "default";
}
