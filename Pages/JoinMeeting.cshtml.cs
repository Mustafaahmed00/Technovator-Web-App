using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;

namespace Technovator_Web_App.Pages
{
    public class JoinMeetingModel : PageModel
    {
        [BindProperty]
        public JoinMeetingInputModel JoinMeeting { get; set; } = new JoinMeetingInputModel();

        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            ErrorMessage = string.Empty; // Ensure error message is cleared on GET
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Invalid meeting details.";
                return Page();
            }

            var zoomUrl = $"https://zoom.us/j/{JoinMeeting.MeetingId}?pwd={HttpUtility.UrlEncode(JoinMeeting.Password)}&uname={HttpUtility.UrlEncode(JoinMeeting.Username)}";

            return Redirect(zoomUrl); // Redirect to the Zoom meeting URL
        }
    }

    public class JoinMeetingInputModel
    {
        public string Username { get; set; } = string.Empty;
        public string MeetingId { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
