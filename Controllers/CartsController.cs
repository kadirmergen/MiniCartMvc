using Microsoft.AspNetCore.Mvc;
using MiniCartMvc.Data;
using MiniCartMvc.Entity;
using MiniCartMvc.Extensions;
using MiniCartMvc.Models;

namespace MiniCartMvc.Controllers
{
    public class CartsController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataContext _dataContext;

        public CartsController(IHttpContextAccessor httpContextAccessor, DataContext dataContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            ViewData["IsAdminDashboard"] = false;

            var cart = GetCart();
            ViewData["Cart"] = cart;  // ViewData'ya güncel cart'ı ekle
            return View(cart);
        }

        public IActionResult AddToCart(int Id)
        {
            Product product = _dataContext.Products.Where(i => i.Id == Id).FirstOrDefault();
            if (product != null)
            {
                var cart = GetCart();
                cart.AddProduct(product, 1);
                SaveCart(cart);
            }
            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int Id)
        {
            Product product = _dataContext.Products.Where(i => i.Id == Id).FirstOrDefault();
            if (product != null)
            {
                var cart = GetCart();
                cart.DeleteProduct(product);
                SaveCart(cart);
            }

            return RedirectToAction("Index");
        }

        public Cart GetCart()
        {
            var session = _httpContextAccessor.HttpContext.Session;

            // Session'dan Cart nesnesini al

            if (string.IsNullOrEmpty(_httpContextAccessor.HttpContext.User.Identity.Name))
            {
                throw new InvalidOperationException("Kullanıcı kimliği alınamadı.");
            }

            // Kullanıcıya özel session anahtarı oluştur
            var cartKey = $"Cart_{_httpContextAccessor.HttpContext.User.Identity.Name}";

            var cart = session.GetObjectFromJson<Cart>(cartKey);

            // Eğer null ise yeni bir Cart oluştur ve session'a kaydet
            if (cart == null)
            {
                cart = new Cart();
                session.SetObjectAsJson("Cart", cart);
            }
            return cart;
        }
        public void SaveCart(Cart cart)
        {
            var session = _httpContextAccessor.HttpContext.Session;

            // Kullanıcı adı doğrudan alınıyor
            if (string.IsNullOrEmpty(_httpContextAccessor.HttpContext.User.Identity.Name))
            {
                throw new InvalidOperationException("Kullanıcı kimliği alınamadı.");
            }

            // Kullanıcıya özel session anahtarı oluştur
            var cartKey = $"Cart_{_httpContextAccessor.HttpContext.User.Identity.Name}";

            // Cart nesnesini session'a kaydet
            session.SetObjectAsJson(cartKey, cart);
        }

        public IActionResult Summary()
        {
            var cart = GetCart();
            SaveCart(cart);
            ViewData["Cart"] = cart;
            return PartialView("_Summary");
        }

        public ActionResult Checkout()
        {
            return View(new ShippingDetail());
        }

        [HttpPost]
        public ActionResult Checkout(ShippingDetail shippingDetail)
        {
            var cart = GetCart();
            if (cart.CartLines.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "There isn't any product in your cart");
                return View(shippingDetail);
            }
            if (User.Identity.IsAuthenticated)
            {
                shippingDetail.UserName = User.Identity.Name; // Giriş yapmış kullanıcı adı atanıyor
            }
            else
            {
                ModelState.AddModelError(string.Empty, "You need to log in before checkout.");
                return View(shippingDetail);
            }
            if (ModelState.IsValid)
            {
                SaveOrder(cart, shippingDetail);
                cart.Clear();
                return View("Completed");
            }
            else 
            {
                return View(shippingDetail);
            }
        }

        private void SaveOrder(Cart cart, ShippingDetail shippingDetail)
        {
            var order = new Order();
            order.OrderNumber = "A" + (new Random()).Next(11111, 99999).ToString(); //random sipariş numarası üretiyor
            order.Total = cart.Total();
            order.OrderDate = DateTime.Now;
            order.OrderState = EnumOrderState.Waiting;

            order.UserName = shippingDetail.UserName;
            order.AddressTitle = shippingDetail.AddressTitle;
            order.Address = shippingDetail.Address;
            order.City = shippingDetail.City;
            order.Strict = shippingDetail.Strict;
            order.Street = shippingDetail.Street;
            order.ZipCode = shippingDetail.ZipCode;
            

            order.OrderLines = new List<OrderLine>();

            foreach (var product in cart.CartLines)
            {
                var orderLine = new OrderLine();
                orderLine.Quantity = product.Quantity;
                orderLine.Price = product.Quantity * product.Product.Price;
                orderLine.ProductId = product.Product.Id;

                order.OrderLines.Add(orderLine);

            }

            _dataContext.Orders.Add(order);
            _dataContext.SaveChanges();
        }
    }
}
