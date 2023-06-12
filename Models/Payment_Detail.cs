using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FORTTHEGAMES.Models
{
    public class Payment_Detail
    {
        [Key]
        public int id_payment_detail { get; set; }

        [ForeignKey("Payment")]
        public int id_payment { get; set; }
        public virtual Payment Payment { get; set; }

        public string? subject { get; set; }

        public string? body { get; set; }

        public DateTime? conciliation_date { get; set; }

        public string? url_cobro { get; set; }

        public string? url_comprobante { get; set; }

        public DateTime? expires_date { get; set; }

        public string? bank_id { get; set; }

        public string? nombre_pagador { get; set; }

        public string? email_pagador { get; set; }

        public string payment_url { get; set; }

        public string? reciber_id { get; set; }

        public string payment_id { get; set; }
    }
}
