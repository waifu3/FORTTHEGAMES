using System.ComponentModel.DataAnnotations;

namespace FORTTHEGAMES.Models
{
    public class Producto
    {
        [Key]
        public int id_producto { get; set; }
        public string nombre { get; set; }
        public string? imagen { get; set; }
        public double valor { get; set; }

        public string descripcion { get; set; }
        public int? estado { get; set; }
        public int id_categoria { get; set; }
    }
}
