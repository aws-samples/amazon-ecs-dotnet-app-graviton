using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MvcMovie.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MvcMovie.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<AppUser> userMgr, SignInManager<AppUser> signinMgr, ILogger<AccountController> logger)
        {
            _userManager = userMgr;
            _signInManager = signinMgr;
            _logger = logger;
        }
        
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            var login = new Login { ReturnUrl = returnUrl };
            return View(login);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login login)
        {
            if (ModelState.IsValid)
            {
                var appUser = await _userManager.FindByNameAsync(login.Email);
                if (appUser != null)
                {
                    await _signInManager.SignOutAsync();
                    var result = await _signInManager.PasswordSignInAsync(appUser, login.Password, false, true);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"User {login.Email} logged in.");
                        return Redirect(login.ReturnUrl ?? "/");
                    }
                }
                ModelState.AddModelError(nameof(login.Email), "Login Failed: Invalid Email or password");
            }
            return View(login);
        }

        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user != null)
                _logger.LogInformation($"User {user.UserName} logged out.");
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        
        public IActionResult ChangePassword()
        {
            var model = new ChangePassword();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePassword changePassword)
        {
            if (!ModelState.IsValid)
                return View(changePassword);
                
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
                return NotFound();

            var changePassResult = await _userManager.ChangePasswordAsync(user, changePassword.CurrentPassword, changePassword.NewPassword);

            if (!changePassResult.Succeeded)
            {
                foreach (var error in changePassResult.Errors)
                    ModelState.AddModelError(error.Code, error.Description);
                return View(changePassword);
            }
            
            _logger.LogInformation($"User {user.UserName} password changed.");

            return RedirectToAction("ChangePasswordConfirmation");
        }

        public IActionResult ChangePasswordConfirmation()
        {
            return View();
        }
    }
}