using System.Globalization;
using EcommerceApp.Reports.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EcommerceApp.Reports.Renderers
{
    public class ConciliacionPagosPdfRenderer
    {
        public byte[] Render(
            ConciliacionPagosReportModel model)
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
                                            .FontSize(8)
                                            .FontColor(
                                                Colors.Grey.Darken4
                                            )
                                );


                                // ==========================================
                                // ENCABEZADO
                                // ==========================================

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


                                                        row.ConstantItem(270)
                                                            .AlignRight()
                                                            .Column(
                                                                right =>
                                                                {
                                                                    right.Item()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            "CONCILIACIÓN DE PAGOS"
                                                                        )
                                                                        .FontSize(14)
                                                                        .Bold();


                                                                    right.Item()
                                                                        .PaddingTop(3)
                                                                        .AlignRight()
                                                                        .Text(
                                                                            $"Generado: {model.FechaGeneracion:dd/MM/yyyy HH:mm}"
                                                                        )
                                                                        .FontSize(8);
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


                                // ==========================================
                                // CONTENIDO
                                // ==========================================

                                page.Content()
                                    .PaddingVertical(16)
                                    .Column(
                                        column =>
                                        {
                                            column.Spacing(13);


                                            // ==================================
                                            // INFORMACIÓN
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
                                                    info =>
                                                    {
                                                        info.Item()
                                                            .Text(
                                                                "Conciliación administrativa"
                                                            )
                                                            .FontSize(11)
                                                            .Bold()
                                                            .FontColor(
                                                                Colors.Blue.Darken2
                                                            );


                                                        info.Item()
                                                            .PaddingTop(4)
                                                            .Text(
                                                                $"El reporte calcula una comisión administrativa estimada del {model.PorcentajeComision:N2}% sobre las ventas registradas."
                                                            )
                                                            .FontSize(8);
                                                    }
                                                );


                                            // ==================================
                                            // RESUMEN
                                            // ==================================

                                            column.Item()
                                                .Text(
                                                    "Resumen financiero"
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
                                                                        "ventas registradas",
                                                                        Colors.Blue.Darken2
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(7)
                                                            .Element(
                                                                c =>
                                                                    Tarjeta(
                                                                        c,
                                                                        "Bruto",
                                                                        Moneda(
                                                                            model.TotalBruto
                                                                        ),
                                                                        "total vendido",
                                                                        Colors.Blue.Darken2
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(7)
                                                            .Element(
                                                                c =>
                                                                    Tarjeta(
                                                                        c,
                                                                        "Comisiones",
                                                                        Moneda(
                                                                            model.TotalComisiones
                                                                        ),
                                                                        "costo estimado",
                                                                        Colors.Red.Darken2
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(7)
                                                            .Element(
                                                                c =>
                                                                    Tarjeta(
                                                                        c,
                                                                        "Neto",
                                                                        Moneda(
                                                                            model.TotalNeto
                                                                        ),
                                                                        "importe estimado",
                                                                        Colors.Green.Darken2
                                                                    )
                                                            );
                                                    }
                                                );


                                            // ==================================
                                            // TABLA
                                            // ==================================

                                            column.Item()
                                                .Text(
                                                    "Detalle de conciliación"
                                                )
                                                .FontSize(13)
                                                .Bold();


                                            if (
                                                model.Movimientos.Count == 0
                                            )
                                            {
                                                column.Item()
                                                    .Border(1)
                                                    .BorderColor(
                                                        Colors.Grey.Lighten2
                                                    )
                                                    .Padding(20)
                                                    .AlignCenter()
                                                    .Text(
                                                        "No existen ventas registradas para conciliar."
                                                    )
                                                    .Bold();
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
                                                                        38
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        1.6f
                                                                    );

                                                                    columns.ConstantColumn(
                                                                        65
                                                                    );

                                                                    columns.ConstantColumn(
                                                                        80
                                                                    );

                                                                    columns.ConstantColumn(
                                                                        80
                                                                    );

                                                                    columns.ConstantColumn(
                                                                        80
                                                                    );
                                                                }
                                                            );


                                                            table.Header(
                                                                header =>
                                                                {
                                                                    header.Cell()
                                                                        .Element(
                                                                            Cabecera
                                                                        )
                                                                        .Text(
                                                                            "Pedido"
                                                                        );


                                                                    header.Cell()
                                                                        .Element(
                                                                            Cabecera
                                                                        )
                                                                        .Text(
                                                                            "Cliente"
                                                                        );


                                                                    header.Cell()
                                                                        .Element(
                                                                            Cabecera
                                                                        )
                                                                        .AlignCenter()
                                                                        .Text(
                                                                            "Fecha"
                                                                        );


                                                                    header.Cell()
                                                                        .Element(
                                                                            Cabecera
                                                                        )
                                                                        .AlignRight()
                                                                        .Text(
                                                                            "Bruto"
                                                                        );


                                                                    header.Cell()
                                                                        .Element(
                                                                            Cabecera
                                                                        )
                                                                        .AlignRight()
                                                                        .Text(
                                                                            $"Comisión\n{model.PorcentajeComision:N2}%"
                                                                        );


                                                                    header.Cell()
                                                                        .Element(
                                                                            Cabecera
                                                                        )
                                                                        .AlignRight()
                                                                        .Text(
                                                                            "Neto"
                                                                        );
                                                                }
                                                            );


                                                            foreach (
                                                                var item
                                                                in model.Movimientos
                                                            )
                                                            {
                                                                table.Cell()
                                                                    .Element(
                                                                        Celda
                                                                    )
                                                                    .Text(
                                                                        $"#{item.PedidoId}"
                                                                    )
                                                                    .Bold();


                                                                table.Cell()
                                                                    .Element(
                                                                        Celda
                                                                    )
                                                                    .Column(
                                                                        c =>
                                                                        {
                                                                            c.Item()
                                                                                .Text(
                                                                                    item.Cliente
                                                                                )
                                                                                .Bold();


                                                                            c.Item()
                                                                                .PaddingTop(2)
                                                                                .Text(
                                                                                    item.Correo
                                                                                )
                                                                                .FontSize(6)
                                                                                .FontColor(
                                                                                    Colors.Grey.Darken1
                                                                                );


                                                                            c.Item()
                                                                                .PaddingTop(2)
                                                                                .Text(
                                                                                    item.EstadoPedido
                                                                                )
                                                                                .FontSize(6)
                                                                                .FontColor(
                                                                                    Colors.Green.Darken2
                                                                                );
                                                                        }
                                                                    );


                                                                table.Cell()
                                                                    .Element(
                                                                        Celda
                                                                    )
                                                                    .AlignCenter()
                                                                    .Text(
                                                                        item.Fecha
                                                                            .ToString(
                                                                                "dd/MM/yy\nHH:mm"
                                                                            )
                                                                    )
                                                                    .FontSize(6.5f);


                                                                table.Cell()
                                                                    .Element(
                                                                        Celda
                                                                    )
                                                                    .AlignRight()
                                                                    .Text(
                                                                        Moneda(
                                                                            item.ImporteBruto
                                                                        )
                                                                    );


                                                                table.Cell()
                                                                    .Element(
                                                                        Celda
                                                                    )
                                                                    .AlignRight()
                                                                    .Text(
                                                                        Moneda(
                                                                            item.Comision
                                                                        )
                                                                    )
                                                                    .FontColor(
                                                                        Colors.Red.Darken2
                                                                    );


                                                                table.Cell()
                                                                    .Element(
                                                                        Celda
                                                                    )
                                                                    .AlignRight()
                                                                    .Text(
                                                                        Moneda(
                                                                            item.ImporteNeto
                                                                        )
                                                                    )
                                                                    .Bold()
                                                                    .FontColor(
                                                                        Colors.Green.Darken2
                                                                    );
                                                            }
                                                        }
                                                    );
                                            }


                                            // ==================================
                                            // TOTALES
                                            // ==================================

                                            column.Item()
                                                .AlignRight()
                                                .Width(330)
                                                .Background(
                                                    Colors.Grey.Lighten4
                                                )
                                                .Border(1)
                                                .BorderColor(
                                                    Colors.Grey.Lighten2
                                                )
                                                .Padding(13)
                                                .Column(
                                                    total =>
                                                    {
                                                        total.Item()
                                                            .Row(
                                                                row =>
                                                                {
                                                                    row.RelativeItem()
                                                                        .Text(
                                                                            "Total bruto:"
                                                                        );


                                                                    row.RelativeItem()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            Moneda(
                                                                                model.TotalBruto
                                                                            )
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
                                                                            "Comisiones:"
                                                                        );


                                                                    row.RelativeItem()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            Moneda(
                                                                                model.TotalComisiones
                                                                            )
                                                                        )
                                                                        .Bold()
                                                                        .FontColor(
                                                                            Colors.Red.Darken2
                                                                        );
                                                                }
                                                            );


                                                        total.Item()
                                                            .PaddingTop(8)
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
                                                                            "TOTAL NETO:"
                                                                        )
                                                                        .FontSize(10)
                                                                        .Bold();


                                                                    row.RelativeItem()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            Moneda(
                                                                                model.TotalNeto
                                                                            )
                                                                        )
                                                                        .FontSize(11)
                                                                        .Bold()
                                                                        .FontColor(
                                                                            Colors.Green.Darken2
                                                                        );
                                                                }
                                                            );
                                                    }
                                                );


                                            // ==================================
                                            // NOTA
                                            // ==================================

                                            column.Item()
                                                .Background(
                                                    Colors.Grey.Lighten4
                                                )
                                                .Padding(10)
                                                .Text(
                                                    "Nota: esta conciliación es administrativa y utiliza una comisión configurable aplicada sobre los pedidos registrados en BigGame. No representa una liquidación bancaria o tributaria externa."
                                                )
                                                .FontSize(7)
                                                .FontColor(
                                                    Colors.Grey.Darken2
                                                );
                                        }
                                    );


                                // ==========================================
                                // PIE
                                // ==========================================

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
                                                    $"BigGame • Conciliación • Comisión {model.PorcentajeComision:N2}%"
                                                )
                                                .FontSize(7);


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


        private static void Tarjeta(
            IContainer container,
            string titulo,
            string valor,
            string detalle,
            string color)
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
                            .FontSize(11)
                            .Bold()
                            .FontColor(
                                color
                            );


                        column.Item()
                            .PaddingTop(2)
                            .Text(
                                detalle
                            )
                            .FontSize(6);
                    }
                );
        }


        private static IContainer Cabecera(
            IContainer container)
        {
            return container
                .Background(
                    Colors.Grey.Darken4
                )
                .PaddingVertical(6)
                .PaddingHorizontal(4)
                .DefaultTextStyle(
                    style =>
                        style
                            .FontColor(
                                Colors.White
                            )
                            .Bold()
                            .FontSize(6.5f)
                );
        }


        private static IContainer Celda(
            IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor(
                    Colors.Grey.Lighten2
                )
                .PaddingVertical(6)
                .PaddingHorizontal(4);
        }


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