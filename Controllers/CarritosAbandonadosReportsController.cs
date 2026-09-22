using EcommerceApp.Data;
using EcommerceApp.Reports.Queries;
using EcommerceApp.Reports.Renderers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CarritosAbandonadosReportsController : Controller
    {
        private readonly CarritosAbandonadosQuery _query;

        private readonly CarritosAbandonadosPdfRenderer _renderer;


        public CarritosAbandonadosReportsController(
            ApplicationDbContext context)
        {
            _query =
                new CarritosAbandonadosQuery(
                    context
                );


            _renderer =
                new CarritosAbandonadosPdfRenderer();
        }


        // ==========================================
        // GENERAR Y DESCARGAR PDF
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> DescargarPdf(
            int minutos = 30)
        {
            // ======================================
            // VALIDAR MINUTOS
            // ======================================

            if (minutos < 1)
            {
                minutos =
                    30;
            }


            if (minutos > 10080)
            {
                minutos =
                    10080;
            }


            // ======================================
            // CONSULTA
            // ======================================

            var reportModel =
                await _query.ExecuteAsync(
                    minutos
                );


            // ======================================
            // RENDERER
            // ======================================

            var pdf =
                _renderer.Render(
                    reportModel
                );


            // ======================================
            // NOMBRE DEL ARCHIVO
            // ======================================

            var nombreArchivo =
                $"BigGame_Carritos_Abandonados_" +
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