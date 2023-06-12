using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FORTTHEGAMES.Models
{
    public class ShoppingCartItems
    {
        [Key]
        public int id { get; set; }

        [ForeignKey("Producto")]
        public int id_producto { get; set; }
        public virtual Producto Producto { get; set; }

        public int monto { get; set; }


        public string ShoppingCartId { get; set; }
    }
}
