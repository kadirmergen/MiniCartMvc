using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniCartMvc.Data;
using MiniCartMvc.Entity;
using MiniCartMvc.Identity;
using MiniCartMvc.Models;
using MiniCartMvc.ViewModels;
using System.Security.Claims;
using static MiniCartMvc.Models.OrderDetailsModel;
using static NuGet.Packaging.PackagingConstants;

namespace MiniCartMvc.Controllers
{
    //[Authorize(Roles = "admin")]
    public class OrdersController : Controller
    {
        private readonly DataContext _context;

        public OrdersController(DataContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult Index(string? search)
        {
            ViewData["IsAdminDashboard"] = true;

            string username = User.Identity.Name;

            if (User.IsInRole("admin"))
            {
                var orders = _context.Orders.Select(i => new OrderViewModel()
                {
                    Id = i.Id,
                    OrderNumber = i.OrderNumber,
                    OrderDate = i.OrderDate,
                    OrderState = i.OrderState,
                    Total = i.Total,
                    Count = i.OrderLines.Count
                }).OrderByDescending(i => i.OrderDate).ToList();

                // Arama işlemi
                if (!string.IsNullOrEmpty(search))
                {
                    string lowerSearch = search.ToLower();
                    // Arama sonuçlarını filtrele ve tekrar listeye dönüştür
                    orders = orders.Where(o => o.OrderNumber.ToLower().Contains(lowerSearch)).ToList();
                }

                return View(orders);
            }

            var ordersForCustomer = _context.Orders.Where(u => u.UserName == username).Select(o => new OrderViewModel()
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderDate = o.OrderDate,
                OrderState = o.OrderState,
                Total = o.Total
            }).OrderByDescending(a => a.OrderDate).ToList();

            return View(ordersForCustomer);
        }

        [Authorize]
        public ActionResult Details(int id)
        {
            var entity = _context.Orders.Where(i => i.Id == id).Select(i => new OrderDetailsModel()
            {
                OrderId = i.Id,
                UserName = i.UserName,
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

            ViewBag.OrderStates = Enum.GetValues(typeof(EnumOrderState))
                              .Cast<EnumOrderState>()
                              .Select(e => new SelectListItem
                              {
                                  Value = ((int)e).ToString(),
                                  Text = e.ToString()
                              }).ToList();

            return View(entity);
        }

        [HttpPost]
        public IActionResult RateProducts(int orderId, Dictionary<int, int> ratings)
        {
            var userName = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.UserName == userName);

            if (user == null)
            {
                TempData["UserNotFoundError"] = "User not found in the system.";
                return RedirectToAction("Details", new { id = orderId });
            }

            var userId = user.Id;

            // Siparişin tamamlanmış olduğundan emin olun
            var order = _context.Orders
                .Include(o => o.OrderLines)
                .FirstOrDefault(o => o.Id == orderId && o.UserName == userName);

            if (order == null || order.OrderState != EnumOrderState.Completed)
            {
                TempData["RateError"] = "You can only rate products from completed orders.";
                return RedirectToAction("Details", new { id = orderId });
            }

            foreach (var rating in ratings)
            {
                var productId = rating.Key;
                var score = rating.Value;
                // Daha önce oy verilmiş mi kontrol et
                var existingRating = _context.Ratings.FirstOrDefault(r => r.ProductId == productId && r.UserId == userId);
                if (existingRating != null)
                {
                    existingRating.Score = score; // Mevcut oyu güncelle
                }
                else
                {
                    // Yeni oy ekle
                    _context.Ratings.Add(new Rating
                    {
                        ProductId = productId,
                        UserId = userId,
                        Score = score
                    });
                }
            }


            _context.SaveChanges();

            TempData["RateSuccess"] = "Your ratings have been submitted.";
            return RedirectToAction("Details", new { id = orderId });
        }

        public ActionResult UpdateOrderState(int orderId, EnumOrderState orderState)
        {
            var order = _context.Orders.FirstOrDefault(i => i.Id == orderId);

            if (order != null)
            {
                order.OrderState = orderState;
                _context.SaveChanges();

                TempData["message"] = "Orders  information has been saved!";

                return RedirectToAction("Details", new { id = orderId });
            }
            return RedirectToAction("Index");
        }
    }
}
