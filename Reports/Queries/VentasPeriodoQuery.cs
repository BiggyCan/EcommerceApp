using EcommerceApp.Data;
using EcommerceApp.Reports.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Reports.Queries
{
    public class VentasPeriodoQuery
    {
        private readonly ApplicationDbContext _context;


        public VentasPeriodoQuery(
            ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<VentasPeriodoReportModel> ExecuteAsync(
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            // ==========================================
            // FECHAS DEL REPORTE
            // ==========================================
            //
            // El usuario selecciona fechas correspondientes
            // al horario de Bolivia (UTC-4).
            //
            // PostgreSQL guarda CreatedAt como
            // timestamp with time zone, por lo que Npgsql
            // exige DateTime con Kind = UTC.
            // ==========================================


            var inicioBolivia =
                fechaInicio.Date;


            var finBoliviaExclusivo =
                fechaFin.Date
                    .AddDays(1);


            // ==========================================
            // CONVERTIR BOLIVIA UTC-4 A UTC
            // ==========================================

            var inicioUtc =
                DateTime.SpecifyKind(
                    inicioBolivia
                        .AddHours(4),
                    DateTimeKind.Utc
                );


            var finUtc =
                DateTime.SpecifyKind(
                    finBoliviaExclusivo
                        .AddHours(4),
                    DateTimeKind.Utc
                );


            // ==========================================
            // CONSULTAR PEDIDOS DEL PERÍODO
            // ==========================================

            var pedidos =
                await _context.Orders
                    .AsNoTracking()
                    .Include(
                        o => o.Items
                    )
                    .Where(
                        o =>
                            o.CreatedAt >= inicioUtc
                            &&
                            o.CreatedAt < finUtc
                    )
                    .OrderByDescending(
                        o => o.CreatedAt
                    )
                    .ToListAsync();


            // ==========================================
            // CONVERTIR DATOS AL MODELO DEL REPORTE
            // ==========================================

            var ventas =
                pedidos
                    .Select(
                        pedido =>
                            new VentaPeriodoItem
                            {
                                PedidoId =
                                    pedido.Id,


                                // La BD está en UTC.
                                // Para mostrarlo en el reporte,
                                // regresamos a hora Bolivia.
                                Fecha =
                                    pedido.CreatedAt
                                        .ToUniversalTime()
                                        .AddHours(-4),


                                Cliente =
                                    pedido.CustomerName,


                                Correo =
                                    pedido.CustomerEmail,


                                Estado =
                                    pedido.Status,


                                CantidadProductos =
                                    pedido.Items.Sum(
                                        item =>
                                            item.Quantity
                                    ),


                                Total =
                                    pedido.TotalAmount
                            }
                    )
                    .ToList();


            // ==========================================
            // TOTALES
            // ==========================================

            var totalPedidos =
                pedidos.Count;


            var totalProductosVendidos =
                pedidos.Sum(
                    pedido =>
                        pedido.Items.Sum(
                            item =>
                                item.Quantity
                        )
                );


            var totalVentas =
                pedidos.Sum(
                    pedido =>
                        pedido.TotalAmount
                );


            var promedioVenta =
                totalPedidos > 0
                    ? totalVentas / totalPedidos
                    : 0m;


            // ==========================================
            // DEVOLVER MODELO
            // ==========================================

            return new VentasPeriodoReportModel
            {
                FechaInicio =
                    fechaInicio.Date,


                FechaFin =
                    fechaFin.Date,


                FechaGeneracion =
                    DateTime.UtcNow
                        .AddHours(-4),


                TotalPedidos =
                    totalPedidos,


                TotalProductosVendidos =
                    totalProductosVendidos,


                TotalVentas =
                    totalVentas,


                PromedioVenta =
                    promedioVenta,


                Ventas =
                    ventas
            };
        }
    }
}