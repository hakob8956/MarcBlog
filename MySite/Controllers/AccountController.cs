using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySite.Models;
using MySite.Models.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MySite.Controllers
{
    public class AccountController : Controller
    {
        private const string secret_key = "6Lcz1oUUAAAAACs6XAgCZP9zuJj7u_UB_D1HrfD3";
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IProfile _profile;


        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IProfile profile)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _profile = profile;

        }
        [HttpGet]

        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return NotFound();
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model, [FromForm(Name = "g-recaptcha-response")] string response)
        {
            if (User.Identity.IsAuthenticated)
            {
                return NotFound();
            }
            // var response = Request["g-recaptcha-response"];
            //string secretKey = secret_key;
            //var client = new WebClient();
            //var result0 = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
            //var obj = JObject.Parse(result0);
            //var status = (bool)obj.SelectToken("success");
            //ViewBag.Message = status ? "Google reCaptcha validation success" : "Google reCaptcha validation failed";
            if (ModelState.IsValid /*&& status*/)
            {

                User user = new User { Email = model.Email, UserName = model.Email, Year = model.Year, Date = model.Date };



                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var resultRole = await _userManager.AddToRoleAsync(user, "user");
                    if (resultRole.Succeeded)
                    {
                        Profile profile = new Profile() { UserID = user.Id };
                        _profile.SaveProfile(profile);


                        await _signInManager.SignInAsync(user, false);



                        return RedirectToAction("Index", "Home");
                    }

                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]

        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return NotFound();
            }
            return View(new LoginModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
           
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {

                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {

                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Incorrect login and / or password");
                }
            }

            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {

            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
