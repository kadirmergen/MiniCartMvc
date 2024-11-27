using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MiniCartMvc.Data;
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
            return View(GetCart());
        }

        public IActionResult AddToCart(int Id)
        {
            var product = _dataContext.Products.Where(i => i.Id == Id).FirstOrDefault();
            if (product != null) 
            {
                GetCart().AddProduct(product,1);
            }

            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int Id)
        {
            var product = _dataContext.Products.Where(i => i.Id == Id).FirstOrDefault();
            if (product != null)
            {
                GetCart().DeleteProduct(product);
            }

            return RedirectToAction("Index");
        }

        public Cart GetCart()
        {
            var session = _httpContextAccessor.HttpContext.Session;

            // Session'dan Cart nesnesini al
            var cart = session.GetObjectFromJson<Cart>("Cart");

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
            session.SetObjectAsJson("Cart", cart);
        }
    }
}
