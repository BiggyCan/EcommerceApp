using EcommerceApp.Data;
using EcommerceApp.Reports.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Reports.Queries
{
    public class ComprobanteCompraQuery
    {
        private readonly ApplicationDbContext _context;


        public ComprobanteCompraQuery(
            ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<ComprobanteCompraReportModel?> ExecuteAsync(
            int pedidoId)
        {
            // ==========================================
            // BUSCAR PEDIDO
            // ==========================================

            var pedido =
                await _context.Orders
                    .AsNoTracking()
                    .Include(
                        order =>
                            order.Items
                    )
                    .FirstOrDefaultAsync(
                        order =>
                            order.Id == pedidoId
                    );


            if (pedido == null)
            {
                return null;
            }


            // ==========================================
            // PRODUCTOS DEL PEDIDO
            // ==========================================

            var productos =
                pedido.Items
                    .Select(
                        item =>
                            new ComprobanteCompraItem
                            {
                                ProductId =
                                    item.ProductId,

                                Producto =
                                    item.ProductName,

                                Cantidad =
                                    item.Quantity,

                                PrecioUnitario =
                                    item.UnitPrice,

                                Subtotal =
                                    item.UnitPrice
                                    *
                                    item.Quantity
                            }
                    )
                    .ToList();


            // ==========================================
            // TOTAL DE UNIDADES
            // ==========================================

            var totalUnidades =
                productos.Sum(
                    producto =>
                        producto.Cantidad
                );


            // ==========================================
            // CONVERTIR FECHA UTC A HORA BOLIVIA
            // ==========================================

            var fechaPedidoBolivia =
                pedido.CreatedAt
                    .ToUniversalTime()
                    .AddHours(-4);


            var fechaGeneracionBolivia =
                DateTime.UtcNow
                    .AddHours(-4);


            // ==========================================
            // CREAR MODELO DEL COMPROBANTE
            // ==========================================

            return new ComprobanteCompraReportModel
            {
                PedidoId =
                    pedido.Id,

                FechaPedido =
                    fechaPedidoBolivia,

                FechaGeneracion =
                    fechaGeneracionBolivia,

                Cliente =
                    pedido.CustomerName,

                Correo =
                    pedido.CustomerEmail,

                Direccion =
                    string.IsNullOrWhiteSpace(
                        pedido.ShippingAddress
                    )
                        ? "No registrada"
                        : pedido.ShippingAddress,

                Estado =
                    pedido.Status,

                TotalUnidades =
                    totalUnidades,

                TotalCompra =
                    pedido.TotalAmount,

                Productos =
                    productos
            };
        }
    }
}