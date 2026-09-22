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
    public class SalesReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalesReportsController(
            ApplicationDbContext context)
        {
            _context = context;
        }


        // ==========================================
        // REPORTE DE VENTAS
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> VentasPdf()
        {
            var orders =
                await _context.Orders
                    .Include(o => o.Items)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();


            var fecha =
                FechaBolivia();


            var totalVentas =
                orders.Count;


            var totalIngresos =
                orders.Sum(
                    o => o.TotalAmount
                );


            var totalUnidades =
                orders.Sum(
                    o => o.Items.Sum(
                        i => i.Quantity
                    )
                );


            var promedioVenta =
                totalVentas > 0
                    ? totalIngresos / totalVentas
                    : 0;


            var totalLineas =
                orders.Sum(
                    o => o.Items.Count
                );


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
                                                                            "REPORTE DE VENTAS"
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
                                            // TARJETAS DE RESUMEN
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
                                                                        "Ventas registradas",
                                                                        totalVentas.ToString()
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(8)
                                                            .Element(
                                                                c =>
                                                                    TarjetaResumen(
                                                                        c,
                                                                        "Unidades vendidas",
                                                                        totalUnidades.ToString()
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(8)
                                                            .Element(
                                                                c =>
                                                                    TarjetaResumen(
                                                                        c,
                                                                        "Productos registrados",
                                                                        totalLineas.ToString()
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(8)
                                                            .Element(
                                                                c =>
                                                                    TarjetaResumen(
                                                                        c,
                                                                        "Ingresos totales",
                                                                        Moneda(
                                                                            totalIngresos
                                                                        )
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(8)
                                                            .Element(
                                                                c =>
                                                                    TarjetaResumen(
                                                                        c,
                                                                        "Promedio por venta",
                                                                        Moneda(
                                                                            promedioVenta
                                                                        )
                                                                    )
                                                            );
                                                    }
                                                );


                                            // ==========================
                                            // TÍTULO DETALLE
                                            // ==========================

                                            column.Item()
                                                .PaddingTop(4)
                                                .Text(
                                                    "Detalle de ventas"
                                                )
                                                .FontSize(13)
                                                .Bold()
                                                .FontColor(
                                                    Colors.Grey.Darken4
                                                );


                                            // ==========================
                                            // SIN VENTAS
                                            // ==========================

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
                                                        "No existen ventas registradas."
                                                    )
                                                    .FontSize(11)
                                                    .FontColor(
                                                        Colors.Grey.Darken1
                                                    );
                                            }
                                            else
                                            {
                                                // ======================
                                                // TABLA
                                                // ======================

                                                column.Item()
                                                    .Table(
                                                        table =>
                                                        {
                                                            table.ColumnsDefinition(
                                                                columns =>
                                                                {
                                                                    columns.ConstantColumn(
                                                                        45
                                                                    );

                                                                    columns.ConstantColumn(
                                                                        70
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        1.6f
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        2.4f
                                                                    );

                                                                    columns.ConstantColumn(
                                                                        48
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        1
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        1
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        1
                                                                    );
                                                                }
                                                            );


                                                            // ==================
                                                            // CABECERA
                                                            // ==================

                                                            table.Header(
                                                                header =>
                                                                {
                                                                    header.Cell()
                                                                        .Element(
                                                                            CeldaCabecera
                                                                        )
                                                                        .Text(
                                                                            "Pedido"
                                                                        );

                                                                    header.Cell()
                                                                        .Element(
                                                                            CeldaCabecera
                                                                        )
                                                                        .Text(
                                                                            "Fecha"
                                                                        );

                                                                    header.Cell()
                                                                        .Element(
                                                                            CeldaCabecera
                                                                        )
                                                                        .Text(
                                                                            "Cliente"
                                                                        );

                                                                    header.Cell()
                                                                        .Element(
                                                                            CeldaCabecera
                                                                        )
                                                                        .Text(
                                                                            "Producto"
                                                                        );

                                                                    header.Cell()
                                                                        .Element(
                                                                            CeldaCabecera
                                                                        )
                                                                        .AlignCenter()
                                                                        .Text(
                                                                            "Cant."
                                                                        );

                                                                    header.Cell()
                                                                        .Element(
                                                                            CeldaCabecera
                                                                        )
                                                                        .AlignRight()
                                                                        .Text(
                                                                            "P. unitario"
                                                                        );

                                                                    header.Cell()
                                                                        .Element(
                                                                            CeldaCabecera
                                                                        )
                                                                        .AlignRight()
                                                                        .Text(
                                                                            "Subtotal"
                                                                        );

                                                                    header.Cell()
                                                                        .Element(
                                                                            CeldaCabecera
                                                                        )
                                                                        .AlignCenter()
                                                                        .Text(
                                                                            "Estado"
                                                                        );
                                                                }
                                                            );


                                                            // ==================
                                                            // DATOS
                                                            // ==================

                                                            foreach (
                                                                var order
                                                                in orders
                                                            )
                                                            {
                                                                var fechaPedido =
                                                                    order.CreatedAt
                                                                        .AddHours(
                                                                            -4
                                                                        );


                                                                if (
                                                                    order.Items.Count == 0
                                                                )
                                                                {
                                                                    table.Cell()
                                                                        .Element(
                                                                            CeldaNormal
                                                                        )
                                                                        .Text(
                                                                            $"#{order.Id}"
                                                                        );

                                                                    table.Cell()
                                                                        .Element(
                                                                            CeldaNormal
                                                                        )
                                                                        .Text(
                                                                            fechaPedido.ToString(
                                                                                "dd/MM/yyyy HH:mm"
                                                                            )
                                                                        );

                                                                    table.Cell()
                                                                        .Element(
                                                                            CeldaNormal
                                                                        )
                                                                        .Column(
                                                                            cliente =>
                                                                            {
                                                                                cliente.Item()
                                                                                    .Text(
                                                                                        order.CustomerName
                                                                                    )
                                                                                    .Bold();

                                                                                cliente.Item()
                                                                                    .Text(
                                                                                        order.CustomerEmail
                                                                                    )
                                                                                    .FontSize(
                                                                                        7
                                                                                    )
                                                                                    .FontColor(
                                                                                        Colors.Grey.Darken1
                                                                                    );
                                                                            }
                                                                        );

                                                                    table.Cell()
                                                                        .Element(
                                                                            CeldaNormal
                                                                        )
                                                                        .Text(
                                                                            "Sin detalle"
                                                                        );

                                                                    table.Cell()
                                                                        .Element(
                                                                            CeldaNormal
                                                                        )
                                                                        .AlignCenter()
                                                                        .Text(
                                                                            "-"
                                                                        );

                                                                    table.Cell()
                                                                        .Element(
                                                                            CeldaNormal
                                                                        )
                                                                        .AlignRight()
                                                                        .Text(
                                                                            "-"
                                                                        );

                                                                    table.Cell()
                                                                        .Element(
                                                                            CeldaNormal
                                                                        )
                                                                        .AlignRight()
                                                                        .Text(
                                                                            Moneda(
                                                                                order.TotalAmount
                                                                            )
                                                                        );

                                                                    table.Cell()
                                                                        .Element(
                                                                            CeldaNormal
                                                                        )
                                                                        .AlignCenter()
                                                                        .Text(
                                                                            order.Status
                                                                        );

                                                                    continue;
                                                                }


                                                                foreach (
                                                                    var item
                                                                    in order.Items
                                                                )
                                                                {
                                                                    table.Cell()
                                                                        .Element(
                                                                            CeldaNormal
                                                                        )
                                                                        .Text(
                                                                            $"#{order.Id}"
                                                                        );

                                                                    table.Cell()
                                                                        .Element(
                                                                            CeldaNormal
                                                                        )
                                                                        .Text(
                                                                            fechaPedido.ToString(
                                                                                "dd/MM/yyyy HH:mm"
                                                                            )
                                                                        );

                                                                    table.Cell()
                                                                        .Element(
                                                                            CeldaNormal
                                                                        )
                                                                        .Column(
                                                                            cliente =>
                                                                            {
                                                                                cliente.Item()
                                                                                    .Text(
                                                                                        order.CustomerName
                                                                                    )
                                                                                    .Bold();

                                                                                cliente.Item()
                                                                                    .Text(
                                                                                        order.CustomerEmail
                                                                                    )
                                                                                    .FontSize(
                                                                                        7
                                                                                    )
                                                                                    .FontColor(
                                                                                        Colors.Grey.Darken1
                                                                                    );
                                                                            }
                                                                        );

                                                                    table.Cell()
                                                                        .Element(
                                                                            CeldaNormal
                                                                        )
                                                                        .Text(
                                                                            item.ProductName
                                                                        );

                                                                    table.Cell()
                                                                        .Element(
                                                                            CeldaNormal
                                                                        )
                                                                        .AlignCenter()
                                                                        .Text(
                                                                            item.Quantity.ToString()
                                                                        );

                                                                    table.Cell()
                                                                        .Element(
                                                                            CeldaNormal
                                                                        )
                                                                        .AlignRight()
                                                                        .Text(
                                                                            Moneda(
                                                                                item.UnitPrice
                                                                            )
                                                                        );

                                                                    table.Cell()
                                                                        .Element(
                                                                            CeldaNormal
                                                                        )
                                                                        .AlignRight()
                                                                        .Text(
                                                                            Moneda(
                                                                                item.Subtotal
                                                                            )
                                                                        );

                                                                    table.Cell()
                                                                        .Element(
                                                                            CeldaNormal
                                                                        )
                                                                        .AlignCenter()
                                                                        .Text(
                                                                            order.Status
                                                                        );
                                                                }
                                                            }
                                                        }
                                                    );


                                                // ======================
                                                // RESUMEN FINAL
                                                // ======================

                                                column.Item()
                                                    .PaddingTop(4)
                                                    .AlignRight()
                                                    .Column(
                                                        resumen =>
                                                        {
                                                            resumen.Item()
                                                                .Text(
                                                                    $"TOTAL DE VENTAS: {totalVentas}"
                                                                )
                                                                .FontSize(9)
                                                                .Bold();

                                                            resumen.Item()
                                                                .PaddingTop(3)
                                                                .Text(
                                                                    $"TOTAL INGRESADO: {Moneda(totalIngresos)}"
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
                                                    "BigGame • Reporte de ventas"
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
                                                                style.FontSize(
                                                                    8
                                                                )
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
                $"BigGame_Ventas_{fecha:yyyyMMdd_HHmm}.pdf";


            return File(
                pdf,
                "application/pdf",
                fileName
            );
        }


        // ==========================================
        // TARJETA RESUMEN
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
        // CABECERA DE TABLA
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
        // CELDAS DE TABLA
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
        // FORMATO MONETARIO
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
        // HORA DE BOLIVIA
        // ==========================================

        private static DateTime FechaBolivia()
        {
            return DateTime.UtcNow
                .AddHours(-4);
        }
    }
}