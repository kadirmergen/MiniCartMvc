using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniCartMvc.Data;
using MiniCartMvc.Entity;
using MiniCartMvc.Models;
using static MiniCartMvc.Models.OrderDetailsModel;

namespace MiniCartMvc.Controllers
{
    [Authorize(Roles = "admin")]
    public class OrdersController : Controller
    {
        private readonly DataContext _context;

        public OrdersController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var orders = _context.Orders.Select(i => new AdminOrderModel()
            {
                Id = i.Id,
                OrderNumber = i.OrderNumber,
                OrderDate = i.OrderDate,
                OrderState = i.OrderState,
                Total = i.Total,
                Count = i.OrderLines.Count
            }).OrderByDescending(i => i.OrderDate).ToList();

            return View(orders);
        }

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
