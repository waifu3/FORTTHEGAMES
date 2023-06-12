using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FORTTHEGAMES.Models
{
    public class Usuario
    {
        [Key]
        public int id_usuario { get; set; }
        public string correo { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string rut { get; set; }
        public string direccion { get; set; }
        public string password { get; set; }
        public int? rol { get; set; }

        [NotMapped]
        public bool MantenerActivo { get; set; }

        [NotMapped]
        public string passwordRepeat { get; set; }

    }
}
