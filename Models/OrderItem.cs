using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FORTTHEGAMES.Models
{
    public class OrderItem
    {
        [Key]
        public int id { get; set; }

        public int cantidad { get; set; }
        public double price { get; set; }


        public int id_order { get; set; }
        [ForeignKey("id_order")]
        public virtual Order Order { get; set; }


        public int id_producto { get; set; }
        [ForeignKey("id_producto")]
        public virtual Producto Producto { get; set; }


    }
}
