using System.Globalization;
using EcommerceApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EcommerceApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrdersReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersReportsController(
            ApplicationDbContext context)
        {
            _context = context;
        }


        // ==========================================
        // REPORTE DE PEDIDOS
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> PedidosPdf()
        {
            var orders =
                await _context.Orders
                    .Include(o => o.Items)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();


            var fecha =
                FechaBolivia();


            var totalPedidos =
                orders.Count;


            var totalUnidades =
                orders.Sum(
                    o => o.Items.Sum(
                        i => i.Quantity
                    )
                );


            var totalGeneral =
                orders.Sum(
                    o => o.TotalAmount
                );


            var confirmados =
                orders.Count(
                    o =>
                        o.Status.Equals(
                            "Confirmado",
                            StringComparison.OrdinalIgnoreCase
                        )
                );


            var otrosEstados =
                totalPedidos - confirmados;


            var document =
                Document.Create(
                    container =>
                    {
                        container.Page(
                            page =>
                            {
                                page.Size(
                                    PageSizes.A4.Landscape()
                                );

                                page.Margin(25);

                                page.PageColor(
                                    Colors.White
                                );

                                page.DefaultTextStyle(
                                    style =>
                                        style
                                            .FontSize(8)
                                            .FontColor(
                                                Colors.Grey.Darken4
                                            )
                                );


                                // ==================================
                                // ENCABEZADO
                                // ==================================

                                page.Header()
                                    .Column(
                                        column =>
                                        {
                                            column.Item()
                                                .Row(
                                                    row =>
                                                    {
                                                        row.RelativeItem()
                                                            .Column(
                                                                left =>
                                                                {
                                                                    left.Item()
                                                                        .Text(
                                                                            "BIGGAME"
                                                                        )
                                                                        .FontSize(24)
                                                                        .Bold()
                                                                        .FontColor(
                                                                            Colors.Blue.Medium
                                                                        );

                                                                    left.Item()
                                                                        .Text(
                                                                            "Sistema de ventas e inventario"
                                                                        )
                                                                        .FontSize(9)
                                                                        .FontColor(
                                                                            Colors.Grey.Darken1
                                                                        );
                                                                }
                                                            );


                                                        row.ConstantItem(250)
                                                            .AlignRight()
                                                            .Column(
                                                                right =>
                                                                {
                                                                    right.Item()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            "REPORTE DE PEDIDOS"
                                                                        )
                                                                        .FontSize(16)
                                                                        .Bold();

                                                                    right.Item()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            $"Generado: {fecha:dd/MM/yyyy HH:mm}"
                                                                        )
                                                                        .FontSize(8)
                                                                        .FontColor(
                                                                            Colors.Grey.Darken1
                                                                        );
                                                                }
                                                            );
                                                    }
                                                );


                                            column.Item()
                                                .PaddingTop(10)
                                                .BorderBottom(2)
                                                .BorderColor(
                                                    Colors.Blue.Medium
                                                );
                                        }
                                    );


                                // ==================================
                                // CONTENIDO
                                // ==================================

                                page.Content()
                                    .PaddingVertical(15)
                                    .Column(
                                        column =>
                                        {
                                            column.Spacing(14);


                                            // ==========================
                                            // RESUMEN
                                            // ==========================

                                            column.Item()
                                                .Row(
                                                    row =>
                                                    {
                                                        row.RelativeItem()
                                                            .Element(
                                                                c =>
                                                                    TarjetaResumen(
                                                                        c,
                                                                        "Pedidos registrados",
                                                                        totalPedidos.ToString()
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(8)
                                                            .Element(
                                                                c =>
                                                                    TarjetaResumen(
                                                                        c,
                                                                        "Unidades pedidas",
                                                                        totalUnidades.ToString()
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(8)
                                                            .Element(
                                                                c =>
                                                                    TarjetaResumen(
                                                                        c,
                                                                        "Confirmados",
                                                                        confirmados.ToString()
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(8)
                                                            .Element(
                                                                c =>
                                                                    TarjetaResumen(
                                                                        c,
                                                                        "Otros estados",
                                                                        otrosEstados.ToString()
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(8)
                                                            .Element(
                                                                c =>
                                                                    TarjetaResumen(
                                                                        c,
                                                                        "Total acumulado",
                                                                        Moneda(
                                                                            totalGeneral
                                                                        )
                                                                    )
                                                            );
                                                    }
                                                );


                                            column.Item()
                                                .Text(
                                                    "Listado de pedidos"
                                                )
                                                .FontSize(13)
                                                .Bold();


                                            if (orders.Count == 0)
                                            {
                                                column.Item()
                                                    .Border(1)
                                                    .BorderColor(
                                                        Colors.Grey.Lighten2
                                                    )
                                                    .Background(
                                                        Colors.Grey.Lighten4
                                                    )
                                                    .Padding(25)
                                                    .AlignCenter()
                                                    .Text(
                                                        "No existen pedidos registrados."
                                                    )
                                                    .FontSize(11);
                                            }
                                            else
                                            {
                                                column.Item()
                                                    .Table(
                                                        table =>
                                                        {
                                                            table.ColumnsDefinition(
                                                                columns =>
                                                                {
                                                                    columns.ConstantColumn(45);

                                                                    columns.ConstantColumn(80);

                                                                    columns.RelativeColumn(1.5f);

                                                                    columns.RelativeColumn(1.6f);

                                                                    columns.RelativeColumn(2);

                                                                    columns.ConstantColumn(55);

                                                                    columns.ConstantColumn(60);

                                                                    columns.RelativeColumn(1);

                                                                    columns.RelativeColumn(1);
                                                                }
                                                            );


                                                            // ==================
                                                            // CABECERA
                                                            // ==================

                                                            table.Header(
                                                                header =>
                                                                {
                                                                    header.Cell()
                                                                        .Element(CeldaCabecera)
                                                                        .Text("Pedido");

                                                                    header.Cell()
                                                                        .Element(CeldaCabecera)
                                                                        .Text("Fecha");

                                                                    header.Cell()
                                                                        .Element(CeldaCabecera)
                                                                        .Text("Cliente");

                                                                    header.Cell()
                                                                        .Element(CeldaCabecera)
                                                                        .Text("Correo");

                                                                    header.Cell()
                                                                        .Element(CeldaCabecera)
                                                                        .Text("Dirección");

                                                                    header.Cell()
                                                                        .Element(CeldaCabecera)
                                                                        .AlignCenter()
                                                                        .Text("Productos");

                                                                    header.Cell()
                                                                        .Element(CeldaCabecera)
                                                                        .AlignCenter()
                                                                        .Text("Unidades");

                                                                    header.Cell()
                                                                        .Element(CeldaCabecera)
                                                                        .AlignRight()
                                                                        .Text("Total");

                                                                    header.Cell()
                                                                        .Element(CeldaCabecera)
                                                                        .AlignCenter()
                                                                        .Text("Estado");
                                                                }
                                                            );


                                                            // ==================
                                                            // FILAS
                                                            // ==================

                                                            foreach (
                                                                var order
                                                                in orders
                                                            )
                                                            {
                                                                var fechaPedido =
                                                                    order.CreatedAt
                                                                        .AddHours(-4);


                                                                var cantidadProductos =
                                                                    order.Items.Count;


                                                                var unidades =
                                                                    order.Items.Sum(
                                                                        i => i.Quantity
                                                                    );


                                                                table.Cell()
                                                                    .Element(CeldaNormal)
                                                                    .Text(
                                                                        $"#{order.Id}"
                                                                    );


                                                                table.Cell()
                                                                    .Element(CeldaNormal)
                                                                    .Text(
                                                                        fechaPedido.ToString(
                                                                            "dd/MM/yyyy HH:mm"
                                                                        )
                                                                    );


                                                                table.Cell()
                                                                    .Element(CeldaNormal)
                                                                    .Text(
                                                                        order.CustomerName
                                                                    );


                                                                table.Cell()
                                                                    .Element(CeldaNormal)
                                                                    .Text(
                                                                        order.CustomerEmail
                                                                    );


                                                                table.Cell()
                                                                    .Element(CeldaNormal)
                                                                    .Text(
                                                                        string.IsNullOrWhiteSpace(
                                                                            order.ShippingAddress
                                                                        )
                                                                            ? "Sin dirección"
                                                                            : order.ShippingAddress
                                                                    );


                                                                table.Cell()
                                                                    .Element(CeldaNormal)
                                                                    .AlignCenter()
                                                                    .Text(
                                                                        cantidadProductos.ToString()
                                                                    );


                                                                table.Cell()
                                                                    .Element(CeldaNormal)
                                                                    .AlignCenter()
                                                                    .Text(
                                                                        unidades.ToString()
                                                                    );


                                                                table.Cell()
                                                                    .Element(CeldaNormal)
                                                                    .AlignRight()
                                                                    .Text(
                                                                        Moneda(
                                                                            order.TotalAmount
                                                                        )
                                                                    );


                                                                table.Cell()
                                                                    .Element(CeldaNormal)
                                                                    .AlignCenter()
                                                                    .Text(
                                                                        order.Status
                                                                    );
                                                            }
                                                        }
                                                    );


                                                // ======================
                                                // TOTAL
                                                // ======================

                                                column.Item()
                                                    .AlignRight()
                                                    .Column(
                                                        resumen =>
                                                        {
                                                            resumen.Item()
                                                                .Text(
                                                                    $"TOTAL DE PEDIDOS: {totalPedidos}"
                                                                )
                                                                .FontSize(9)
                                                                .Bold();


                                                            resumen.Item()
                                                                .PaddingTop(3)
                                                                .Text(
                                                                    $"VALOR TOTAL: {Moneda(totalGeneral)}"
                                                                )
                                                                .FontSize(12)
                                                                .Bold()
                                                                .FontColor(
                                                                    Colors.Blue.Darken2
                                                                );
                                                        }
                                                    );
                                            }
                                        }
                                    );


                                // ==================================
                                // PIE DE PÁGINA
                                // ==================================

                                page.Footer()
                                    .BorderTop(1)
                                    .BorderColor(
                                        Colors.Grey.Lighten2
                                    )
                                    .PaddingTop(8)
                                    .Row(
                                        row =>
                                        {
                                            row.RelativeItem()
                                                .Text(
                                                    "BigGame • Reporte de pedidos"
                                                )
                                                .FontSize(8)
                                                .FontColor(
                                                    Colors.Grey.Darken1
                                                );


                                            row.RelativeItem()
                                                .AlignRight()
                                                .Text(
                                                    text =>
                                                    {
                                                        text.DefaultTextStyle(
                                                            style =>
                                                                style.FontSize(8)
                                                        );

                                                        text.Span(
                                                            "Página "
                                                        );

                                                        text.CurrentPageNumber();

                                                        text.Span(
                                                            " de "
                                                        );

                                                        text.TotalPages();
                                                    }
                                                );
                                        }
                                    );
                            }
                        );
                    }
                );


            var pdf =
                document.GeneratePdf();


            var fileName =
                $"BigGame_Pedidos_{fecha:yyyyMMdd_HHmm}.pdf";


            return File(
                pdf,
                "application/pdf",
                fileName
            );
        }


        // ==========================================
        // TARJETA
        // ==========================================

        private static void TarjetaResumen(
            IContainer container,
            string titulo,
            string valor)
        {
            container
                .Border(1)
                .BorderColor(
                    Colors.Grey.Lighten2
                )
                .Background(
                    Colors.Grey.Lighten4
                )
                .Padding(9)
                .Column(
                    column =>
                    {
                        column.Item()
                            .Text(titulo)
                            .FontSize(7)
                            .FontColor(
                                Colors.Grey.Darken1
                            );


                        column.Item()
                            .PaddingTop(3)
                            .Text(valor)
                            .FontSize(12)
                            .Bold()
                            .FontColor(
                                Colors.Blue.Darken2
                            );
                    }
                );
        }


        // ==========================================
        // CABECERA TABLA
        // ==========================================

        private static IContainer CeldaCabecera(
            IContainer container)
        {
            return container
                .Background(
                    Colors.Grey.Darken4
                )
                .PaddingVertical(6)
                .PaddingHorizontal(5)
                .DefaultTextStyle(
                    style =>
                        style
                            .FontColor(
                                Colors.White
                            )
                            .Bold()
                            .FontSize(7)
                );
        }


        // ==========================================
        // CELDA NORMAL
        // ==========================================

        private static IContainer CeldaNormal(
            IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor(
                    Colors.Grey.Lighten2
                )
                .PaddingVertical(5)
                .PaddingHorizontal(5);
        }


        // ==========================================
        // MONEDA
        // ==========================================

        private static string Moneda(
            decimal valor)
        {
            var cultura =
                CultureInfo.GetCultureInfo(
                    "es-BO"
                );


            return
                $"Bs {valor.ToString("N2", cultura)}";
        }


        // ==========================================
        // HORA BOLIVIA
        // ==========================================

        private static DateTime FechaBolivia()
        {
            return DateTime.UtcNow
                .AddHours(-4);
        }
    }
}