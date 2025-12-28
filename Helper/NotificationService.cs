using System;
using System.Text;
using System.Text.Json;
using CloseFriendMyanamr.ViewModel;

namespace CloseFriendMyanamr.Helper;

public class NotificationService
{
private readonly IHttpClientFactory _httpClientFactory;
    private const string ExpoApiUrl = "https://exp.host/--/api/v2/push/send";

    public NotificationService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task SendNotificationAsync(string expoPushToken, string title, string message, string propertyId)
    {
        var client = _httpClientFactory.CreateClient();
        
        // var payload = new ExpoNotificationRequest
        // {
        //     To = new List<string> { expoPushToken },
        //     Title = title,
        //     Body = message,
        //     Data = new { ClickAction = "NavigateToSettings" } // Optional custom data
        // };

        var payload = new
        {
            to = new List<string> {expoPushToken},
            title = "New Property Added!",
            body = "Tap to view the latest listing.",
            data = new { url = $"https://tbalmm-001-site1.jtempurl.com/Property/PropertyInfo?propertyId={propertyId}" },
            priority = "high",
            sound = "default",
            channelId = "default"
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Expo recommends these headers
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");

        var response = await client.PostAsync(ExpoApiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Notification sent! Response: {responseString}");
        }
        else
        {
            Console.WriteLine($"Failed to send notification. Status: {response.StatusCode}");
        }
    }
}
