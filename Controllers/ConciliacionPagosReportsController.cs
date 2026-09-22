using EcommerceApp.Data;
using EcommerceApp.Reports.Queries;
using EcommerceApp.Reports.Renderers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ConciliacionPagosReportsController : Controller
    {
        private readonly ConciliacionPagosQuery _query;

        private readonly ConciliacionPagosPdfRenderer _renderer;


        public ConciliacionPagosReportsController(
            ApplicationDbContext context)
        {
            _query =
                new ConciliacionPagosQuery(
                    context
                );


            _renderer =
                new ConciliacionPagosPdfRenderer();
        }


        [HttpGet]
        public async Task<IActionResult> DescargarPdf(
            decimal comision = 3m)
        {
            // ==========================================
            // CONSULTA
            // ==========================================

            var reportModel =
                await _query.ExecuteAsync(
                    comision
                );


            // ==========================================
            // RENDERER
            // ==========================================

            var pdf =
                _renderer.Render(
                    reportModel
                );


            // ==========================================
            // NOMBRE DEL ARCHIVO
            // ==========================================

            var nombreArchivo =
                $"BigGame_Conciliacion_Pagos_" +
                $"{reportModel.FechaGeneracion:yyyyMMdd_HHmm}.pdf";


            // ==========================================
            // ENTREGA
            // ==========================================

            return File(
                pdf,
                "application/pdf",
                nombreArchivo
            );
        }
    }
}