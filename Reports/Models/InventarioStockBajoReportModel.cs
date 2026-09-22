namespace EcommerceApp.Reports.Models
{
    public class InventarioStockBajoReportModel
    {
        public DateTime FechaGeneracion { get; set; }

        public int LimiteStockBajo { get; set; }

        public int TotalProductosAnalizados { get; set; }

        public int ProductosConStockBajo { get; set; }

        public int ProductosAgotados { get; set; }

        public int UnidadesDisponibles { get; set; }

        public decimal ValorInventario { get; set; }

        public List<InventarioStockBajoItem> Productos { get; set; }
            = new List<InventarioStockBajoItem>();
    }


    public class InventarioStockBajoItem
    {
        public int ProductId { get; set; }

        public string Producto { get; set; }
            = string.Empty;

        public string Categoria { get; set; }
            = string.Empty;

        public decimal Precio { get; set; }

        public int Stock { get; set; }

        public decimal ValorStock { get; set; }

        public string Estado { get; set; }
            = string.Empty;
    }
}