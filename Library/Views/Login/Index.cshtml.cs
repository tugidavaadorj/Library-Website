using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Library.Views.Login
{
    public class LoginModel : PageModel
    {
        [BindProperty] // Bind on Post
        public LogInInfo loginData { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var isValid = (loginData.CardID == "username" && loginData.Password == "password"); // TODO Validate the username and the password with your own logic
                if (!isValid)
                {
                    ModelState.AddModelError("", "username or password is invalid");
                    return Page();
                }
                // Create the identity from the user info
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, loginData.CardID));
                identity.AddClaim(new Claim(ClaimTypes.Name, loginData.CardID));
                // Authenticate using the identity
                var principal = new ClaimsPrincipal(identity);
                return RedirectToPage("Home/Index");
            }
            else
            {
                ModelState.AddModelError("", "username or password is blank");
                return Page();
            }
        }

        public class LogInInfo
        {
            [DisplayName("Card Id: ")]
            [Required(ErrorMessage = "This field is required.")]
            public string CardID { get; set; }
            [DisplayName("Password: ")]
            [DataType(DataType.Password)]
            [Required(ErrorMessage = "This field is required.")]
            public string Password { get; set; }
        }
    }
}