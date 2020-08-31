using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using LibraryData;
using Library.Models.Login;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Metadata.Ecma335;

namespace Library.Controllers
{
    public class AccountController : Controller
    {
        private IPatron _patron;

        public AccountController(IPatron patron)
        {
            _patron = patron;
        }
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        //Try using FindByIdAsync
        public async Task<IActionResult> Login(Library.Models.Login.LogInInfo userModel, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(userModel);
            }
            var allPatrons = _patron.GetAll();

            var patronModels = allPatrons.Select(p => new LogInInfo
            {
                CardID = p.LibraryCard.Id.ToString(),
                Password = p.password
            }).ToList();

            foreach (var model in patronModels)
            {
                if (userModel.CardID == model.CardID && userModel.Password == model.Password)
                {
                    // Create the identity from the user info
                    var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userModel.CardID));
                    identity.AddClaim(new Claim(ClaimTypes.Name, _patron.Get(int.Parse(model.CardID)).LastName));
                    // Authenticate using the identity

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                    //Session["userID"] = _patron.Get(int.Parse(model.CardID));
                    //not working
                    return RedirectToLocal(returnUrl);
                    //return View("~/Views/Home/index.cshtml"); does the same thing
                }
            }
            ModelState.AddModelError("", "Invalid CardId or Password");
            //I was using this one with HTML Form
            //userModel.LoginErrorMessage = "Wrong username or password";
            return View("Login", userModel);
        }
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
