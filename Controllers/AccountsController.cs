using Microsoft.AspNetCore.Identity;
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
        private readonly IdentityDataContext _identityContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly DataContext _context;

        public AccountsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, IdentityDataContext identityDataContext, DataContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _identityContext = identityDataContext;
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

        [Authorize]
        public ActionResult Details(int id)
        {
            var entity = _context.Orders.Where(i => i.Id == id).Select(i => new OrderDetailsModel()
            {
                OrderId = i.Id,
                OrderNumber = i.OrderNumber,
                Total = i.Total,
                OrderDate = i.OrderDate,
                OrderState = i.OrderState,
                AddressTitle = i.AddressTitle,
                Address = i.Address,
                City = i.City,
                Street = i.Street,
                Strict = i.Strict,
                ZipCode = i.ZipCode,
                OrderLines = i.OrderLines.Select(a => new OrderLineModel()
                {
                    ProductId = a.ProductId,
                    ProductName = a.Product.Name,
                    Image = a.Product.ImagePath,
                    Quantity = a.Quantity,
                    Price = a.Price,
                }).ToList()
            }).FirstOrDefault();

            return View(entity);
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
