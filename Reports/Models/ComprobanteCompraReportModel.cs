namespace EcommerceApp.Reports.Models
{
    public class ComprobanteCompraReportModel
    {
        public int PedidoId { get; set; }

        public DateTime FechaPedido { get; set; }

        public DateTime FechaGeneracion { get; set; }

        public string Cliente { get; set; }
            = string.Empty;

        public string Correo { get; set; }
            = string.Empty;

        public string Direccion { get; set; }
            = string.Empty;

        public string Estado { get; set; }
            = string.Empty;

        public int TotalUnidades { get; set; }

        public decimal TotalCompra { get; set; }

        public List<ComprobanteCompraItem> Productos { get; set; }
            = new List<ComprobanteCompraItem>();
    }


    public class ComprobanteCompraItem
    {
        public int ProductId { get; set; }

        public string Producto { get; set; }
            = string.Empty;

        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal { get; set; }
    }
}