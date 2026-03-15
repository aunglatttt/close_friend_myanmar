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

    public async Task SendNotificationAsync(List<string> expoPushToken, string propertyId)
    {
        var client = _httpClientFactory.CreateClient();

        var payload = new
        {
            to = expoPushToken,
            title = "New Property Added!",
            body = "Tap to view detail.",
            data = new { url = $"https://admin.closefriendmyanmar.com/Property/PropertyInfo?propertyId={propertyId}" },
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
