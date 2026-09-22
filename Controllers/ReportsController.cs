using EcommerceApp.Data;
using EcommerceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EcommerceApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(
            ApplicationDbContext context)
        {
            _context = context;
        }


        // ==========================================
        // CENTRO DE REPORTES
        // ==========================================

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        // ==========================================
        // REPORTE DE PRODUCTOS
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> ProductosPdf()
        {
            var products =
                await _context.Products
                    .OrderBy(p => p.Name)
                    .ToListAsync();


            var fecha =
                FechaBolivia();


            var totalProductos =
                products.Count;


            var totalUnidades =
                products.Sum(
                    p => p.Stock
                );


            var valorInventario =
                products.Sum(
                    p => p.Price * p.Stock
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
                                            .FontSize(9)
                                            .FontColor(
                                                Colors.Grey.Darken4
                                            )
                                );


                                // ==============================
                                // ENCABEZADO
                                // ==============================

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


                                                        row.ConstantItem(220)
                                                            .AlignRight()
                                                            .Column(
                                                                right =>
                                                                {
                                                                    right.Item()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            "REPORTE DE PRODUCTOS"
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


                                // ==============================
                                // CONTENIDO
                                // ==============================

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
                                                                container =>
                                                                    TarjetaResumen(
                                                                        container,
                                                                        "Productos registrados",
                                                                        totalProductos.ToString()
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(10)
                                                            .Element(
                                                                container =>
                                                                    TarjetaResumen(
                                                                        container,
                                                                        "Unidades en stock",
                                                                        totalUnidades.ToString()
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(10)
                                                            .Element(
                                                                container =>
                                                                    TarjetaResumen(
                                                                        container,
                                                                        "Valor del inventario",
                                                                        $"Bs {valorInventario:N2}"
                                                                    )
                                                            );
                                                    }
                                                );


                                            // ==========================
                                            // TABLA
                                            // ==========================

                                            column.Item()
                                                .Table(
                                                    table =>
                                                    {
                                                        table.ColumnsDefinition(
                                                            columns =>
                                                            {
                                                                columns.ConstantColumn(35);

                                                                columns.RelativeColumn(3);

                                                                columns.RelativeColumn(1.4f);

                                                                columns.RelativeColumn(1);

                                                                columns.ConstantColumn(55);

                                                                columns.RelativeColumn(1.2f);
                                                            }
                                                        );


                                                        // ==================
                                                        // CABECERA TABLA
                                                        // ==================

                                                        table.Header(
                                                            header =>
                                                            {
                                                                header.Cell()
                                                                    .Element(CeldaCabecera)
                                                                    .Text("ID");

                                                                header.Cell()
                                                                    .Element(CeldaCabecera)
                                                                    .Text("Producto");

                                                                header.Cell()
                                                                    .Element(CeldaCabecera)
                                                                    .Text("Categoría");

                                                                header.Cell()
                                                                    .Element(CeldaCabecera)
                                                                    .AlignRight()
                                                                    .Text("Precio");

                                                                header.Cell()
                                                                    .Element(CeldaCabecera)
                                                                    .AlignCenter()
                                                                    .Text("Stock");

                                                                header.Cell()
                                                                    .Element(CeldaCabecera)
                                                                    .AlignRight()
                                                                    .Text("Valor stock");
                                                            }
                                                        );


                                                        // ==================
                                                        // FILAS
                                                        // ==================

                                                        foreach (
                                                            var product
                                                            in products
                                                        )
                                                        {
                                                            table.Cell()
                                                                .Element(CeldaNormal)
                                                                .Text(
                                                                    product.Id.ToString()
                                                                );

                                                            table.Cell()
                                                                .Element(CeldaNormal)
                                                                .Text(
                                                                    product.Name
                                                                );

                                                            table.Cell()
                                                                .Element(CeldaNormal)
                                                                .Text(
                                                                    string.IsNullOrWhiteSpace(
                                                                        product.Category
                                                                    )
                                                                        ? "Sin categoría"
                                                                        : product.Category
                                                                );

                                                            table.Cell()
                                                                .Element(CeldaNormal)
                                                                .AlignRight()
                                                                .Text(
                                                                    $"Bs {product.Price:N2}"
                                                                );

                                                            table.Cell()
                                                                .Element(CeldaNormal)
                                                                .AlignCenter()
                                                                .Text(
                                                                    product.Stock.ToString()
                                                                );

                                                            table.Cell()
                                                                .Element(CeldaNormal)
                                                                .AlignRight()
                                                                .Text(
                                                                    $"Bs {(product.Price * product.Stock):N2}"
                                                                );
                                                        }
                                                    }
                                                );


                                            // ==========================
                                            // TOTAL FINAL
                                            // ==========================

                                            column.Item()
                                                .AlignRight()
                                                .Text(
                                                    $"TOTAL VALOR INVENTARIO: Bs {valorInventario:N2}"
                                                )
                                                .FontSize(11)
                                                .Bold()
                                                .FontColor(
                                                    Colors.Blue.Darken2
                                                );
                                        }
                                    );


                                // ==============================
                                // PIE DE PÁGINA
                                // ==============================

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
                                                    "BigGame • Reporte administrativo"
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
                $"BigGame_Productos_{fecha:yyyyMMdd_HHmm}.pdf";


            return File(
                pdf,
                "application/pdf",
                fileName
            );
        }


        // ==========================================
        // REPORTE DE INVENTARIO
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> InventarioPdf()
        {
            var products =
                await _context.Products
                    .OrderBy(p => p.Stock)
                    .ThenBy(p => p.Name)
                    .ToListAsync();


            var fecha =
                FechaBolivia();


            var totalProductos =
                products.Count;


            var totalUnidades =
                products.Sum(
                    p => p.Stock
                );


            var agotados =
                products.Count(
                    p => p.Stock <= 0
                );


            var stockBajo =
                products.Count(
                    p =>
                        p.Stock > 0
                        &&
                        p.Stock <= 5
                );


            var valorInventario =
                products.Sum(
                    p => p.Price * p.Stock
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
                                            .FontSize(9)
                                            .FontColor(
                                                Colors.Grey.Darken4
                                            )
                                );


                                // ==============================
                                // ENCABEZADO
                                // ==============================

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


                                                        row.ConstantItem(220)
                                                            .AlignRight()
                                                            .Column(
                                                                right =>
                                                                {
                                                                    right.Item()
                                                                        .AlignRight()
                                                                        .Text(
                                                                            "REPORTE DE INVENTARIO"
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


                                // ==============================
                                // CONTENIDO
                                // ==============================

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
                                                                container =>
                                                                    TarjetaResumen(
                                                                        container,
                                                                        "Productos",
                                                                        totalProductos.ToString()
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(8)
                                                            .Element(
                                                                container =>
                                                                    TarjetaResumen(
                                                                        container,
                                                                        "Unidades",
                                                                        totalUnidades.ToString()
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(8)
                                                            .Element(
                                                                container =>
                                                                    TarjetaResumen(
                                                                        container,
                                                                        "Stock bajo",
                                                                        stockBajo.ToString()
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(8)
                                                            .Element(
                                                                container =>
                                                                    TarjetaResumen(
                                                                        container,
                                                                        "Agotados",
                                                                        agotados.ToString()
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(8)
                                                            .Element(
                                                                container =>
                                                                    TarjetaResumen(
                                                                        container,
                                                                        "Valor total",
                                                                        $"Bs {valorInventario:N2}"
                                                                    )
                                                            );
                                                    }
                                                );


                                            // ==========================
                                            // TABLA
                                            // ==========================

                                            column.Item()
                                                .Table(
                                                    table =>
                                                    {
                                                        table.ColumnsDefinition(
                                                            columns =>
                                                            {
                                                                columns.ConstantColumn(35);

                                                                columns.RelativeColumn(3);

                                                                columns.RelativeColumn(1.4f);

                                                                columns.RelativeColumn(1);

                                                                columns.ConstantColumn(55);

                                                                columns.RelativeColumn(1.1f);

                                                                columns.RelativeColumn(1.3f);
                                                            }
                                                        );


                                                        table.Header(
                                                            header =>
                                                            {
                                                                header.Cell()
                                                                    .Element(CeldaCabecera)
                                                                    .Text("ID");

                                                                header.Cell()
                                                                    .Element(CeldaCabecera)
                                                                    .Text("Producto");

                                                                header.Cell()
                                                                    .Element(CeldaCabecera)
                                                                    .Text("Categoría");

                                                                header.Cell()
                                                                    .Element(CeldaCabecera)
                                                                    .AlignRight()
                                                                    .Text("Precio");

                                                                header.Cell()
                                                                    .Element(CeldaCabecera)
                                                                    .AlignCenter()
                                                                    .Text("Stock");

                                                                header.Cell()
                                                                    .Element(CeldaCabecera)
                                                                    .AlignCenter()
                                                                    .Text("Estado");

                                                                header.Cell()
                                                                    .Element(CeldaCabecera)
                                                                    .AlignRight()
                                                                    .Text("Valor");
                                                            }
                                                        );


                                                        foreach (
                                                            var product
                                                            in products
                                                        )
                                                        {
                                                            var estado =
                                                                EstadoStock(
                                                                    product.Stock
                                                                );


                                                            table.Cell()
                                                                .Element(CeldaNormal)
                                                                .Text(
                                                                    product.Id.ToString()
                                                                );

                                                            table.Cell()
                                                                .Element(CeldaNormal)
                                                                .Text(
                                                                    product.Name
                                                                );

                                                            table.Cell()
                                                                .Element(CeldaNormal)
                                                                .Text(
                                                                    string.IsNullOrWhiteSpace(
                                                                        product.Category
                                                                    )
                                                                        ? "Sin categoría"
                                                                        : product.Category
                                                                );

                                                            table.Cell()
                                                                .Element(CeldaNormal)
                                                                .AlignRight()
                                                                .Text(
                                                                    $"Bs {product.Price:N2}"
                                                                );

                                                            table.Cell()
                                                                .Element(CeldaNormal)
                                                                .AlignCenter()
                                                                .Text(
                                                                    product.Stock.ToString()
                                                                );

                                                            table.Cell()
                                                                .Element(CeldaNormal)
                                                                .AlignCenter()
                                                                .Text(
                                                                    estado
                                                                );

                                                            table.Cell()
                                                                .Element(CeldaNormal)
                                                                .AlignRight()
                                                                .Text(
                                                                    $"Bs {(product.Price * product.Stock):N2}"
                                                                );
                                                        }
                                                    }
                                                );
                                        }
                                    );


                                // ==============================
                                // PIE DE PÁGINA
                                // ==============================

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
                                                    "BigGame • Control de inventario"
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
                $"BigGame_Inventario_{fecha:yyyyMMdd_HHmm}.pdf";


            return File(
                pdf,
                "application/pdf",
                fileName
            );
        }


        // ==========================================
        // TARJETAS DE RESUMEN
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
                .Padding(10)
                .Column(
                    column =>
                    {
                        column.Item()
                            .Text(titulo)
                            .FontSize(8)
                            .FontColor(
                                Colors.Grey.Darken1
                            );

                        column.Item()
                            .PaddingTop(3)
                            .Text(valor)
                            .FontSize(14)
                            .Bold()
                            .FontColor(
                                Colors.Blue.Darken2
                            );
                    }
                );
        }


        // ==========================================
        // ESTILO CABECERA TABLA
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
                            .FontSize(8)
                );
        }


        // ==========================================
        // ESTILO CELDAS
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
        // ESTADO DEL STOCK
        // ==========================================

        private static string EstadoStock(
            int stock)
        {
            if (stock <= 0)
            {
                return "AGOTADO";
            }

            if (stock <= 5)
            {
                return "STOCK BAJO";
            }

            return "DISPONIBLE";
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