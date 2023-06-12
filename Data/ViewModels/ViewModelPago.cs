namespace FORTTHEGAMES.Data.ViewModels
{
    public class ViewModelPago
    {
        public string num_solicitud { get; set; }
        public string cliente { get; set; }
        public DateTime fecha { get; set; }
        public double monto { get; set; }
        public string token { get; set; }
        public string body { get; set; }
        public string subject { get; set; }
        public string transaction_id { get; set; }

    }
}
