using EcommerceApp.Data;
using EcommerceApp.Models;
using EcommerceApp.Reports.Queries;
using EcommerceApp.Reports.Renderers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Controllers
{
    [Authorize]
    public class ComprobanteCompraReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ComprobanteCompraQuery _query;

        private readonly ComprobanteCompraPdfRenderer _renderer;


        public ComprobanteCompraReportsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context =
                context;

            _userManager =
                userManager;

            _query =
                new ComprobanteCompraQuery(
                    context
                );

            _renderer =
                new ComprobanteCompraPdfRenderer();
        }


        // ==========================================
        // GENERAR Y DESCARGAR COMPROBANTE
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> DescargarPdf(
            int id)
        {
            // ======================================
            // VALIDAR PEDIDO
            // ======================================

            if (id <= 0)
            {
                return BadRequest(
                    "El número de pedido no es válido."
                );
            }


            // ======================================
            // USUARIO ACTUAL
            // ======================================

            var usuarioActual =
                await _userManager
                    .GetUserAsync(User);


            if (usuarioActual == null)
            {
                return Challenge();
            }


            // ======================================
            // VERIFICAR SI ES ADMIN
            // ======================================

            var esAdministrador =
                await _userManager
                    .IsInRoleAsync(
                        usuarioActual,
                        "Admin"
                    );


            // ======================================
            // USUARIO NORMAL:
            // SOLO SUS PROPIOS PEDIDOS
            // ======================================

            if (!esAdministrador)
            {
                var pedidoPerteneceAlUsuario =
                    await _context.Orders
                        .AsNoTracking()
                        .AnyAsync(
                            pedido =>
                                pedido.Id == id
                                &&
                                pedido.UserId
                                    == usuarioActual.Id
                        );


                if (!pedidoPerteneceAlUsuario)
                {
                    return Forbid();
                }
            }


            // ======================================
            // CONSULTA
            // ======================================

            var reportModel =
                await _query.ExecuteAsync(
                    id
                );


            if (reportModel == null)
            {
                return NotFound(
                    "No se encontró el pedido solicitado."
                );
            }


            // ======================================
            // RENDERER PDF
            // ======================================

            var pdf =
                _renderer.Render(
                    reportModel
                );


            // ======================================
            // NOMBRE DEL ARCHIVO
            // ======================================

            var nombreArchivo =
                $"BigGame_Comprobante_Pedido_" +
                $"{reportModel.PedidoId}_" +
                $"{reportModel.FechaGeneracion:yyyyMMdd_HHmm}.pdf";


            // ======================================
            // ENTREGA
            // ======================================

            return File(
                pdf,
                "application/pdf",
                nombreArchivo
            );
        }
    }
}