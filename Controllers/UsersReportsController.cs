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
    public class UsersReportsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersReportsController(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }


        // ==========================================
        // REPORTE DE USUARIOS
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> UsuariosPdf()
        {
            var users =
                await _userManager.Users
                    .OrderByDescending(
                        u => u.CreatedAt
                    )
                    .ToListAsync();


            var rows =
                new List<UserReportRow>();


            foreach (var user in users)
            {
                var roles =
                    await _userManager
                        .GetRolesAsync(user);


                rows.Add(
                    new UserReportRow
                    {
                        Id =
                            user.Id,

                        FullName =
                            string.IsNullOrWhiteSpace(
                                user.FullName
                            )
                                ? "Sin nombre"
                                : user.FullName,

                        Email =
                            string.IsNullOrWhiteSpace(
                                user.Email
                            )
                                ? "Sin correo"
                                : user.Email,

                        Address =
                            string.IsNullOrWhiteSpace(
                                user.Address
                            )
                                ? "Sin dirección"
                                : user.Address,

                        PhoneNumber =
                            string.IsNullOrWhiteSpace(
                                user.PhoneNumber
                            )
                                ? "No registrado"
                                : user.PhoneNumber,

                        Roles =
                            roles.Count > 0
                                ? string.Join(
                                    ", ",
                                    roles
                                )
                                : "Sin rol",

                        CreatedAt =
                            user.CreatedAt
                    }
                );
            }


            var fecha =
                FechaBolivia();


            var totalUsuarios =
                rows.Count;


            var administradores =
                rows.Count(
                    u =>
                        u.Roles.Contains(
                            "Admin",
                            StringComparison.OrdinalIgnoreCase
                        )
                );


            var usuarios =
                rows.Count(
                    u =>
                        u.Roles.Contains(
                            "User",
                            StringComparison.OrdinalIgnoreCase
                        )
                );


            var sinRol =
                rows.Count(
                    u =>
                        u.Roles.Equals(
                            "Sin rol",
                            StringComparison.OrdinalIgnoreCase
                        )
                );


            var conDireccion =
                rows.Count(
                    u =>
                        !u.Address.Equals(
                            "Sin dirección",
                            StringComparison.OrdinalIgnoreCase
                        )
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
                                                                            "REPORTE DE USUARIOS"
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
                                                                        "Usuarios registrados",
                                                                        totalUsuarios.ToString()
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(8)
                                                            .Element(
                                                                c =>
                                                                    TarjetaResumen(
                                                                        c,
                                                                        "Administradores",
                                                                        administradores.ToString()
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(8)
                                                            .Element(
                                                                c =>
                                                                    TarjetaResumen(
                                                                        c,
                                                                        "Usuarios",
                                                                        usuarios.ToString()
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(8)
                                                            .Element(
                                                                c =>
                                                                    TarjetaResumen(
                                                                        c,
                                                                        "Sin rol",
                                                                        sinRol.ToString()
                                                                    )
                                                            );


                                                        row.RelativeItem()
                                                            .PaddingLeft(8)
                                                            .Element(
                                                                c =>
                                                                    TarjetaResumen(
                                                                        c,
                                                                        "Con dirección",
                                                                        conDireccion.ToString()
                                                                    )
                                                            );
                                                    }
                                                );


                                            // ==========================
                                            // TÍTULO
                                            // ==========================

                                            column.Item()
                                                .Text(
                                                    "Listado de usuarios"
                                                )
                                                .FontSize(13)
                                                .Bold();


                                            // ==========================
                                            // SIN REGISTROS
                                            // ==========================

                                            if (rows.Count == 0)
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
                                                        "No existen usuarios registrados."
                                                    )
                                                    .FontSize(11);
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
                                                                        42
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        1.5f
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        1.8f
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        1.4f
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        1.2f
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        1
                                                                    );

                                                                    columns.ConstantColumn(
                                                                        95
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
                                                                            "N.º"
                                                                        );


                                                                    header.Cell()
                                                                        .Element(
                                                                            CeldaCabecera
                                                                        )
                                                                        .Text(
                                                                            "Nombre"
                                                                        );


                                                                    header.Cell()
                                                                        .Element(
                                                                            CeldaCabecera
                                                                        )
                                                                        .Text(
                                                                            "Correo"
                                                                        );


                                                                    header.Cell()
                                                                        .Element(
                                                                            CeldaCabecera
                                                                        )
                                                                        .Text(
                                                                            "Dirección"
                                                                        );


                                                                    header.Cell()
                                                                        .Element(
                                                                            CeldaCabecera
                                                                        )
                                                                        .Text(
                                                                            "Teléfono"
                                                                        );


                                                                    header.Cell()
                                                                        .Element(
                                                                            CeldaCabecera
                                                                        )
                                                                        .Text(
                                                                            "Rol"
                                                                        );


                                                                    header.Cell()
                                                                        .Element(
                                                                            CeldaCabecera
                                                                        )
                                                                        .Text(
                                                                            "Registro"
                                                                        );
                                                                }
                                                            );


                                                            // ==================
                                                            // FILAS
                                                            // ==================

                                                            var numero =
                                                                1;


                                                            foreach (
                                                                var user
                                                                in rows
                                                            )
                                                            {
                                                                var fechaRegistro =
                                                                    user.CreatedAt
                                                                        .AddHours(-4);


                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .Text(
                                                                        numero.ToString()
                                                                    );


                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .Text(
                                                                        user.FullName
                                                                    );


                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .Text(
                                                                        user.Email
                                                                    );


                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .Text(
                                                                        user.Address
                                                                    );


                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .Text(
                                                                        user.PhoneNumber
                                                                    );


                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .Text(
                                                                        user.Roles
                                                                    );


                                                                table.Cell()
                                                                    .Element(
                                                                        CeldaNormal
                                                                    )
                                                                    .Text(
                                                                        fechaRegistro.ToString(
                                                                            "dd/MM/yyyy HH:mm"
                                                                        )
                                                                    );


                                                                numero++;
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
                                                                    $"TOTAL DE USUARIOS: {totalUsuarios}"
                                                                )
                                                                .FontSize(10)
                                                                .Bold();


                                                            resumen.Item()
                                                                .PaddingTop(3)
                                                                .Text(
                                                                    $"ADMINISTRADORES: {administradores}   |   USUARIOS: {usuarios}"
                                                                )
                                                                .FontSize(9)
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
                                // PIE
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
                                                    "BigGame • Reporte de usuarios"
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
                $"BigGame_Usuarios_{fecha:yyyyMMdd_HHmm}.pdf";


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
                .PaddingVertical(6)
                .PaddingHorizontal(5);
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
        // FILA DEL REPORTE
        // ==========================================

        private class UserReportRow
        {
            public string Id { get; set; } =
                string.Empty;

            public string FullName { get; set; } =
                string.Empty;

            public string Email { get; set; } =
                string.Empty;

            public string Address { get; set; } =
                string.Empty;

            public string PhoneNumber { get; set; } =
                string.Empty;

            public string Roles { get; set; } =
                string.Empty;

            public DateTime CreatedAt { get; set; }
        }
    }
}