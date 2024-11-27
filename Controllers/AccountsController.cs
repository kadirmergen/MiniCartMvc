using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniCartMvc.Identity;
using Microsoft.AspNetCore.Identity;
using MiniCartMvc.Models;

namespace MiniCartMvc.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IdentityDataContext _identityContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, IdentityDataContext identityDataContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _identityContext = identityDataContext;
        }
        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterModel registerModel)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser();
                user.Name = registerModel.Name;
                user.Surname = registerModel.Surname;
                user.Email = registerModel.Email;
                user.UserName = registerModel.Username;

                IdentityResult result = _userManager.CreateAsync(user, registerModel.Password).Result;

                if (result.Succeeded) 
                {
                    //kullanıcı oluştu ve kullanıcıyı bir role atayabilirsiniz
                    if (_roleManager.RoleExistsAsync("User").Result)
                    {
                        _userManager.AddToRoleAsync(user, "User");
                    }
                    return RedirectToAction("Login", "Accounts");
                }
                else
                {
                    ModelState.AddModelError("RegisterUserError", "User registration has been denied.");
                }
                
            }
            return View(registerModel);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel loginModel)
        {
            Microsoft.AspNetCore.Identity.SignInResult signInResult;
            if (ModelState.IsValid)
            {
                var user = _signInManager.UserManager.FindByNameAsync(loginModel.Username).Result;
                if (user == null)
                {
                    ModelState.AddModelError("LoginUserError", "Invalid username or password.");
                    return View(loginModel);
                }
                signInResult = _signInManager.PasswordSignInAsync(user.UserName, loginModel.Password, 
                    isPersistent: loginModel.RememberMe, lockoutOnFailure: false).Result;
                if (signInResult.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                }
            }
            return View(loginModel);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync().Wait();
            return RedirectToAction("Login", "Accounts");
        }
    }
}
