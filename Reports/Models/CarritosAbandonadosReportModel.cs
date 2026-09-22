namespace EcommerceApp.Reports.Models
{
    public class CarritosAbandonadosReportModel
    {
        public DateTime FechaGeneracion { get; set; }

        public int MinutosAbandono { get; set; }

        public int TotalCarritos { get; set; }

        public int TotalUnidades { get; set; }

        public decimal ValorTotalAbandonado { get; set; }

        public List<CarritoAbandonadoItem> Carritos { get; set; }
            = new List<CarritoAbandonadoItem>();
    }


    public class CarritoAbandonadoItem
    {
        public int CarritoId { get; set; }

        public string Usuario { get; set; }
            = string.Empty;

        public string Correo { get; set; }
            = string.Empty;

        public int TotalItems { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastActivityAt { get; set; }

        public double MinutosSinActividad { get; set; }

        public string Estado { get; set; }
            = string.Empty;

        public string Productos { get; set; }
            = string.Empty;
    }
}