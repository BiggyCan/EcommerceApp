using System.Globalization;
using EcommerceApp.Reports.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EcommerceApp.Reports.Renderers
{
    public class ProductosMasVendidosPdfRenderer
    {
        public byte[] Render(
            ProductosMasVendidosReportModel model)
        {
            var document =
                Document.Create(
                    container =>
                    {
                        container.Page(
                            page =>
                            {
                                page.Size(PageSizes.A4);

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


                                                        row.ConstantItem(250)
                                                            .AlignRight()
                                                            .Column(
                                                                right =>
                                                                {
                                                                    right.Item()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            "PRODUCTOS MÁS VENDIDOS"
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
                                            // INTRODUCCIÓN
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
                                                                "Ranking de productos"
                                                            )
                                                            .FontSize(12)
                                                            .Bold()
                                                            .FontColor(
                                                                Colors.Blue.Darken2
                                                            );


                                                        box.Item()
                                                            .PaddingTop(4)
                                                            .Text(
                                                                "Productos ordenados de mayor a menor según la cantidad total de unidades vendidas registradas en BigGame."
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
                                                                        "Productos vendidos",
                                                                        model.TotalProductosDiferentes.ToString(),
                                                                        "productos diferentes"
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(8)
                                                            .Element(
                                                                c =>
                                                                    Tarjeta(
                                                                        c,
                                                                        "Unidades vendidas",
                                                                        model.TotalUnidadesVendidas.ToString(),
                                                                        "unidades totales"
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(8)
                                                            .Element(
                                                                c =>
                                                                    Tarjeta(
                                                                        c,
                                                                        "Ingresos",
                                                                        Moneda(
                                                                            model.TotalIngresos
                                                                        ),
                                                                        "ventas acumuladas"
                                                                    )
                                                            );
                                                    }
                                                );


                                            // ==================================
                                            // TABLA
                                            // ==================================

                                            column.Item()
                                                .Text(
                                                    "Clasificación por ventas"
                                                )
                                                .FontSize(13)
                                                .Bold();


                                            if (
                                                model.Productos.Count == 0
                                            )
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
                                                        "Todavía no existen productos vendidos para generar el ranking."
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
                                                                        40
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        3
                                                                    );

                                                                    columns.ConstantColumn(
                                                                        70
                                                                    );

                                                                    columns.ConstantColumn(
                                                                        90
                                                                    );

                                                                    columns.ConstantColumn(
                                                                        90
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
                                                                        .AlignCenter()
                                                                        .Text(
                                                                            "#"
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
                                                                            "Vendidos"
                                                                        );


                                                                    header.Cell()
                                                                        .Element(
                                                                            CeldaCabecera
                                                                        )
                                                                        .AlignRight()
                                                                        .Text(
                                                                            "Precio prom."
                                                                        );


                                                                    header.Cell()
                                                                        .Element(
                                                                            CeldaCabecera
                                                                        )
                                                                        .AlignRight()
                                                                        .Text(
                                                                            "Generado"
                                                                        );
                                                                }
                                                            );


                                                            foreach (
                                                                var producto
                                                                in model.Productos
                                                            )
                                                            {
                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .AlignCenter()
                                                                    .Text(
                                                                        producto.Posicion.ToString()
                                                                    )
                                                                    .Bold()
                                                                    .FontColor(
                                                                        producto.Posicion <= 3
                                                                            ? Colors.Blue.Darken2
                                                                            : Colors.Grey.Darken3
                                                                    );


                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .Column(
                                                                        productColumn =>
                                                                        {
                                                                            productColumn.Item()
                                                                                .Text(
                                                                                    producto.Producto
                                                                                )
                                                                                .Bold();


                                                                            productColumn.Item()
                                                                                .PaddingTop(2)
                                                                                .Text(
                                                                                    $"ID: {producto.ProductId}"
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
                                                                    .AlignCenter()
                                                                    .Text(
                                                                        producto.CantidadVendida.ToString()
                                                                    )
                                                                    .Bold();


                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .AlignRight()
                                                                    .Text(
                                                                        Moneda(
                                                                            producto.PrecioPromedio
                                                                        )
                                                                    );


                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .AlignRight()
                                                                    .Text(
                                                                        Moneda(
                                                                            producto.TotalGenerado
                                                                        )
                                                                    )
                                                                    .Bold();
                                                            }
                                                        }
                                                    );
                                            }


                                            // ==================================
                                            // RESUMEN FINAL
                                            // ==================================

                                            column.Item()
                                                .AlignRight()
                                                .Width(280)
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
                                                                            "Productos diferentes:"
                                                                        );


                                                                    row.RelativeItem()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            model.TotalProductosDiferentes.ToString()
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
                                                                            "Unidades vendidas:"
                                                                        );


                                                                    row.RelativeItem()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            model.TotalUnidadesVendidas.ToString()
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
                                                                            "TOTAL GENERADO:"
                                                                        )
                                                                        .FontSize(10)
                                                                        .Bold();


                                                                    row.RelativeItem()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            Moneda(
                                                                                model.TotalIngresos
                                                                            )
                                                                        )
                                                                        .FontSize(10)
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
                                                    $"BigGame • Productos más vendidos • {model.FechaGeneracion:dd/MM/yyyy HH:mm}"
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
                            .FontSize(13)
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
                .PaddingVertical(7)
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
        // CELDA
        // ==========================================

        private static IContainer CeldaNormal(
            IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor(
                    Colors.Grey.Lighten2
                )
                .PaddingVertical(7)
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
    }
}