using System.Globalization;
using EcommerceApp.Reports.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EcommerceApp.Reports.Renderers
{
    public class InventarioStockBajoPdfRenderer
    {
        public byte[] Render(
            InventarioStockBajoReportModel model)
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


                                                        row.ConstantItem(260)
                                                            .AlignRight()
                                                            .Column(
                                                                right =>
                                                                {
                                                                    right.Item()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            "INVENTARIO CON STOCK BAJO"
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
                                                                "Alerta de inventario"
                                                            )
                                                            .FontSize(12)
                                                            .Bold()
                                                            .FontColor(
                                                                Colors.Orange.Darken3
                                                            );


                                                        box.Item()
                                                            .PaddingTop(4)
                                                            .Text(
                                                                $"Este reporte muestra los productos con {model.LimiteStockBajo} unidades o menos disponibles en inventario."
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
                                                    "Resumen del inventario"
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
                                                                        "Productos analizados",
                                                                        model.TotalProductosAnalizados.ToString(),
                                                                        "catálogo total",
                                                                        Colors.Blue.Darken2
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(7)
                                                            .Element(
                                                                c =>
                                                                    Tarjeta(
                                                                        c,
                                                                        "Stock bajo",
                                                                        model.ProductosConStockBajo.ToString(),
                                                                        "productos en alerta",
                                                                        Colors.Orange.Darken3
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(7)
                                                            .Element(
                                                                c =>
                                                                    Tarjeta(
                                                                        c,
                                                                        "Agotados",
                                                                        model.ProductosAgotados.ToString(),
                                                                        "sin existencias",
                                                                        Colors.Red.Darken2
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(7)
                                                            .Element(
                                                                c =>
                                                                    Tarjeta(
                                                                        c,
                                                                        "Unidades",
                                                                        model.UnidadesDisponibles.ToString(),
                                                                        "en productos alerta",
                                                                        Colors.Green.Darken2
                                                                    )
                                                            );
                                                    }
                                                );


                                            // ==================================
                                            // VALOR INVENTARIO
                                            // ==================================

                                            column.Item()
                                                .Background(
                                                    Colors.Grey.Lighten4
                                                )
                                                .Border(1)
                                                .BorderColor(
                                                    Colors.Grey.Lighten2
                                                )
                                                .Padding(12)
                                                .Row(
                                                    row =>
                                                    {
                                                        row.RelativeItem()
                                                            .Column(
                                                                left =>
                                                                {
                                                                    left.Item()
                                                                        .Text(
                                                                            "Valor del inventario en alerta"
                                                                        )
                                                                        .FontSize(8)
                                                                        .FontColor(
                                                                            Colors.Grey.Darken1
                                                                        );


                                                                    left.Item()
                                                                        .PaddingTop(3)
                                                                        .Text(
                                                                            Moneda(
                                                                                model.ValorInventario
                                                                            )
                                                                        )
                                                                        .FontSize(16)
                                                                        .Bold()
                                                                        .FontColor(
                                                                            Colors.Blue.Darken2
                                                                        );
                                                                }
                                                            );


                                                        row.RelativeItem()
                                                            .AlignRight()
                                                            .Column(
                                                                right =>
                                                                {
                                                                    right.Item()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            $"Límite configurado: {model.LimiteStockBajo}"
                                                                        )
                                                                        .FontSize(8)
                                                                        .Bold();


                                                                    right.Item()
                                                                        .PaddingTop(4)
                                                                        .AlignRight()
                                                                        .Text(
                                                                            "Ordenado desde el stock más crítico."
                                                                        )
                                                                        .FontSize(7)
                                                                        .FontColor(
                                                                            Colors.Grey.Darken1
                                                                        );
                                                                }
                                                            );
                                                    }
                                                );


                                            // ==================================
                                            // TABLA
                                            // ==================================

                                            column.Item()
                                                .Text(
                                                    "Productos con stock bajo"
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
                                                        Colors.Green.Lighten2
                                                    )
                                                    .Background(
                                                        Colors.Green.Lighten5
                                                    )
                                                    .Padding(18)
                                                    .AlignCenter()
                                                    .Text(
                                                        $"No existen productos con {model.LimiteStockBajo} unidades o menos."
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
                                                                        35
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        2.6f
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        1.2f
                                                                    );

                                                                    columns.ConstantColumn(
                                                                        50
                                                                    );

                                                                    columns.ConstantColumn(
                                                                        65
                                                                    );

                                                                    columns.ConstantColumn(
                                                                        75
                                                                    );

                                                                    columns.ConstantColumn(
                                                                        65
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
                                                                            "Producto"
                                                                        );


                                                                    header.Cell()
                                                                        .Element(
                                                                            CeldaCabecera
                                                                        )
                                                                        .Text(
                                                                            "Categoría"
                                                                        );


                                                                    header.Cell()
                                                                        .Element(
                                                                            CeldaCabecera
                                                                        )
                                                                        .AlignCenter()
                                                                        .Text(
                                                                            "Stock"
                                                                        );


                                                                    header.Cell()
                                                                        .Element(
                                                                            CeldaCabecera
                                                                        )
                                                                        .AlignRight()
                                                                        .Text(
                                                                            "Precio"
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
                                                                            "Estado"
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
                                                                    .Text(
                                                                        producto.ProductId.ToString()
                                                                    );


                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .Text(
                                                                        producto.Producto
                                                                    )
                                                                    .Bold();


                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .Text(
                                                                        producto.Categoria
                                                                    )
                                                                    .FontSize(7);


                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .AlignCenter()
                                                                    .Text(
                                                                        producto.Stock.ToString()
                                                                    )
                                                                    .Bold()
                                                                    .FontColor(
                                                                        ColorEstado(
                                                                            producto.Estado
                                                                        )
                                                                    );


                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .AlignRight()
                                                                    .Text(
                                                                        Moneda(
                                                                            producto.Precio
                                                                        )
                                                                    );


                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .AlignRight()
                                                                    .Text(
                                                                        Moneda(
                                                                            producto.ValorStock
                                                                        )
                                                                    );


                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .AlignCenter()
                                                                    .Text(
                                                                        producto.Estado
                                                                    )
                                                                    .Bold()
                                                                    .FontSize(7)
                                                                    .FontColor(
                                                                        ColorEstado(
                                                                            producto.Estado
                                                                        )
                                                                    );
                                                            }
                                                        }
                                                    );
                                            }


                                            // ==================================
                                            // LEYENDA
                                            // ==================================

                                            column.Item()
                                                .Background(
                                                    Colors.Grey.Lighten4
                                                )
                                                .Border(1)
                                                .BorderColor(
                                                    Colors.Grey.Lighten2
                                                )
                                                .Padding(10)
                                                .Column(
                                                    legend =>
                                                    {
                                                        legend.Item()
                                                            .Text(
                                                                "Niveles de alerta"
                                                            )
                                                            .FontSize(9)
                                                            .Bold();


                                                        legend.Item()
                                                            .PaddingTop(5)
                                                            .Text(
                                                                "0 unidades: Agotado"
                                                            )
                                                            .FontSize(7)
                                                            .FontColor(
                                                                Colors.Red.Darken2
                                                            );


                                                        legend.Item()
                                                            .PaddingTop(2)
                                                            .Text(
                                                                "1 a 3 unidades: Crítico"
                                                            )
                                                            .FontSize(7)
                                                            .FontColor(
                                                                Colors.Red.Darken1
                                                            );


                                                        legend.Item()
                                                            .PaddingTop(2)
                                                            .Text(
                                                                "4 a 5 unidades: Muy bajo"
                                                            )
                                                            .FontSize(7)
                                                            .FontColor(
                                                                Colors.Orange.Darken3
                                                            );


                                                        legend.Item()
                                                            .PaddingTop(2)
                                                            .Text(
                                                                $"6 a {model.LimiteStockBajo} unidades: Stock bajo"
                                                            )
                                                            .FontSize(7)
                                                            .FontColor(
                                                                Colors.Orange.Darken2
                                                            );
                                                    }
                                                );


                                            // ==================================
                                            // RESUMEN FINAL
                                            // ==================================

                                            column.Item()
                                                .AlignRight()
                                                .Width(290)
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
                                                                            "Productos en alerta:"
                                                                        );


                                                                    row.RelativeItem()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            model.Productos.Count.ToString()
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
                                                                            "Productos agotados:"
                                                                        );


                                                                    row.RelativeItem()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            model.ProductosAgotados.ToString()
                                                                        )
                                                                        .Bold()
                                                                        .FontColor(
                                                                            Colors.Red.Darken2
                                                                        );
                                                                }
                                                            );


                                                        total.Item()
                                                            .PaddingTop(5)
                                                            .Row(
                                                                row =>
                                                                {
                                                                    row.RelativeItem()
                                                                        .Text(
                                                                            "Unidades restantes:"
                                                                        );


                                                                    row.RelativeItem()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            model.UnidadesDisponibles.ToString()
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
                                                                            "VALOR TOTAL:"
                                                                        )
                                                                        .FontSize(10)
                                                                        .Bold();


                                                                    row.RelativeItem()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            Moneda(
                                                                                model.ValorInventario
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
                                                    $"BigGame • Inventario con stock bajo • Límite: {model.LimiteStockBajo}"
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
        // CABECERA
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
                            .FontSize(6.5f)
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
        // COLOR DEL ESTADO
        // ==========================================

        private static string ColorEstado(
            string estado)
        {
            return estado switch
            {
                "Agotado" =>
                    Colors.Red.Darken2,

                "Crítico" =>
                    Colors.Red.Darken1,

                "Muy bajo" =>
                    Colors.Orange.Darken3,

                _ =>
                    Colors.Orange.Darken2
            };
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