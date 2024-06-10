using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Technovator_Web_App.Services
{
    public class ZoomService
    {
        private readonly string _apiKey = "HGMJyDn7TDa5ETBu3xQqFg";
        private readonly string _apiSecret = "xuYlq01J000h5q0n2VLNsXbzr3X6qW2B";
        private readonly HttpClient _httpClient;

        public ZoomService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.zoom.us/v2/")
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GenerateJwtToken());
        }

        private string GenerateJwtToken()
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_apiSecret);
            var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddMinutes(20),
                SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key), Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<ZoomMeetingResponse> CreateMeetingAsync(string topic, string startTime)
        {
            var meetingDetails = new
            {
                topic,
                type = 2,
                start_time = startTime,
                duration = 30,
                settings = new
                {
                    host_video = true,
                    participant_video = true
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(meetingDetails), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("users/me/meetings", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ZoomMeetingResponse>(jsonResponse);
            }

            return null;
        }
    }

    public class ZoomMeetingResponse
    {
        public string Id { get; set; }
        public string Join_Url { get; set; }
        public string Start_Url { get; set; }
    }
}
