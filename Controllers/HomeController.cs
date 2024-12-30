using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MiniCartMvc.Data;
using MiniCartMvc.Entity;
using MiniCartMvc.ViewModels;
using System.Diagnostics;

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
                    Id =i.Id,
                    Name =i.Name.Length > 50 ? i.Name.Substring(0, 47) + "..." : i.Name,
                    Description = i.Description.Length>50?i.Description.Substring(0,47 ) + "..." : i.Description,
                    Price = i.Price,
                    Stock = i.Stock,
                    Image = i.ImagePath,
                    CategoryId = i.CategoryId
                }).FirstOrDefault()).ToList();

            return View(randomProducts);
        }

        public IActionResult Details(int id)
        {
            var product = _context.Products.Where(i => i.Id == id).FirstOrDefault();
            if (product == null) 
            {
                return NotFound();
            }
            return View(product);
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
