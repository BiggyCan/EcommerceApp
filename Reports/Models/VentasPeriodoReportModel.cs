namespace EcommerceApp.Reports.Models
{
    public class VentasPeriodoReportModel
    {
        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public DateTime FechaGeneracion { get; set; }

        public int TotalPedidos { get; set; }

        public int TotalProductosVendidos { get; set; }

        public decimal TotalVentas { get; set; }

        public decimal PromedioVenta { get; set; }

        public List<VentaPeriodoItem> Ventas { get; set; }
            = new List<VentaPeriodoItem>();
    }


    public class VentaPeriodoItem
    {
        public int PedidoId { get; set; }

        public DateTime Fecha { get; set; }

        public string Cliente { get; set; }
            = string.Empty;

        public string Correo { get; set; }
            = string.Empty;

        public string Estado { get; set; }
            = string.Empty;

        public int CantidadProductos { get; set; }

        public decimal Total { get; set; }
    }
}