using FORTTHEGAMES.Data;
using FORTTHEGAMES.Data.Cart;
using FORTTHEGAMES.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FORTTHEGAMES.Controllers
{
    public class TiendaController : Controller
    {
        private readonly FORTTHEGAMESContext _context;
        private readonly ShoppingCart _shoppingCart;

        public TiendaController(FORTTHEGAMESContext context, ShoppingCart shoppingCart)
        {
            _context = context;
            _shoppingCart = shoppingCart;
        }
        public async Task<IActionResult> Index()
        {




            var productos = _context.Producto.ToList();
            if (productos != null)
            {
                var carro = _shoppingCart.GetShoppingCartItems();
                if (carro.Count > 0)
                {
                    foreach (var item in carro)
                    {
                        productos.Remove(item.Producto);

                    }
                }

                return View(productos);
            }
            else
            {
                return Problem("Entity set 'FORTTHEGAMESContext.Producto'  is null.");
            }

            //return _context.Producto != null ?
            //              View(await _context.Producto.ToListAsync()) :
            //              Problem("Entity set 'FORTTHEGAMESContext.Producto'  is null.");
        }

    }
}
