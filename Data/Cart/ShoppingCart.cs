using FORTTHEGAMES.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace FORTTHEGAMES.Data.Cart
{
    public class ShoppingCart
    {
        public FORTTHEGAMESContext _context;

        public string ShoppingCartId { get; set; }
        public List<ShoppingCartItems> ShoppingCartItems { get; set; }

        public ShoppingCart(FORTTHEGAMESContext context)
        {
            _context = context;
        }

        public static ShoppingCart GetShoppingCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
            var context = services.GetService<FORTTHEGAMESContext>();

            string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();
            session.SetString("CartId", cartId);

            return new ShoppingCart(context) { ShoppingCartId = cartId };
        }

        public void AddItemToCart(Producto producto)
        {
            var shoppingCartItem = _context.ShoppingCartItems.FirstOrDefault(n => n.Producto.id_producto == producto.id_producto && n.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItems()
                {
                    ShoppingCartId = ShoppingCartId,
                    Producto = producto,
                    monto = 1
                };

                _context.ShoppingCartItems.Add(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.monto++;
            }
            _context.SaveChanges();
        }

        public void RemoveItemFromCart(Producto producto)
        {
            var shoppingCartItem = _context.ShoppingCartItems.FirstOrDefault(n => n.Producto.id_producto == producto.id_producto && n.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem != null)
            {
                if (shoppingCartItem.monto > 1)
                {
                    shoppingCartItem.monto--;
                }
                else
                {
                    _context.ShoppingCartItems.Remove(shoppingCartItem);
                }
            }
            _context.SaveChanges();
        }

        public List<ShoppingCartItems> GetShoppingCartItems()
        {
            var test = ShoppingCartId;
            return ShoppingCartItems ?? (ShoppingCartItems = _context.ShoppingCartItems.Where(n => n.ShoppingCartId == ShoppingCartId).Include(n => n.Producto).ToList());
        }

        public double GetShoppingCartTotal() => _context.ShoppingCartItems.Where(n => n.ShoppingCartId == ShoppingCartId).Select(n => n.Producto.valor * n.monto).Sum();

        public async Task ClearShoppingCartAsync()
        {
            var items = await _context.ShoppingCartItems.Where(n => n.ShoppingCartId == ShoppingCartId).ToListAsync();
            _context.ShoppingCartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }
}
