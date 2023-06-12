using System.ComponentModel.DataAnnotations;

namespace FORTTHEGAMES.Models
{
    public class Payment
    {
        [Key]
        public int id { get; set; }

        public Decimal amount { get; set; }

        public string transaction_id { get; set; }

        public string? notification_token { get; set; }

        public string? status { get; set; }

        public string? status_detail { get; set; }

        public int id_unique_service { get; set; }


    }
}
