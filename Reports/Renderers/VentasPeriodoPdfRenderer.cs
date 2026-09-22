using System.Globalization;
using EcommerceApp.Reports.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EcommerceApp.Reports.Renderers
{
    public class VentasPeriodoPdfRenderer
    {
        public byte[] Render(
            VentasPeriodoReportModel model)
        {
            var document =
                Document.Create(
                    container =>
                    {
                        container.Page(
                            page =>
                            {
                                page.Size(
                                    PageSizes.A4
                                );

                                page.Margin(28);

                                page.PageColor(
                                    Colors.White
                                );

                                page.DefaultTextStyle(
                                    style =>
                                        style
                                            .FontSize(9)
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
                                                                        .FontSize(25)
                                                                        .Bold()
                                                                        .FontColor(
                                                                            Colors.Blue.Medium
                                                                        );

                                                                    left.Item()
                                                                        .Text(
                                                                            "Sistema de ventas e inventario"
                                                                        )
                                                                        .FontSize(8)
                                                                        .FontColor(
                                                                            Colors.Grey.Darken1
                                                                        );
                                                                }
                                                            );


                                                        row.ConstantItem(240)
                                                            .AlignRight()
                                                            .Column(
                                                                right =>
                                                                {
                                                                    right.Item()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            "REPORTE DE VENTAS POR PERÍODO"
                                                                        )
                                                                        .FontSize(15)
                                                                        .Bold();


                                                                    right.Item()
                                                                        .PaddingTop(3)
                                                                        .AlignRight()
                                                                        .Text(
                                                                            $"Generado: {model.FechaGeneracion:dd/MM/yyyy HH:mm}"
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
                                    .PaddingVertical(16)
                                    .Column(
                                        column =>
                                        {
                                            column.Spacing(14);


                                            // ==================================
                                            // PERÍODO
                                            // ==================================

                                            column.Item()
                                                .Background(
                                                    Colors.Blue.Lighten5
                                                )
                                                .Border(1)
                                                .BorderColor(
                                                    Colors.Blue.Lighten3
                                                )
                                                .Padding(12)
                                                .Column(
                                                    box =>
                                                    {
                                                        box.Item()
                                                            .Text(
                                                                "Período consultado"
                                                            )
                                                            .FontSize(10)
                                                            .Bold()
                                                            .FontColor(
                                                                Colors.Blue.Darken2
                                                            );


                                                        box.Item()
                                                            .PaddingTop(4)
                                                            .Text(
                                                                $"{model.FechaInicio:dd/MM/yyyy} al {model.FechaFin:dd/MM/yyyy}"
                                                            )
                                                            .FontSize(13)
                                                            .Bold();
                                                    }
                                                );


                                            // ==================================
                                            // RESUMEN
                                            // ==================================

                                            column.Item()
                                                .Text(
                                                    "Resumen de ventas"
                                                )
                                                .FontSize(13)
                                                .Bold();


                                            column.Item()
                                                .Row(
                                                    row =>
                                                    {
                                                        row.RelativeItem()
                                                            .Element(
                                                                c =>
                                                                    Tarjeta(
                                                                        c,
                                                                        "Pedidos",
                                                                        model.TotalPedidos.ToString(),
                                                                        "registrados"
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(7)
                                                            .Element(
                                                                c =>
                                                                    Tarjeta(
                                                                        c,
                                                                        "Productos vendidos",
                                                                        model.TotalProductosVendidos.ToString(),
                                                                        "unidades"
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(7)
                                                            .Element(
                                                                c =>
                                                                    Tarjeta(
                                                                        c,
                                                                        "Total vendido",
                                                                        Moneda(
                                                                            model.TotalVentas
                                                                        ),
                                                                        "ingresos"
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(7)
                                                            .Element(
                                                                c =>
                                                                    Tarjeta(
                                                                        c,
                                                                        "Promedio",
                                                                        Moneda(
                                                                            model.PromedioVenta
                                                                        ),
                                                                        "por pedido"
                                                                    )
                                                            );
                                                    }
                                                );


                                            // ==================================
                                            // TABLA
                                            // ==================================

                                            column.Item()
                                                .Text(
                                                    "Detalle de ventas"
                                                )
                                                .FontSize(13)
                                                .Bold();


                                            if (model.Ventas.Count == 0)
                                            {
                                                column.Item()
                                                    .Border(1)
                                                    .BorderColor(
                                                        Colors.Grey.Lighten2
                                                    )
                                                    .Background(
                                                        Colors.Grey.Lighten4
                                                    )
                                                    .Padding(18)
                                                    .AlignCenter()
                                                    .Text(
                                                        "No existen ventas registradas en el período seleccionado."
                                                    )
                                                    .FontSize(10)
                                                    .FontColor(
                                                        Colors.Grey.Darken1
                                                    );
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
                                                                    columns.ConstantColumn(
                                                                        45
                                                                    );

                                                                    columns.ConstantColumn(
                                                                        88
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        2.2f
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        1.3f
                                                                    );

                                                                    columns.ConstantColumn(
                                                                        55
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        1.2f
                                                                    );
                                                                }
                                                            );


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
                                                                            "Estado"
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
                                                                            "Total"
                                                                        );
                                                                }
                                                            );


                                                            foreach (
                                                                var venta
                                                                in model.Ventas
                                                            )
                                                            {
                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .Text(
                                                                        $"#{venta.PedidoId}"
                                                                    );


                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .Text(
                                                                        venta.Fecha.ToString(
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
                                                                                    venta.Cliente
                                                                                )
                                                                                .Bold();


                                                                            cliente.Item()
                                                                                .PaddingTop(2)
                                                                                .Text(
                                                                                    venta.Correo
                                                                                )
                                                                                .FontSize(6)
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
                                                                        venta.Estado
                                                                    );


                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .AlignCenter()
                                                                    .Text(
                                                                        venta.CantidadProductos.ToString()
                                                                    );


                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .AlignRight()
                                                                    .Text(
                                                                        Moneda(
                                                                            venta.Total
                                                                        )
                                                                    )
                                                                    .Bold();
                                                            }
                                                        }
                                                    );
                                            }


                                            // ==================================
                                            // TOTAL FINAL
                                            // ==================================

                                            column.Item()
                                                .AlignRight()
                                                .Width(250)
                                                .Background(
                                                    Colors.Grey.Lighten4
                                                )
                                                .Border(1)
                                                .BorderColor(
                                                    Colors.Grey.Lighten2
                                                )
                                                .Padding(12)
                                                .Column(
                                                    total =>
                                                    {
                                                        total.Item()
                                                            .Row(
                                                                row =>
                                                                {
                                                                    row.RelativeItem()
                                                                        .Text(
                                                                            "Pedidos:"
                                                                        );


                                                                    row.RelativeItem()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            model.TotalPedidos.ToString()
                                                                        )
                                                                        .Bold();
                                                                }
                                                            );


                                                        total.Item()
                                                            .PaddingTop(5)
                                                            .Row(
                                                                row =>
                                                                {
                                                                    row.RelativeItem()
                                                                        .Text(
                                                                            "Unidades:"
                                                                        );


                                                                    row.RelativeItem()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            model.TotalProductosVendidos.ToString()
                                                                        )
                                                                        .Bold();
                                                                }
                                                            );


                                                        total.Item()
                                                            .PaddingTop(7)
                                                            .BorderTop(1)
                                                            .BorderColor(
                                                                Colors.Grey.Lighten2
                                                            )
                                                            .PaddingTop(7)
                                                            .Row(
                                                                row =>
                                                                {
                                                                    row.RelativeItem()
                                                                        .Text(
                                                                            "TOTAL:"
                                                                        )
                                                                        .FontSize(11)
                                                                        .Bold();


                                                                    row.RelativeItem()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            Moneda(
                                                                                model.TotalVentas
                                                                            )
                                                                        )
                                                                        .FontSize(11)
                                                                        .Bold()
                                                                        .FontColor(
                                                                            Colors.Blue.Darken2
                                                                        );
                                                                }
                                                            );
                                                    }
                                                );
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
                                    .PaddingTop(7)
                                    .Row(
                                        row =>
                                        {
                                            row.RelativeItem()
                                                .Text(
                                                    $"BigGame • Ventas del {model.FechaInicio:dd/MM/yyyy} al {model.FechaFin:dd/MM/yyyy}"
                                                )
                                                .FontSize(7)
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
                                                                style.FontSize(7)
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


            return document.GeneratePdf();
        }


        // ==========================================
        // TARJETA DE RESUMEN
        // ==========================================

        private static void Tarjeta(
            IContainer container,
            string titulo,
            string valor,
            string detalle)
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
                            .Text(
                                titulo
                            )
                            .FontSize(7)
                            .FontColor(
                                Colors.Grey.Darken1
                            );


                        column.Item()
                            .PaddingTop(3)
                            .Text(
                                valor
                            )
                            .FontSize(12)
                            .Bold()
                            .FontColor(
                                Colors.Blue.Darken2
                            );


                        column.Item()
                            .PaddingTop(2)
                            .Text(
                                detalle
                            )
                            .FontSize(6)
                            .FontColor(
                                Colors.Grey.Darken1
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
                .PaddingVertical(6)
                .PaddingHorizontal(5);
        }


        // ==========================================
        // FORMATO MONEDA
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
    }
}