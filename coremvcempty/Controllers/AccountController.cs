using coremvcempty.Models;
using coremvcempty.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coremvcempty.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;

        public UserManager<ApplicationUser> UserManager { get; }
        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager)
        {
            this.UserManager = userManager;
            this.signInManager = signInManager;
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
          
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email , City= model.City};
                var newUser= await UserManager.CreateAsync(user,model.Password);

                if (newUser.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent:false);
                    return RedirectToAction("Index", "Home");
                }
               foreach(var errors in newUser.Errors)
                {
                    ModelState.AddModelError("", errors.Description);
                }

            }
          

            return View(model);
        }

        [AllowAnonymous]
        public  IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginVM model, string returnurl)
        {
            if (ModelState.IsValid)
            {
                var result= await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnurl)&&Url.IsLocalUrl(returnurl))
                    {
                        return Redirect(returnurl);
                    }
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid User");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet][HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> IsEmailInUse(string Email)
        {
            var user = await UserManager.FindByEmailAsync(Email);

            if (user == null)
            {
                return Json(true);
            }
            else return Json($"Email {Email} is already Taken. Please use another one");
        }
    }
}
