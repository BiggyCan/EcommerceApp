using EcommerceApp.Data;
using EcommerceApp.Reports.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Reports.Queries
{
    public class ConciliacionPagosQuery
    {
        private readonly ApplicationDbContext _context;


        public ConciliacionPagosQuery(
            ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<ConciliacionPagosReportModel> ExecuteAsync(
            decimal porcentajeComision = 3m)
        {
            // ==========================================
            // VALIDAR COMISIÓN
            // ==========================================

            if (porcentajeComision < 0)
            {
                porcentajeComision = 0;
            }


            if (porcentajeComision > 100)
            {
                porcentajeComision = 100;
            }


            // ==========================================
            // CONSULTAR PEDIDOS
            // ==========================================

            var pedidos =
                await _context.Orders
                    .AsNoTracking()
                    .OrderByDescending(
                        order =>
                            order.CreatedAt
                    )
                    .ToListAsync();


            // ==========================================
            // PREPARAR MOVIMIENTOS
            // ==========================================

            var movimientos =
                pedidos
                    .Select(
                        order =>
                        {
                            var importeBruto =
                                order.TotalAmount;


                            var comision =
                                Math.Round(
                                    importeBruto
                                    *
                                    porcentajeComision
                                    /
                                    100m,
                                    2,
                                    MidpointRounding.AwayFromZero
                                );


                            var importeNeto =
                                importeBruto
                                -
                                comision;


                            return new ConciliacionPagoItem
                            {
                                PedidoId =
                                    order.Id,

                                Cliente =
                                    string.IsNullOrWhiteSpace(
                                        order.CustomerName
                                    )
                                        ? "Cliente BigGame"
                                        : order.CustomerName,

                                Correo =
                                    string.IsNullOrWhiteSpace(
                                        order.CustomerEmail
                                    )
                                        ? "No registrado"
                                        : order.CustomerEmail,

                                Fecha =
                                    order.CreatedAt
                                        .ToUniversalTime()
                                        .AddHours(-4),

                                EstadoPedido =
                                    order.Status,

                                EstadoConciliacion =
                                    "Calculado",

                                ImporteBruto =
                                    importeBruto,

                                Comision =
                                    comision,

                                ImporteNeto =
                                    importeNeto
                            };
                        }
                    )
                    .ToList();


            // ==========================================
            // TOTALES
            // ==========================================

            var totalBruto =
                movimientos.Sum(
                    item =>
                        item.ImporteBruto
                );


            var totalComisiones =
                movimientos.Sum(
                    item =>
                        item.Comision
                );


            var totalNeto =
                movimientos.Sum(
                    item =>
                        item.ImporteNeto
                );


            // ==========================================
            // MODELO FINAL
            // ==========================================

            return new ConciliacionPagosReportModel
            {
                FechaGeneracion =
                    DateTime.UtcNow
                        .AddHours(-4),

                PorcentajeComision =
                    porcentajeComision,

                TotalPedidos =
                    movimientos.Count,

                TotalBruto =
                    totalBruto,

                TotalComisiones =
                    totalComisiones,

                TotalNeto =
                    totalNeto,

                Movimientos =
                    movimientos
            };
        }
    }
}