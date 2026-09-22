using System.Globalization;
using EcommerceApp.Reports.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EcommerceApp.Reports.Renderers
{
    public class ComprobanteCompraPdfRenderer
    {
        public byte[] Render(
            ComprobanteCompraReportModel model)
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

                                page.Margin(30);

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
                                                                        .FontSize(27)
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
                                                                            "COMPROBANTE DE COMPRA"
                                                                        )
                                                                        .FontSize(15)
                                                                        .Bold();


                                                                    right.Item()
                                                                        .PaddingTop(4)
                                                                        .AlignRight()
                                                                        .Text(
                                                                            $"Pedido N.º {model.PedidoId}"
                                                                        )
                                                                        .FontSize(10)
                                                                        .Bold()
                                                                        .FontColor(
                                                                            Colors.Blue.Darken2
                                                                        );


                                                                    right.Item()
                                                                        .PaddingTop(3)
                                                                        .AlignRight()
                                                                        .Text(
                                                                            $"Generado: {model.FechaGeneracion:dd/MM/yyyy HH:mm}"
                                                                        )
                                                                        .FontSize(7)
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
                                    .PaddingVertical(18)
                                    .Column(
                                        column =>
                                        {
                                            column.Spacing(15);


                                            // ==================================
                                            // ESTADO DEL PEDIDO
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
                                                .Row(
                                                    row =>
                                                    {
                                                        row.RelativeItem()
                                                            .Column(
                                                                left =>
                                                                {
                                                                    left.Item()
                                                                        .Text(
                                                                            "Compra registrada correctamente"
                                                                        )
                                                                        .FontSize(12)
                                                                        .Bold()
                                                                        .FontColor(
                                                                            Colors.Blue.Darken2
                                                                        );


                                                                    left.Item()
                                                                        .PaddingTop(4)
                                                                        .Text(
                                                                            $"Fecha de compra: {model.FechaPedido:dd/MM/yyyy HH:mm}"
                                                                        )
                                                                        .FontSize(8);
                                                                }
                                                            );


                                                        row.ConstantItem(120)
                                                            .AlignRight()
                                                            .Column(
                                                                right =>
                                                                {
                                                                    right.Item()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            "Estado"
                                                                        )
                                                                        .FontSize(7)
                                                                        .FontColor(
                                                                            Colors.Grey.Darken1
                                                                        );


                                                                    right.Item()
                                                                        .PaddingTop(3)
                                                                        .AlignRight()
                                                                        .Text(
                                                                            model.Estado
                                                                        )
                                                                        .FontSize(10)
                                                                        .Bold()
                                                                        .FontColor(
                                                                            Colors.Green.Darken2
                                                                        );
                                                                }
                                                            );
                                                    }
                                                );


                                            // ==================================
                                            // DATOS DEL CLIENTE
                                            // ==================================

                                            column.Item()
                                                .Text(
                                                    "Datos del cliente"
                                                )
                                                .FontSize(13)
                                                .Bold();


                                            column.Item()
                                                .Border(1)
                                                .BorderColor(
                                                    Colors.Grey.Lighten2
                                                )
                                                .Background(
                                                    Colors.Grey.Lighten4
                                                )
                                                .Padding(12)
                                                .Column(
                                                    datos =>
                                                    {
                                                        datos.Item()
                                                            .Row(
                                                                row =>
                                                                {
                                                                    row.RelativeItem()
                                                                        .Element(
                                                                            c =>
                                                                                Campo(
                                                                                    c,
                                                                                    "Cliente",
                                                                                    model.Cliente
                                                                                )
                                                                        );


                                                                    row.RelativeItem()
                                                                        .PaddingLeft(12)
                                                                        .Element(
                                                                            c =>
                                                                                Campo(
                                                                                    c,
                                                                                    "Correo",
                                                                                    model.Correo
                                                                                )
                                                                        );
                                                                }
                                                            );


                                                        datos.Item()
                                                            .PaddingTop(10)
                                                            .Element(
                                                                c =>
                                                                    Campo(
                                                                        c,
                                                                        "Dirección de entrega",
                                                                        model.Direccion
                                                                    )
                                                            );
                                                    }
                                                );


                                            // ==================================
                                            // RESUMEN
                                            // ==================================

                                            column.Item()
                                                .Text(
                                                    "Resumen de la compra"
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
                                                                        "Pedido",
                                                                        $"#{model.PedidoId}",
                                                                        "identificador"
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(8)
                                                            .Element(
                                                                c =>
                                                                    Tarjeta(
                                                                        c,
                                                                        "Productos",
                                                                        model.Productos.Count.ToString(),
                                                                        "productos diferentes"
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
                                                                        "unidades compradas"
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(8)
                                                            .Element(
                                                                c =>
                                                                    Tarjeta(
                                                                        c,
                                                                        "Total",
                                                                        Moneda(
                                                                            model.TotalCompra
                                                                        ),
                                                                        "importe pagado"
                                                                    )
                                                            );
                                                    }
                                                );


                                            // ==================================
                                            // DETALLE DE PRODUCTOS
                                            // ==================================

                                            column.Item()
                                                .Text(
                                                    "Detalle de productos"
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
                                                    .Padding(16)
                                                    .AlignCenter()
                                                    .Text(
                                                        "Este pedido no contiene productos."
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

                                                                    columns.RelativeColumn(
                                                                        3
                                                                    );

                                                                    columns.ConstantColumn(
                                                                        65
                                                                    );

                                                                    columns.ConstantColumn(
                                                                        85
                                                                    );

                                                                    columns.ConstantColumn(
                                                                        85
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
                                                                        .AlignCenter()
                                                                        .Text(
                                                                            "Cantidad"
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
                                                                            "Subtotal"
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
                                                                    .AlignCenter()
                                                                    .Text(
                                                                        producto.Cantidad.ToString()
                                                                    );


                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .AlignRight()
                                                                    .Text(
                                                                        Moneda(
                                                                            producto.PrecioUnitario
                                                                        )
                                                                    );


                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .AlignRight()
                                                                    .Text(
                                                                        Moneda(
                                                                            producto.Subtotal
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
                                                .Width(300)
                                                .Border(1)
                                                .BorderColor(
                                                    Colors.Grey.Lighten2
                                                )
                                                .Background(
                                                    Colors.Grey.Lighten4
                                                )
                                                .Padding(14)
                                                .Column(
                                                    total =>
                                                    {
                                                        total.Item()
                                                            .Row(
                                                                row =>
                                                                {
                                                                    row.RelativeItem()
                                                                        .Text(
                                                                            "Unidades compradas:"
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
                                                                            "TOTAL COMPRA:"
                                                                        )
                                                                        .FontSize(11)
                                                                        .Bold();


                                                                    row.RelativeItem()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            Moneda(
                                                                                model.TotalCompra
                                                                            )
                                                                        )
                                                                        .FontSize(12)
                                                                        .Bold()
                                                                        .FontColor(
                                                                            Colors.Blue.Darken2
                                                                        );
                                                                }
                                                            );
                                                    }
                                                );


                                            // ==================================
                                            // NOTA
                                            // ==================================

                                            column.Item()
                                                .PaddingTop(4)
                                                .BorderTop(1)
                                                .BorderColor(
                                                    Colors.Grey.Lighten2
                                                )
                                                .PaddingTop(10)
                                                .Text(
                                                    "Este documento es un comprobante interno de compra generado por BigGame y resume la información registrada en el pedido."
                                                )
                                                .FontSize(7)
                                                .FontColor(
                                                    Colors.Grey.Darken1
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
                                                    $"BigGame • Comprobante pedido #{model.PedidoId}"
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
        // CAMPO
        // ==========================================

        private static void Campo(
            IContainer container,
            string titulo,
            string valor)
        {
            container
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
                                string.IsNullOrWhiteSpace(valor)
                                    ? "No registrado"
                                    : valor
                            )
                            .FontSize(9)
                            .Bold();
                    }
                );
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
        // CABECERA TABLA
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