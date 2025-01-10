using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MiniCartMvc.Data;
using MiniCartMvc.Entity;
using MiniCartMvc.Identity;
using MiniCartMvc.Models;
using MiniCartMvc.ViewModels;
using System.Diagnostics;
using System.Security.Claims;

namespace MiniCartMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _context;


        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["IsAdminDashboard"] = false;

            var randomProducts = _context.Categories.OrderBy(c => Guid.NewGuid())
                .Select(c => c.Products!.OrderBy(p => Guid.NewGuid()).Select(i => new ProductViewModel()
                {
                    Id = i.Id,
                    Name = i.Name.Length > 50 ? i.Name.Substring(0, 47) + "..." : i.Name,
                    Description = i.Description.Length > 50 ? i.Description.Substring(0, 47) + "..." : i.Description,
                    Price = i.Price,
                    Stock = i.Stock,
                    Image = i.ImagePath,
                    CategoryId = i.CategoryId
                }).FirstOrDefault()).ToList();

            return View(randomProducts);
        }

        public IActionResult Details(int id)
        {
            var product = _context.Products.Include(p => p.Comments).Include(p => p.Ratings).Where(i => i.Id == id).FirstOrDefault();
            if (product == null)
            {
                return NotFound();
            }

            var userIds = product.Comments.Select(c => c.UserId).Distinct().ToList();
            var users = _context.Users.Where(u => userIds.Contains(u.Id)).ToDictionary(u => u.Id, u => $"{u.Name} {u.Surname}");

            var avarageRating = product.Ratings.Any() ? product.Ratings.Average(r => r.Score) : 0;

            var productDetailsViewModel = new ProductDetailsViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Image = product.ImagePath,
                AverageRating = avarageRating,
                Comments = product.Comments.OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentViewModel
                {
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    Username = users.ContainsKey(c.UserId) ? users[c.UserId] : "Anonymous"
                }).ToList()
            };

            return View(productDetailsViewModel);
        }

        public IActionResult List(int? id)
        {
            var product = _context.Products.Select(i => new ProductViewModel()
            {
                Id = i.Id,
                Name = i.Name.Length > 50 ? i.Name.Substring(0, 47) + "..." : i.Name,
                Description = i.Description.Length > 50 ? i.Description.Substring(0, 47) + "..." : i.Description,
                Price = i.Price,
                Stock = i.Stock,
                Image = i.ImagePath,
                CategoryId = i.CategoryId
            }).AsQueryable();
            if (id != null)
            {
                product = product.Where(i => i.CategoryId == id);
            }


            var categories = _context.Categories.ToList();
            ViewData["Categories"] = categories;

            var productList = product.ToList();

            if (!productList.Any())
            {
                return View();
            }
            return View(productList);
        }

        [HttpPost]
        //[Authorize]
        public IActionResult AddComment(int productId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["CommentNullError"] = "You have to write something to make comment.";
                return RedirectToAction("Details", new { id = productId });
            }

            var userName = User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(userName))
            {
                TempData["AddCommentLoginError"] = "You must be logged in to add a comment.";
                return RedirectToAction("Details", new { id = productId });
            }
            var hasPurchased = _context.Orders.Where(o => o.UserName == userName && o.OrderState == EnumOrderState.Completed)
            .SelectMany(o => o.OrderLines)
            .Any(ol => ol.ProductId == productId);

            if (!hasPurchased)
            {
                TempData["NotPurchasedError"] = "You can only comment on products you have purchased.";
                return RedirectToAction("Details", new { id = productId });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var comment = new Comment
            {
                Content = content,
                ProductId = productId,
                UserId = userId
            };
            _context.Comments.Add(comment);
            _context.SaveChanges();
            return RedirectToAction("Details", new { id = productId });
        }

        /*[HttpPost]
        public IActionResult RateProduct(int productId, int score)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                TempData["UserNotFoundError"] = "You must be logged in to rate a product.";
                return RedirectToAction("Details", new { id = productId });
            }

            var hasPurchased = _context.Orders
                .Include(o => o.OrderLines)
                .Any(o => o.OrderLines.Any(ol => ol.ProductId == productId) &&
                    o.UserName == userId &&
                    o.OrderState == EnumOrderState.Completed);

            if (!hasPurchased)
            {
                TempData["RateError"] = "You can only rate products you have purchased.";
                return RedirectToAction("Details", new { id = productId });
            }

            var existingRating = _context.Ratings.FirstOrDefault(r => r.ProductId == productId && r.UserId == userId);

            if (existingRating != null)
            {
                existingRating.Score = score;
            }
            else
            {
                var rating = new Rating
                {
                    ProductId = productId,
                    UserId = userId,
                    Score = score
                };
                _context.Ratings.Add(rating);
            }
            _context.SaveChanges();

            TempData["RateSuccess"] = "Your rating has been submitted.";
            return RedirectToAction("Details", new { id = productId });
        }*/



        /*public IActionResult GetCategories()
        {
            var categories = _context.Categories.ToList();
            if (categories == null || !categories.Any())
            {
                categories = new List<Category>();
            }
            //ViewData["Categories"] = categories;
            return View("GetCategories", categories);
        }*/

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
