using EcommerceApp.Data;
using EcommerceApp.Reports.Queries;
using EcommerceApp.Reports.Renderers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InventarioStockBajoReportsController : Controller
    {
        private readonly InventarioStockBajoQuery _query;
        private readonly InventarioStockBajoPdfRenderer _renderer;


        public InventarioStockBajoReportsController(
            ApplicationDbContext context)
        {
            _query =
                new InventarioStockBajoQuery(
                    context
                );

            _renderer =
                new InventarioStockBajoPdfRenderer();
        }


        // ==========================================
        // GENERAR Y DESCARGAR PDF
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> DescargarPdf(
            int limite = 10)
        {
            // ======================================
            // VALIDAR LÍMITE
            // ======================================

            if (limite < 0)
            {
                limite = 10;
            }


            if (limite > 1000)
            {
                limite = 1000;
            }


            // ======================================
            // CONSULTA
            // ======================================

            var reportModel =
                await _query.ExecuteAsync(
                    limite
                );


            // ======================================
            // RENDERER
            // ======================================

            var pdf =
                _renderer.Render(
                    reportModel
                );


            // ======================================
            // ENTREGA
            // ======================================

            var nombreArchivo =
                $"BigGame_Inventario_Stock_Bajo_" +
                $"{reportModel.FechaGeneracion:yyyyMMdd_HHmm}.pdf";


            return File(
                pdf,
                "application/pdf",
                nombreArchivo
            );
        }
    }
}