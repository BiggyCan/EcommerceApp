using System.Globalization;
using EcommerceApp.Reports.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EcommerceApp.Reports.Renderers
{
    public class CarritosAbandonadosPdfRenderer
    {
        public byte[] Render(
            CarritosAbandonadosReportModel model)
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


                                                        row.ConstantItem(265)
                                                            .AlignRight()
                                                            .Column(
                                                                right =>
                                                                {
                                                                    right.Item()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            "CARRITOS ABANDONADOS"
                                                                        )
                                                                        .FontSize(14)
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


                                // ==========================================
                                // CONTENIDO
                                // ==========================================

                                page.Content()
                                    .PaddingVertical(16)
                                    .Column(
                                        column =>
                                        {
                                            column.Spacing(14);


                                            // ==================================
                                            // DESCRIPCIÓN
                                            // ==================================

                                            column.Item()
                                                .Background(
                                                    Colors.Orange.Lighten5
                                                )
                                                .Border(1)
                                                .BorderColor(
                                                    Colors.Orange.Lighten3
                                                )
                                                .Padding(12)
                                                .Column(
                                                    box =>
                                                    {
                                                        box.Item()
                                                            .Text(
                                                                "Seguimiento de carritos no convertidos"
                                                            )
                                                            .FontSize(12)
                                                            .Bold()
                                                            .FontColor(
                                                                Colors.Orange.Darken3
                                                            );


                                                        box.Item()
                                                            .PaddingTop(4)
                                                            .Text(
                                                                $"Se consideran abandonados los carritos que permanecen activos durante {model.MinutosAbandono} minutos o más sin registrar actividad."
                                                            )
                                                            .FontSize(8)
                                                            .FontColor(
                                                                Colors.Grey.Darken2
                                                            );
                                                    }
                                                );


                                            // ==================================
                                            // RESUMEN
                                            // ==================================

                                            column.Item()
                                                .Text(
                                                    "Resumen"
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
                                                                        "Carritos",
                                                                        model.TotalCarritos.ToString(),
                                                                        "abandonados",
                                                                        Colors.Red.Darken2
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(8)
                                                            .Element(
                                                                c =>
                                                                    Tarjeta(
                                                                        c,
                                                                        "Unidades",
                                                                        model.TotalUnidades.ToString(),
                                                                        "productos sin comprar",
                                                                        Colors.Orange.Darken3
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(8)
                                                            .Element(
                                                                c =>
                                                                    Tarjeta(
                                                                        c,
                                                                        "Valor potencial",
                                                                        Moneda(
                                                                            model.ValorTotalAbandonado
                                                                        ),
                                                                        "ventas no convertidas",
                                                                        Colors.Blue.Darken2
                                                                    )
                                                            );
                                                    }
                                                );


                                            // ==================================
                                            // TABLA
                                            // ==================================

                                            column.Item()
                                                .Text(
                                                    "Detalle de carritos abandonados"
                                                )
                                                .FontSize(13)
                                                .Bold();


                                            if (
                                                model.Carritos.Count == 0
                                            )
                                            {
                                                column.Item()
                                                    .Border(1)
                                                    .BorderColor(
                                                        Colors.Green.Lighten2
                                                    )
                                                    .Background(
                                                        Colors.Green.Lighten5
                                                    )
                                                    .Padding(18)
                                                    .AlignCenter()
                                                    .Text(
                                                        $"No existen carritos con {model.MinutosAbandono} minutos o más de inactividad."
                                                    )
                                                    .FontSize(10)
                                                    .Bold()
                                                    .FontColor(
                                                        Colors.Green.Darken2
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
                                                                        32
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        1.35f
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        1.8f
                                                                    );

                                                                    columns.ConstantColumn(
                                                                        42
                                                                    );

                                                                    columns.ConstantColumn(
                                                                        72
                                                                    );

                                                                    columns.ConstantColumn(
                                                                        67
                                                                    );

                                                                    columns.ConstantColumn(
                                                                        63
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
                                                                            "ID"
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
                                                                            "Productos"
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
                                                                            "Valor"
                                                                        );


                                                                    header.Cell()
                                                                        .Element(
                                                                            CeldaCabecera
                                                                        )
                                                                        .AlignCenter()
                                                                        .Text(
                                                                            "Última actividad"
                                                                        );


                                                                    header.Cell()
                                                                        .Element(
                                                                            CeldaCabecera
                                                                        )
                                                                        .AlignCenter()
                                                                        .Text(
                                                                            "Inactividad"
                                                                        );
                                                                }
                                                            );


                                                            foreach (
                                                                var cart
                                                                in model.Carritos
                                                            )
                                                            {
                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .Text(
                                                                        cart.CarritoId.ToString()
                                                                    );


                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .Column(
                                                                        c =>
                                                                        {
                                                                            c.Item()
                                                                                .Text(
                                                                                    cart.Usuario
                                                                                )
                                                                                .Bold()
                                                                                .FontSize(7);


                                                                            c.Item()
                                                                                .PaddingTop(2)
                                                                                .Text(
                                                                                    cart.Correo
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
                                                                        cart.Productos
                                                                    )
                                                                    .FontSize(6.5f);


                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .AlignCenter()
                                                                    .Text(
                                                                        cart.TotalItems.ToString()
                                                                    )
                                                                    .Bold();


                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .AlignRight()
                                                                    .Text(
                                                                        Moneda(
                                                                            cart.TotalAmount
                                                                        )
                                                                    )
                                                                    .Bold();


                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .AlignCenter()
                                                                    .Text(
                                                                        cart.LastActivityAt
                                                                            .ToString(
                                                                                "dd/MM/yy\nHH:mm"
                                                                            )
                                                                    )
                                                                    .FontSize(6.5f);


                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .AlignCenter()
                                                                    .Text(
                                                                        $"{cart.MinutosSinActividad:N0} min"
                                                                    )
                                                                    .Bold()
                                                                    .FontColor(
                                                                        Colors.Red.Darken2
                                                                    );
                                                            }
                                                        }
                                                    );
                                            }


                                            // ==================================
                                            // INFORMACIÓN ADMINISTRATIVA
                                            // ==================================

                                            column.Item()
                                                .Background(
                                                    Colors.Grey.Lighten4
                                                )
                                                .Border(1)
                                                .BorderColor(
                                                    Colors.Grey.Lighten2
                                                )
                                                .Padding(11)
                                                .Column(
                                                    info =>
                                                    {
                                                        info.Item()
                                                            .Text(
                                                                "Criterio del reporte"
                                                            )
                                                            .FontSize(9)
                                                            .Bold();


                                                        info.Item()
                                                            .PaddingTop(5)
                                                            .Text(
                                                                $"Un carrito pasa de Activo a Abandonado cuando su última actividad supera el límite de {model.MinutosAbandono} minutos y no fue convertido en pedido."
                                                            )
                                                            .FontSize(7)
                                                            .FontColor(
                                                                Colors.Grey.Darken2
                                                            );


                                                        info.Item()
                                                            .PaddingTop(4)
                                                            .Text(
                                                                "Los carritos que completan una compra se registran como Convertidos y no se incluyen en este reporte."
                                                            )
                                                            .FontSize(7)
                                                            .FontColor(
                                                                Colors.Grey.Darken2
                                                            );
                                                    }
                                                );


                                            // ==================================
                                            // TOTAL FINAL
                                            // ==================================

                                            column.Item()
                                                .AlignRight()
                                                .Width(300)
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
                                                                            "Carritos abandonados:"
                                                                        );


                                                                    row.RelativeItem()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            model.TotalCarritos.ToString()
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
                                                                            model.TotalUnidades.ToString()
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
                                                                            "VALOR POTENCIAL:"
                                                                        )
                                                                        .FontSize(10)
                                                                        .Bold();


                                                                    row.RelativeItem()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            Moneda(
                                                                                model.ValorTotalAbandonado
                                                                            )
                                                                        )
                                                                        .FontSize(10)
                                                                        .Bold()
                                                                        .FontColor(
                                                                            Colors.Red.Darken2
                                                                        );
                                                                }
                                                            );
                                                    }
                                                );
                                        }
                                    );


                                // ==========================================
                                // PIE DE PÁGINA
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
                                                    $"BigGame • Carritos abandonados • Límite: {model.MinutosAbandono} min"
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
        // TARJETA
        // ==========================================

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
                .Padding(10)
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
                                color
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
                .PaddingHorizontal(4)
                .DefaultTextStyle(
                    style =>
                        style
                            .FontColor(
                                Colors.White
                            )
                            .Bold()
                            .FontSize(6.3f)
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
                .PaddingHorizontal(4);
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
    }
}