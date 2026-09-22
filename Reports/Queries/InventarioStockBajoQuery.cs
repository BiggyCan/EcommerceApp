using EcommerceApp.Data;
using EcommerceApp.Reports.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Reports.Queries
{
    public class InventarioStockBajoQuery
    {
        private readonly ApplicationDbContext _context;


        public InventarioStockBajoQuery(
            ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<InventarioStockBajoReportModel> ExecuteAsync(
            int limiteStockBajo = 10)
        {
            // ==========================================
            // VALIDAR LÍMITE
            // ==========================================

            if (limiteStockBajo < 0)
            {
                limiteStockBajo = 10;
            }


            // ==========================================
            // CONSULTAR TODOS LOS PRODUCTOS
            // ==========================================

            var todosLosProductos =
                await _context.Products
                    .AsNoTracking()
                    .OrderBy(
                        producto =>
                            producto.Stock
                    )
                    .ThenBy(
                        producto =>
                            producto.Name
                    )
                    .ToListAsync();


            // ==========================================
            // PRODUCTOS CON STOCK BAJO
            // ==========================================

            var productosStockBajo =
                todosLosProductos
                    .Where(
                        producto =>
                            producto.Stock <= limiteStockBajo
                    )
                    .Select(
                        producto =>
                            new InventarioStockBajoItem
                            {
                                ProductId =
                                    producto.Id,

                                Producto =
                                    producto.Name,

                                Categoria =
                                    string.IsNullOrWhiteSpace(
                                        producto.Category
                                    )
                                        ? "Sin categoría"
                                        : producto.Category,

                                Precio =
                                    producto.Price,

                                Stock =
                                    producto.Stock,

                                ValorStock =
                                    producto.Price
                                    *
                                    producto.Stock,

                                Estado =
                                    ObtenerEstado(
                                        producto.Stock
                                    )
                            }
                    )
                    .OrderBy(
                        producto =>
                            producto.Stock
                    )
                    .ThenBy(
                        producto =>
                            producto.Producto
                    )
                    .ToList();


            // ==========================================
            // INDICADORES GENERALES
            // ==========================================

            var totalProductosAnalizados =
                todosLosProductos.Count;


            var productosConStockBajo =
                todosLosProductos.Count(
                    producto =>
                        producto.Stock > 0
                        &&
                        producto.Stock <= limiteStockBajo
                );


            var productosAgotados =
                todosLosProductos.Count(
                    producto =>
                        producto.Stock <= 0
                );


            // ==========================================
            // EXISTENCIAS DE LOS PRODUCTOS EN ALERTA
            // ==========================================

            var unidadesDisponibles =
                productosStockBajo.Sum(
                    producto =>
                        producto.Stock
                );


            // ==========================================
            // VALOR DEL INVENTARIO EN ALERTA
            // ==========================================

            var valorInventario =
                productosStockBajo.Sum(
                    producto =>
                        producto.ValorStock
                );


            // ==========================================
            // DEVOLVER MODELO
            // ==========================================

            return new InventarioStockBajoReportModel
            {
                FechaGeneracion =
                    DateTime.UtcNow
                        .AddHours(-4),

                LimiteStockBajo =
                    limiteStockBajo,

                TotalProductosAnalizados =
                    totalProductosAnalizados,

                ProductosConStockBajo =
                    productosConStockBajo,

                ProductosAgotados =
                    productosAgotados,

                UnidadesDisponibles =
                    unidadesDisponibles,

                ValorInventario =
                    valorInventario,

                Productos =
                    productosStockBajo
            };
        }


        // ==========================================
        // ESTADO DEL INVENTARIO
        // ==========================================

        private static string ObtenerEstado(
            int stock)
        {
            if (stock <= 0)
            {
                return "Agotado";
            }


            if (stock <= 3)
            {
                return "Crítico";
            }


            if (stock <= 5)
            {
                return "Muy bajo";
            }


            return "Stock bajo";
        }
    }
}