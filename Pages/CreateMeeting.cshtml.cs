using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Technovator_Web_App.Pages
{
    public class CreateMeetingModel : PageModel
    {
        private readonly ILogger<CreateMeetingModel> _logger;

        public CreateMeetingModel(ILogger<CreateMeetingModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        public string MeetingId { get; set; }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public int Duration { get; set; } // Duration in minutes

        private static string ZoomApiUrl = "https://api.zoom.us/v2";
        private static string ClientId = "HGMJyDn7TDa5ETBu3xQqFg";
        private static string ClientSecret = "xuYlq01J000h5q0n2VLNsXbzr3X6qW2B";

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var token = GenerateJwtToken();
                var meetingDetails = await CreateZoomMeetingAsync(token);

                if (meetingDetails != null)
                {
                    TempData["SuccessMessage"] = "Zoom meeting created successfully.";
                    TempData["MeetingDetails"] = JsonConvert.SerializeObject(meetingDetails);
                    return RedirectToPage("/MeetingSuccess");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to create Zoom meeting. Please try again.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Zoom meeting");
                TempData["ErrorMessage"] = "An error occurred while creating the Zoom meeting. Please try again.";
                return Page();
            }
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

        private async Task<dynamic> CreateZoomMeetingAsync(string token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var meetingDetails = new
                {
                    topic = "Test Zoom Meeting",
                    type = 2,
                    start_time = DateTime.UtcNow.AddMinutes(10),
                    duration = Duration,
                    timezone = "UTC",
                    settings = new { host_video = "true", participant_video = "true" }
                };

                var response = await client.PostAsJsonAsync($"{ZoomApiUrl}/users/me/meetings", meetingDetails);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject(responseContent);
                }
                else
                {
                    _logger.LogError("Failed to create Zoom meeting: {statusCode}", response.StatusCode);
                    return null;
                }
            }
        }
    }
}
