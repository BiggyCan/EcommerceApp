using EcommerceApp.Data;
using EcommerceApp.Reports.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Reports.Queries
{
    public class CarritosAbandonadosQuery
    {
        private readonly ApplicationDbContext _context;


        public CarritosAbandonadosQuery(
            ApplicationDbContext context)
        {
            _context =
                context;
        }


        public async Task<CarritosAbandonadosReportModel> ExecuteAsync(
            int minutosAbandono = 30)
        {
            // ==========================================
            // VALIDAR TIEMPO DE ABANDONO
            // ==========================================

            if (minutosAbandono < 1)
            {
                minutosAbandono =
                    30;
            }


            if (minutosAbandono > 10080)
            {
                minutosAbandono =
                    10080;
            }


            // ==========================================
            // FECHA ACTUAL UTC
            // ==========================================

            var ahoraUtc =
                DateTime.UtcNow;


            var fechaLimite =
                ahoraUtc.AddMinutes(
                    -minutosAbandono
                );


            // ==========================================
            // DETECTAR CARRITOS ACTIVOS SIN ACTIVIDAD
            // ==========================================

            var carritosParaAbandonar =
                await _context
                    .TrackedCarts
                    .Where(
                        cart =>
                            cart.Status
                            ==
                            "Activo"
                            &&
                            cart.LastActivityAt
                            <=
                            fechaLimite
                    )
                    .ToListAsync();


            // ==========================================
            // ACTIVO -> ABANDONADO
            // ==========================================

            if (
                carritosParaAbandonar.Any()
            )
            {
                foreach (
                    var cart
                    in carritosParaAbandonar
                )
                {
                    cart.Status =
                        "Abandonado";
                }


                await _context
                    .SaveChangesAsync();
            }


            // ==========================================
            // CONSULTAR TODOS LOS ABANDONADOS
            // ==========================================

            var carritosAbandonados =
                await _context
                    .TrackedCarts
                    .AsNoTracking()
                    .Include(
                        cart =>
                            cart.User
                    )
                    .Include(
                        cart =>
                            cart.Items
                    )
                    .Where(
                        cart =>
                            cart.Status
                            ==
                            "Abandonado"
                    )
                    .OrderByDescending(
                        cart =>
                            cart.LastActivityAt
                    )
                    .ToListAsync();


            // ==========================================
            // PREPARAR ITEMS DEL REPORTE
            // ==========================================

            var items =
                carritosAbandonados
                    .Select(
                        cart =>
                        {
                            var minutosSinActividad =
                                (
                                    ahoraUtc
                                    -
                                    cart.LastActivityAt
                                )
                                .TotalMinutes;


                            var productos =
                                cart.Items.Any()
                                    ?
                                    string.Join(
                                        ", ",
                                        cart.Items
                                            .OrderBy(
                                                item =>
                                                    item.ProductName
                                            )
                                            .Select(
                                                item =>
                                                    $"{item.ProductName} x{item.Quantity}"
                                            )
                                    )
                                    :
                                    "Sin productos";


                            return new CarritoAbandonadoItem
                            {
                                CarritoId =
                                    cart.Id,

                                Usuario =
                                    !string.IsNullOrWhiteSpace(
                                        cart.User?.FullName
                                    )
                                        ?
                                        cart.User!.FullName!
                                        :
                                        cart.User?.Email
                                        ??
                                        "Usuario no disponible",

                                Correo =
                                    cart.User?.Email
                                    ??
                                    "No registrado",

                                TotalItems =
                                    cart.TotalItems,

                                TotalAmount =
                                    cart.TotalAmount,

                                CreatedAt =
                                    cart.CreatedAt
                                        .ToUniversalTime()
                                        .AddHours(-4),

                                LastActivityAt =
                                    cart.LastActivityAt
                                        .ToUniversalTime()
                                        .AddHours(-4),

                                MinutosSinActividad =
                                    Math.Round(
                                        minutosSinActividad,
                                        0
                                    ),

                                Estado =
                                    cart.Status,

                                Productos =
                                    productos
                            };
                        }
                    )
                    .ToList();


            // ==========================================
            // TOTALES
            // ==========================================

            var totalCarritos =
                items.Count;


            var totalUnidades =
                items.Sum(
                    item =>
                        item.TotalItems
                );


            var valorTotalAbandonado =
                items.Sum(
                    item =>
                        item.TotalAmount
                );


            // ==========================================
            // MODELO FINAL
            // ==========================================

            return new CarritosAbandonadosReportModel
            {
                FechaGeneracion =
                    ahoraUtc
                        .AddHours(-4),

                MinutosAbandono =
                    minutosAbandono,

                TotalCarritos =
                    totalCarritos,

                TotalUnidades =
                    totalUnidades,

                ValorTotalAbandonado =
                    valorTotalAbandonado,

                Carritos =
                    items
            };
        }
    }
}