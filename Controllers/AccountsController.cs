using Microsoft.AspNetCore.Mvc;
using MiniCartMvc.Identity;
using Microsoft.AspNetCore.Identity;
using MiniCartMvc.Models;
using MiniCartMvc.Data;
using static MiniCartMvc.Models.OrderDetailsModel;
using Microsoft.AspNetCore.Authorization;
using MiniCartMvc.ViewModels;

namespace MiniCartMvc.Controllers
{
    public class AccountsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly DataContext _context;

        public AccountsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, DataContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }
        [Authorize]
        public ActionResult Index()
        {
            var username = User.Identity.Name;
            var orders = _context.Orders.Where(i => i.UserName == username).Select(i => new OrderViewModel()
            {
                Id = i.Id,
                OrderNumber = i.OrderNumber,
                OrderDate = i.OrderDate,
                OrderState = i.OrderState,
                Total = i.Total
            }).OrderByDescending(i => i.OrderDate).ToList();

            return View(orders);
        }
        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var currentUsername = User.Identity?.Name;

            if (string.IsNullOrEmpty(currentUsername))
            {
                return null; // Kullanıcı oturumu yoksa null döndür.
            }

            return await _signInManager.UserManager.FindByNameAsync(currentUsername);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            var model = new AccountSettingsViewModel
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Username = user.UserName
            };

            return View(model); 
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(AccountSettingsViewModel? accountSettingsViewModel)
        {
            if (accountSettingsViewModel == null)
            {
                return BadRequest("Invalid account settings data.");
            }

            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();
                if (user == null)
                {
                    return Unauthorized();
                }

                // Check for unique email and username
                if (_context.Users.Any(u => u.Email == accountSettingsViewModel.Email && u.Id != user.Id))
                {
                    return Conflict("The email is already in use.");
                }

                if (_context.Users.Any(u => u.UserName == accountSettingsViewModel.Username && u.Id != user.Id))
                {
                    return Conflict("The username is already in use.");
                }

                // Güncellenmek istenen alanları kontrol edin ve sadece null olmayanları güncelleyin
                user.Name = accountSettingsViewModel.Name ?? user.Name;
                user.Surname = accountSettingsViewModel.Surname ?? user.Surname;
                user.Email = accountSettingsViewModel.Email ?? user.Email;
                user.PhoneNumber = accountSettingsViewModel.PhoneNumber ?? user.PhoneNumber;
                user.UserName = accountSettingsViewModel.Username ?? user.UserName;

                // Değişiklikleri veritabanına kaydedin
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Account settings updated successfully.";

                return RedirectToAction("Details", "Accounts");
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [Authorize]
        //Account Settings
        public async Task<IActionResult> Details(int id)
        {
            var applicationUser = await GetCurrentUserAsync();

            if (applicationUser == null)
            {
                return NotFound("User cannot found!"); 
            }

            var user= new AccountSettingsViewModel() 
            {
                Name = applicationUser.Name,
                Surname = applicationUser.Surname,
                Username = applicationUser.UserName,
                Email = applicationUser.Email,
                PhoneNumber = applicationUser.PhoneNumber
            };

            if (TempData.ContainsKey("SuccessMessage"))
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
            }

            if (TempData.ContainsKey("PasswordChangedMessage"))
            {
                ViewData["PasswordChangedMessage"] = TempData["PasswordChangedMessage"];
            }

            return View(user);
        }

        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser();
                user.Name = registerModel.Name;
                user.Surname = registerModel.Surname;
                user.Email = registerModel.Email;
                user.UserName = registerModel.Username;

                if (await _signInManager.UserManager.FindByNameAsync(registerModel.Username) != null)
                {
                    // Kullanıcı adı zaten mevcut.
                    ModelState.AddModelError("Username", "This username is already taken.");
                    return View(registerModel);
                }

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

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                return BadRequest("Passwords do not match."); // Şifreler eşleşmiyor.
            }

            var applicationUser = await GetCurrentUserAsync();

            if (applicationUser == null)
            {
                return Unauthorized(); // Kullanıcı oturumu yoksa yetkilendirme hatası.
            }

            var removePasswordResult = await _signInManager.UserManager.RemovePasswordAsync(applicationUser);
            
            if (!removePasswordResult.Succeeded)
            {
                return BadRequest(removePasswordResult.Errors); // Şifre kaldırmada hata varsa döndür.
            }

            var addPasswordResult = await _signInManager.UserManager.AddPasswordAsync(applicationUser, password);
            
            if (!addPasswordResult.Succeeded)
            {
                return BadRequest(addPasswordResult.Errors); // Şifre eklemede hata varsa döndür.
            }

            TempData["PasswordChangedMessage"] = "Password changed successfully.";


            return RedirectToAction("Details", "Accounts");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync().Wait();
            return RedirectToAction("Login", "Accounts");
        }
    }
}
