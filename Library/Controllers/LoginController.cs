using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using LibraryData;
using Library.Models.Login;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Library.Controllers
{
    public class LoginController : Controller
    {
        private IPatron _patron;
        public LoginController(IPatron patron)
        {
            _patron = patron;
        }
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Authorize(Library.Models.Login.LogInInfo userModel)
        {
            var allPatrons = _patron.GetAll();

            var patronModels = allPatrons.Select(p => new LogInInfo
            {
                CardID = p.LibraryCard.Id.ToString(),
                Password = p.password
            }).ToList();

            foreach(var model in patronModels)
            {
                if (userModel.CardID == model.CardID && userModel.Password == model.Password)
                {
                    // Create the identity from the user info
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userModel.CardID));
                    identity.AddClaim(new Claim(ClaimTypes.Name, userModel.Password));
                    // Authenticate using the identity
                    var principal = new ClaimsPrincipal(identity);
                    
                    //Session["userID"] = _patron.Get(int.Parse(model.CardID));
                    //not working
                    return RedirectToAction("Index", "Home");
                    //return View("~/Views/Home/index.cshtml"); does the same thing
                }
            }
            userModel.LoginErrorMessage = "Wrong username or password";
            return View("index", userModel);
        }
    }
}
