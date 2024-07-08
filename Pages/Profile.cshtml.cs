using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Threading.Tasks;

namespace Technovator_Web_App.Pages
{
    public class ProfileModel : PageModel
    {
        [BindProperty]
        public string FullName { get; set; }
        [BindProperty]
        public string? Email { get; set; }
        [BindProperty]
        public string PhoneNumber { get; set; }
        [BindProperty]
        public string Address { get; set; }
        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string TimeZone { get; set; }
        [BindProperty]
        public bool DarkMode { get; set; }
        [BindProperty]
        public bool TwoFactorAuth { get; set; }
        [BindProperty]
        public IFormFile ProfilePicture { get; set; }
        public string ProfilePicturePath { get; set; }

        public void OnGet()
        {
            ProfilePicturePath = HttpContext.Session.GetString("ProfilePicturePath") ?? "~/images/default-profile.jpg";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ProfilePicture != null)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", ProfilePicture.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfilePicture.CopyToAsync(stream);
                }

                HttpContext.Session.SetString("ProfilePicturePath", $"/images/{ProfilePicture.FileName}");
                ProfilePicturePath = $"/images/{ProfilePicture.FileName}";
            }

            // Save other profile data as needed
            // ...

            return Page();
        }
    }
}
