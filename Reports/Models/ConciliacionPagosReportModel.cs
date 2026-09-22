namespace EcommerceApp.Reports.Models
{
    public class ConciliacionPagosReportModel
    {
        public DateTime FechaGeneracion { get; set; }

        public decimal PorcentajeComision { get; set; }

        public int TotalPedidos { get; set; }

        public decimal TotalBruto { get; set; }

        public decimal TotalComisiones { get; set; }

        public decimal TotalNeto { get; set; }

        public List<ConciliacionPagoItem> Movimientos { get; set; }
            = new List<ConciliacionPagoItem>();
    }


    public class ConciliacionPagoItem
    {
        public int PedidoId { get; set; }

        public string Cliente { get; set; }
            = string.Empty;

        public string Correo { get; set; }
            = string.Empty;

        public DateTime Fecha { get; set; }

        public string EstadoPedido { get; set; }
            = string.Empty;

        public string EstadoConciliacion { get; set; }
            = "Calculado";

        public decimal ImporteBruto { get; set; }

        public decimal Comision { get; set; }

        public decimal ImporteNeto { get; set; }
    }
}