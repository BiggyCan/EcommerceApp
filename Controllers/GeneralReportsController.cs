using System.Globalization;
using EcommerceApp.Data;
using EcommerceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EcommerceApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GeneralReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GeneralReportsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // ==========================================
        // REPORTE GENERAL
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> ReporteGeneralPdf()
        {
            var products =
                await _context.Products
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();


            var orders =
                await _context.Orders
                    .Include(o => o.Items)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();


            var users =
                await _userManager.Users
                    .OrderByDescending(u => u.CreatedAt)
                    .ToListAsync();


            var admins =
                await _userManager
                    .GetUsersInRoleAsync("Admin");


            var normalUsers =
                await _userManager
                    .GetUsersInRoleAsync("User");


            var fecha =
                FechaBolivia();


            // ==========================================
            // PRODUCTOS E INVENTARIO
            // ==========================================

            var totalProductos =
                products.Count;


            var stockTotal =
                products.Sum(
                    p => p.Stock
                );


            var productosAgotados =
                products.Count(
                    p => p.Stock <= 0
                );


            var stockBajo =
                products.Count(
                    p =>
                        p.Stock > 0 &&
                        p.Stock <= 5
                );


            var valorInventario =
                products.Sum(
                    p => p.Price * p.Stock
                );


            // ==========================================
            // VENTAS Y PEDIDOS
            // ==========================================

            var totalPedidos =
                orders.Count;


            var totalVentas =
                orders.Sum(
                    o => o.TotalAmount
                );


            var unidadesVendidas =
                orders.Sum(
                    o =>
                        o.Items.Sum(
                            i => i.Quantity
                        )
                );


            var promedioVenta =
                totalPedidos > 0
                    ? totalVentas / totalPedidos
                    : 0;


            var pedidosConfirmados =
                orders.Count(
                    o =>
                        o.Status.Equals(
                            "Confirmado",
                            StringComparison.OrdinalIgnoreCase
                        )
                );


            // ==========================================
            // USUARIOS
            // ==========================================

            var totalUsuarios =
                users.Count;


            var totalAdmins =
                admins.Count;


            var totalUsuariosNormales =
                normalUsers.Count;


            // ==========================================
            // CATEGORÍAS
            // ==========================================

            var categorias =
                products
                    .GroupBy(
                        p =>
                            string.IsNullOrWhiteSpace(
                                p.Category
                            )
                                ? "Sin categoría"
                                : p.Category
                    )
                    .Select(
                        grupo =>
                            new CategorySummary
                            {
                                Name =
                                    grupo.Key,

                                Products =
                                    grupo.Count(),

                                Stock =
                                    grupo.Sum(
                                        p => p.Stock
                                    ),

                                Value =
                                    grupo.Sum(
                                        p =>
                                            p.Price *
                                            p.Stock
                                    )
                            }
                    )
                    .OrderByDescending(
                        c => c.Products
                    )
                    .ToList();


            // ==========================================
            // PRODUCTOS MÁS VENDIDOS
            // ==========================================

            var topProducts =
                orders
                    .SelectMany(
                        o => o.Items
                    )
                    .GroupBy(
                        item =>
                            item.ProductName
                    )
                    .Select(
                        grupo =>
                            new TopProductSummary
                            {
                                Name =
                                    grupo.Key,

                                Quantity =
                                    grupo.Sum(
                                        i => i.Quantity
                                    ),

                                Total =
                                    grupo.Sum(
                                        i => i.Subtotal
                                    )
                            }
                    )
                    .OrderByDescending(
                        p => p.Quantity
                    )
                    .ThenByDescending(
                        p => p.Total
                    )
                    .Take(8)
                    .ToList();


            // ==========================================
            // ÚLTIMOS PEDIDOS
            // ==========================================

            var recentOrders =
                orders
                    .Take(8)
                    .ToList();


            // ==========================================
            // CREAR PDF
            // ==========================================

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
                                                                        .FontSize(26)
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
                                                                            "REPORTE GENERAL"
                                                                        )
                                                                        .FontSize(17)
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
                                            column.Spacing(15);


                                            // ==========================
                                            // TÍTULO
                                            // ==========================

                                            column.Item()
                                                .Text(
                                                    "Resumen ejecutivo de BigGame"
                                                )
                                                .FontSize(18)
                                                .Bold();


                                            column.Item()
                                                .Text(
                                                    "Vista consolidada del catálogo, inventario, ventas, pedidos y usuarios registrados en el sistema."
                                                )
                                                .FontSize(9)
                                                .FontColor(
                                                    Colors.Grey.Darken1
                                                );


                                            // ==========================
                                            // TARJETAS PRINCIPALES
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
                                                                        "Productos",
                                                                        totalProductos.ToString(),
                                                                        "Catálogo"
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(7)
                                                            .Element(
                                                                c =>
                                                                    TarjetaResumen(
                                                                        c,
                                                                        "Stock total",
                                                                        stockTotal.ToString(),
                                                                        "Unidades"
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(7)
                                                            .Element(
                                                                c =>
                                                                    TarjetaResumen(
                                                                        c,
                                                                        "Pedidos",
                                                                        totalPedidos.ToString(),
                                                                        "Registrados"
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(7)
                                                            .Element(
                                                                c =>
                                                                    TarjetaResumen(
                                                                        c,
                                                                        "Usuarios",
                                                                        totalUsuarios.ToString(),
                                                                        "Registrados"
                                                                    )
                                                            );
                                                    }
                                                );


                                            column.Item()
                                                .Row(
                                                    row =>
                                                    {
                                                        row.RelativeItem()
                                                            .Element(
                                                                c =>
                                                                    TarjetaResumen(
                                                                        c,
                                                                        "Ingresos",
                                                                        Moneda(
                                                                            totalVentas
                                                                        ),
                                                                        "Ventas"
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(7)
                                                            .Element(
                                                                c =>
                                                                    TarjetaResumen(
                                                                        c,
                                                                        "Inventario",
                                                                        Moneda(
                                                                            valorInventario
                                                                        ),
                                                                        "Valor actual"
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(7)
                                                            .Element(
                                                                c =>
                                                                    TarjetaResumen(
                                                                        c,
                                                                        "Unidades vendidas",
                                                                        unidadesVendidas.ToString(),
                                                                        "Histórico"
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(7)
                                                            .Element(
                                                                c =>
                                                                    TarjetaResumen(
                                                                        c,
                                                                        "Promedio venta",
                                                                        Moneda(
                                                                            promedioVenta
                                                                        ),
                                                                        "Por pedido"
                                                                    )
                                                            );
                                                    }
                                                );


                                            // ==========================
                                            // ESTADO DEL NEGOCIO
                                            // ==========================

                                            column.Item()
                                                .Text(
                                                    "Estado general"
                                                )
                                                .FontSize(13)
                                                .Bold();


                                            column.Item()
                                                .Table(
                                                    table =>
                                                    {
                                                        table.ColumnsDefinition(
                                                            columns =>
                                                            {
                                                                columns.RelativeColumn();

                                                                columns.RelativeColumn();

                                                                columns.RelativeColumn();
                                                            }
                                                        );


                                                        table.Header(
                                                            header =>
                                                            {
                                                                header.Cell()
                                                                    .Element(CeldaCabecera)
                                                                    .Text(
                                                                        "Área"
                                                                    );

                                                                header.Cell()
                                                                    .Element(CeldaCabecera)
                                                                    .Text(
                                                                        "Indicador"
                                                                    );

                                                                header.Cell()
                                                                    .Element(CeldaCabecera)
                                                                    .Text(
                                                                        "Resultado"
                                                                    );
                                                            }
                                                        );


                                                        FilaEstado(
                                                            table,
                                                            "Inventario",
                                                            "Productos con stock bajo",
                                                            stockBajo.ToString()
                                                        );


                                                        FilaEstado(
                                                            table,
                                                            "Inventario",
                                                            "Productos agotados",
                                                            productosAgotados.ToString()
                                                        );


                                                        FilaEstado(
                                                            table,
                                                            "Pedidos",
                                                            "Pedidos confirmados",
                                                            pedidosConfirmados.ToString()
                                                        );


                                                        FilaEstado(
                                                            table,
                                                            "Usuarios",
                                                            "Administradores",
                                                            totalAdmins.ToString()
                                                        );


                                                        FilaEstado(
                                                            table,
                                                            "Usuarios",
                                                            "Usuarios estándar",
                                                            totalUsuariosNormales.ToString()
                                                        );
                                                    }
                                                );


                                            // ==========================
                                            // CATEGORÍAS
                                            // ==========================

                                            column.Item()
                                                .Text(
                                                    "Resumen por categorías"
                                                )
                                                .FontSize(13)
                                                .Bold();


                                            if (categorias.Count == 0)
                                            {
                                                column.Item()
                                                    .Text(
                                                        "No existen categorías registradas."
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
                                                                    columns.RelativeColumn(
                                                                        2
                                                                    );

                                                                    columns.RelativeColumn();

                                                                    columns.RelativeColumn();

                                                                    columns.RelativeColumn(
                                                                        1.5f
                                                                    );
                                                                }
                                                            );


                                                            table.Header(
                                                                header =>
                                                                {
                                                                    header.Cell()
                                                                        .Element(CeldaCabecera)
                                                                        .Text(
                                                                            "Categoría"
                                                                        );

                                                                    header.Cell()
                                                                        .Element(CeldaCabecera)
                                                                        .AlignCenter()
                                                                        .Text(
                                                                            "Productos"
                                                                        );

                                                                    header.Cell()
                                                                        .Element(CeldaCabecera)
                                                                        .AlignCenter()
                                                                        .Text(
                                                                            "Stock"
                                                                        );

                                                                    header.Cell()
                                                                        .Element(CeldaCabecera)
                                                                        .AlignRight()
                                                                        .Text(
                                                                            "Valor"
                                                                        );
                                                                }
                                                            );


                                                            foreach (
                                                                var categoria
                                                                in categorias
                                                            )
                                                            {
                                                                table.Cell()
                                                                    .Element(CeldaNormal)
                                                                    .Text(
                                                                        categoria.Name
                                                                    );


                                                                table.Cell()
                                                                    .Element(CeldaNormal)
                                                                    .AlignCenter()
                                                                    .Text(
                                                                        categoria.Products.ToString()
                                                                    );


                                                                table.Cell()
                                                                    .Element(CeldaNormal)
                                                                    .AlignCenter()
                                                                    .Text(
                                                                        categoria.Stock.ToString()
                                                                    );


                                                                table.Cell()
                                                                    .Element(CeldaNormal)
                                                                    .AlignRight()
                                                                    .Text(
                                                                        Moneda(
                                                                            categoria.Value
                                                                        )
                                                                    );
                                                            }
                                                        }
                                                    );
                                            }


                                            // ==========================
                                            // PRODUCTOS MÁS VENDIDOS
                                            // ==========================

                                            column.Item()
                                                .Text(
                                                    "Productos más vendidos"
                                                )
                                                .FontSize(13)
                                                .Bold();


                                            if (topProducts.Count == 0)
                                            {
                                                column.Item()
                                                    .Text(
                                                        "Todavía no existen productos vendidos."
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
                                                                    columns.RelativeColumn(
                                                                        3
                                                                    );

                                                                    columns.RelativeColumn();

                                                                    columns.RelativeColumn(
                                                                        1.5f
                                                                    );
                                                                }
                                                            );


                                                            table.Header(
                                                                header =>
                                                                {
                                                                    header.Cell()
                                                                        .Element(CeldaCabecera)
                                                                        .Text(
                                                                            "Producto"
                                                                        );

                                                                    header.Cell()
                                                                        .Element(CeldaCabecera)
                                                                        .AlignCenter()
                                                                        .Text(
                                                                            "Unidades"
                                                                        );

                                                                    header.Cell()
                                                                        .Element(CeldaCabecera)
                                                                        .AlignRight()
                                                                        .Text(
                                                                            "Ingresos"
                                                                        );
                                                                }
                                                            );


                                                            foreach (
                                                                var product
                                                                in topProducts
                                                            )
                                                            {
                                                                table.Cell()
                                                                    .Element(CeldaNormal)
                                                                    .Text(
                                                                        product.Name
                                                                    );


                                                                table.Cell()
                                                                    .Element(CeldaNormal)
                                                                    .AlignCenter()
                                                                    .Text(
                                                                        product.Quantity.ToString()
                                                                    );


                                                                table.Cell()
                                                                    .Element(CeldaNormal)
                                                                    .AlignRight()
                                                                    .Text(
                                                                        Moneda(
                                                                            product.Total
                                                                        )
                                                                    );
                                                            }
                                                        }
                                                    );
                                            }


                                            // ==========================
                                            // ÚLTIMOS PEDIDOS
                                            // ==========================

                                            column.Item()
                                                .Text(
                                                    "Últimos pedidos registrados"
                                                )
                                                .FontSize(13)
                                                .Bold();


                                            if (recentOrders.Count == 0)
                                            {
                                                column.Item()
                                                    .Text(
                                                        "No existen pedidos registrados."
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
                                                                        95
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        2
                                                                    );

                                                                    columns.RelativeColumn();

                                                                    columns.RelativeColumn(
                                                                        1.2f
                                                                    );
                                                                }
                                                            );


                                                            table.Header(
                                                                header =>
                                                                {
                                                                    header.Cell()
                                                                        .Element(CeldaCabecera)
                                                                        .Text(
                                                                            "Pedido"
                                                                        );

                                                                    header.Cell()
                                                                        .Element(CeldaCabecera)
                                                                        .Text(
                                                                            "Fecha"
                                                                        );

                                                                    header.Cell()
                                                                        .Element(CeldaCabecera)
                                                                        .Text(
                                                                            "Cliente"
                                                                        );

                                                                    header.Cell()
                                                                        .Element(CeldaCabecera)
                                                                        .Text(
                                                                            "Estado"
                                                                        );

                                                                    header.Cell()
                                                                        .Element(CeldaCabecera)
                                                                        .AlignRight()
                                                                        .Text(
                                                                            "Total"
                                                                        );
                                                                }
                                                            );


                                                            foreach (
                                                                var order
                                                                in recentOrders
                                                            )
                                                            {
                                                                var fechaPedido =
                                                                    order.CreatedAt
                                                                        .AddHours(-4);


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
                                                                        order.Status
                                                                    );


                                                                table.Cell()
                                                                    .Element(CeldaNormal)
                                                                    .AlignRight()
                                                                    .Text(
                                                                        Moneda(
                                                                            order.TotalAmount
                                                                        )
                                                                    );
                                                            }
                                                        }
                                                    );
                                            }


                                            // ==========================
                                            // CIERRE
                                            // ==========================

                                            column.Item()
                                                .PaddingTop(5)
                                                .Border(1)
                                                .BorderColor(
                                                    Colors.Blue.Lighten3
                                                )
                                                .Background(
                                                    Colors.Blue.Lighten5
                                                )
                                                .Padding(12)
                                                .Column(
                                                    box =>
                                                    {
                                                        box.Item()
                                                            .Text(
                                                                "Resumen administrativo"
                                                            )
                                                            .FontSize(11)
                                                            .Bold()
                                                            .FontColor(
                                                                Colors.Blue.Darken2
                                                            );


                                                        box.Item()
                                                            .PaddingTop(5)
                                                            .Text(
                                                                $"BigGame registra {totalProductos} productos, {stockTotal} unidades en inventario, {totalPedidos} pedidos, {unidadesVendidas} unidades vendidas y {totalUsuarios} usuarios."
                                                            )
                                                            .FontSize(9);


                                                        box.Item()
                                                            .PaddingTop(3)
                                                            .Text(
                                                                $"Ingresos acumulados: {Moneda(totalVentas)} | Valor actual del inventario: {Moneda(valorInventario)}."
                                                            )
                                                            .FontSize(9)
                                                            .Bold();
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
                                    .PaddingTop(8)
                                    .Row(
                                        row =>
                                        {
                                            row.RelativeItem()
                                                .Text(
                                                    "BigGame • Reporte general"
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
                $"BigGame_Reporte_General_{fecha:yyyyMMdd_HHmm}.pdf";


            return File(
                pdf,
                "application/pdf",
                fileName
            );
        }


        // ==========================================
        // FILA DE ESTADO
        // ==========================================

        private static void FilaEstado(
            TableDescriptor table,
            string area,
            string indicador,
            string resultado)
        {
            table.Cell()
                .Element(CeldaNormal)
                .Text(area);


            table.Cell()
                .Element(CeldaNormal)
                .Text(indicador);


            table.Cell()
                .Element(CeldaNormal)
                .Text(resultado);
        }


        // ==========================================
        // TARJETA DE RESUMEN
        // ==========================================

        private static void TarjetaResumen(
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
                            .Text(titulo)
                            .FontSize(7)
                            .FontColor(
                                Colors.Grey.Darken1
                            );


                        column.Item()
                            .PaddingTop(2)
                            .Text(valor)
                            .FontSize(12)
                            .Bold()
                            .FontColor(
                                Colors.Blue.Darken2
                            );


                        column.Item()
                            .PaddingTop(2)
                            .Text(detalle)
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
        // HORA DE BOLIVIA
        // ==========================================

        private static DateTime FechaBolivia()
        {
            return DateTime.UtcNow
                .AddHours(-4);
        }


        // ==========================================
        // MODELOS INTERNOS
        // ==========================================

        private class CategorySummary
        {
            public string Name { get; set; } =
                string.Empty;

            public int Products { get; set; }

            public int Stock { get; set; }

            public decimal Value { get; set; }
        }


        private class TopProductSummary
        {
            public string Name { get; set; } =
                string.Empty;

            public int Quantity { get; set; }

            public decimal Total { get; set; }
        }
    }
}