using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FORTTHEGAMES.Models;

namespace FORTTHEGAMES.Data
{
    public class FORTTHEGAMESContext : DbContext
    {
        public FORTTHEGAMESContext (DbContextOptions<FORTTHEGAMESContext> options)
            : base(options)
        {
        }

        public DbSet<FORTTHEGAMES.Models.Usuario> Usuario { get; set; } = default!;

        public DbSet<FORTTHEGAMES.Models.Producto>? Producto { get; set; }

        public DbSet<FORTTHEGAMES.Models.ShoppingCartItems> ShoppingCartItems { get; set; }

        public DbSet<FORTTHEGAMES.Models.Payment> Payment { get; set; }

        public DbSet<FORTTHEGAMES.Models.Payment_Detail> Payment_Detail { get; set; }

        public DbSet<FORTTHEGAMES.Models.Order> Order { get; set; }

        public DbSet<FORTTHEGAMES.Models.OrderItem> OrderItem { get; set; }
    }
}
