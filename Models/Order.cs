using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FORTTHEGAMES.Models
{
    public class Order
    {
        [Key]
        public int id { get; set; }

        [ForeignKey("Usuario")]
        public int id_usuario { get; set; }
        public virtual Usuario Usuario { get; set; }

        public double total { get; set; }

        public string id_shoppingitem { get; set; }

        public DateTime? fecha { get; set; }

        public List<OrderItem> OrderItems { get; set; }



    }
}
