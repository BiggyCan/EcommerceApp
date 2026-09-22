using EcommerceApp.Data;
using EcommerceApp.Reports.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Reports.Queries
{
    public class ProductosMasVendidosQuery
    {
        private readonly ApplicationDbContext _context;


        public ProductosMasVendidosQuery(
            ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<ProductosMasVendidosReportModel> ExecuteAsync()
        {
            // ==========================================
            // CONSULTAR PRODUCTOS VENDIDOS
            // ==========================================

            var productos =
                await _context.OrderItems
                    .AsNoTracking()
                    .GroupBy(
                        item => new
                        {
                            item.ProductId,
                            item.ProductName
                        }
                    )
                    .Select(
                        grupo =>
                            new
                            {
                                ProductId =
                                    grupo.Key.ProductId,

                                Producto =
                                    grupo.Key.ProductName,

                                CantidadVendida =
                                    grupo.Sum(
                                        item => item.Quantity
                                    ),

                                TotalGenerado =
                                    grupo.Sum(
                                        item =>
                                            item.UnitPrice
                                            *
                                            item.Quantity
                                    ),

                                PrecioPromedio =
                                    grupo.Sum(
                                        item =>
                                            item.UnitPrice
                                            *
                                            item.Quantity
                                    )
                                    /
                                    grupo.Sum(
                                        item =>
                                            item.Quantity
                                    )
                            }
                    )
                    .OrderByDescending(
                        producto =>
                            producto.CantidadVendida
                    )
                    .ThenByDescending(
                        producto =>
                            producto.TotalGenerado
                    )
                    .ToListAsync();


            // ==========================================
            // CONVERTIR AL MODELO DEL REPORTE
            // ==========================================

            var resultado =
                productos
                    .Select(
                        (producto, index) =>
                            new ProductoMasVendidoItem
                            {
                                Posicion =
                                    index + 1,

                                ProductId =
                                    producto.ProductId,

                                Producto =
                                    producto.Producto,

                                CantidadVendida =
                                    producto.CantidadVendida,

                                PrecioPromedio =
                                    producto.PrecioPromedio,

                                TotalGenerado =
                                    producto.TotalGenerado
                            }
                    )
                    .ToList();


            // ==========================================
            // TOTALES
            // ==========================================

            var totalProductosDiferentes =
                resultado.Count;


            var totalUnidadesVendidas =
                resultado.Sum(
                    producto =>
                        producto.CantidadVendida
                );


            var totalIngresos =
                resultado.Sum(
                    producto =>
                        producto.TotalGenerado
                );


            // ==========================================
            // DEVOLVER MODELO
            // ==========================================

            return new ProductosMasVendidosReportModel
            {
                FechaGeneracion =
                    DateTime.UtcNow
                        .AddHours(-4),

                TotalProductosDiferentes =
                    totalProductosDiferentes,

                TotalUnidadesVendidas =
                    totalUnidadesVendidas,

                TotalIngresos =
                    totalIngresos,

                Productos =
                    resultado
            };
        }
    }
}