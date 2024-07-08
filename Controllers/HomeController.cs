using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

public class ZoomController : Controller
{
    private static string ZoomApiUrl = "https://api.zoom.us/v2";
    private static string ClientId = "HGMJyDn7TDa5ETBu3xQqFg";
    private static string ClientSecret = "xuYlq01J000h5q0n2VLNsXbzr3X6qW2B";

    public IActionResult CreateMeeting()
    {
        var token = GenerateJwtToken();
        var meetingDetails = CreateZoomMeeting(token);

        ViewBag.Message = meetingDetails != null ? "Zoom meeting created successfully." : "Failed to create Zoom meeting.";
        ViewBag.Response = meetingDetails;

        return View();
    }

    private string GenerateJwtToken()
    {
        var header = new { alg = "HS256", typ = "JWT" };
        var payload = new
        {
            iss = ClientId,
            exp = DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds()
        };

        var headerBase64 = Base64UrlEncode(JsonConvert.SerializeObject(header));
        var payloadBase64 = Base64UrlEncode(JsonConvert.SerializeObject(payload));

        var secretKey = Encoding.UTF8.GetBytes(ClientSecret);
        var stringToSign = $"{headerBase64}.{payloadBase64}";
        using (var hasher = new HMACSHA256(secretKey))
        {
            var signature = hasher.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
            var signatureBase64 = Base64UrlEncode(signature);

            return $"{headerBase64}.{payloadBase64}.{signatureBase64}";
        }
    }

    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    private static string Base64UrlEncode(string input)
    {
        return Base64UrlEncode(Encoding.UTF8.GetBytes(input));
    }

    private dynamic CreateZoomMeeting(string token)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var meetingDetails = new
            {
                topic = "Test Zoom Meeting",
                type = 2,
                start_time = DateTime.UtcNow.AddMinutes(10),
                duration = 30,
                timezone = "UTC",
                settings = new { host_video = "true", participant_video = "true" }
            };

            var response = client.PostAsJsonAsync($"{ZoomApiUrl}/users/me/meetings", meetingDetails).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject(responseContent);
            }
            else
            {
                return null;
            }
        }
    }
}
