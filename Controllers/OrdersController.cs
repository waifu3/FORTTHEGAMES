using FORTTHEGAMES.Data;
using FORTTHEGAMES.Data.Cart;
using FORTTHEGAMES.Data.ViewModels;
using FORTTHEGAMES.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FORTTHEGAMES.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ShoppingCart _shoppingCart;
        private readonly FORTTHEGAMESContext _context;
        public IConfiguration _configuration;
        public OrdersController(ShoppingCart shoppingCart, FORTTHEGAMESContext context, IConfiguration configuration)
        {

            _shoppingCart = shoppingCart;
            _context = context;
            _configuration = configuration;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            string correo = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Usuario us = await _context.Usuario.FirstOrDefaultAsync(u => u.correo == correo);

            //int usuarioid = int.Parse(userId);

            var orders = await _context.Order.Include(n => n.OrderItems).ThenInclude(n => n.Producto).Include(n => n.Usuario).Where(u=>u.Usuario.id_usuario == us.id_usuario).ToListAsync();


            return View(orders);
        }


        public async Task<IActionResult> AddItemToShoppingCart(int id)
        {
            //var item = await _moviesService.GetMovieByIdAsync(id);
            var item = _context.Producto.FirstOrDefault(p => p.id_producto == id);

            if (item != null)
            {
                _shoppingCart.AddItemToCart(item);
            }
            return RedirectToAction("Index", "Tienda");
        }

        public IActionResult ShoppingCart()
        {
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            var response = new ShoppingCartVM()
            {
                ShoppingCart = _shoppingCart,
                ShoppingCartTotal = _shoppingCart.GetShoppingCartTotal()
            };

            return View(response);
        }

        public async Task<IActionResult> RemoveItemFromShoppingCart(int id)
        {
            //var item = await _moviesService.GetMovieByIdAsync(id);
            var item = _context.Producto.FirstOrDefault(p => p.id_producto == id);

            if (item != null)
            {
                _shoppingCart.RemoveItemFromCart(item);
            }
            return RedirectToAction(nameof(ShoppingCart));
        }



    }
}
