using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Technovator_Web_App.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginInputModel Login { get; set; } = new LoginInputModel();

        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            ErrorMessage = string.Empty; // Ensure error message is cleared on GET
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Invalid login attempt.";
                return Page();
            }

            // For now, allow any email and password
            if (!string.IsNullOrEmpty(Login.Email) && !string.IsNullOrEmpty(Login.Password))
            {
                // Set the session variable to indicate the user is logged in
                HttpContext.Session.SetString("IsLoggedIn", "true");

                // Redirect to Join Meeting page
                return RedirectToPage("/JoinMeeting");
            }

            ErrorMessage = "Invalid login attempt.";
            return Page();
        }
    }

    public class LoginInputModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
