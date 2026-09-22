namespace EcommerceApp.Reports.Models
{
    public class ProductosMasVendidosReportModel
    {
        public DateTime FechaGeneracion { get; set; }

        public int TotalProductosDiferentes { get; set; }

        public int TotalUnidadesVendidas { get; set; }

        public decimal TotalIngresos { get; set; }

        public List<ProductoMasVendidoItem> Productos { get; set; }
            = new List<ProductoMasVendidoItem>();
    }


    public class ProductoMasVendidoItem
    {
        public int Posicion { get; set; }

        public int ProductId { get; set; }

        public string Producto { get; set; }
            = string.Empty;

        public int CantidadVendida { get; set; }

        public decimal PrecioPromedio { get; set; }

        public decimal TotalGenerado { get; set; }
    }
}