using EcommerceApp.Data;
using EcommerceApp.Reports.Queries;
using EcommerceApp.Reports.Renderers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductosMasVendidosReportsController : Controller
    {
        private readonly ProductosMasVendidosQuery _query;
        private readonly ProductosMasVendidosPdfRenderer _renderer;


        public ProductosMasVendidosReportsController(
            ApplicationDbContext context)
        {
            _query =
                new ProductosMasVendidosQuery(
                    context
                );

            _renderer =
                new ProductosMasVendidosPdfRenderer();
        }


        // ==========================================
        // GENERAR Y DESCARGAR PDF
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> DescargarPdf()
        {
            // ======================================
            // CONSULTA
            // ======================================

            var reportModel =
                await _query.ExecuteAsync();


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
                $"BigGame_Productos_Mas_Vendidos_" +
                $"{reportModel.FechaGeneracion:yyyyMMdd_HHmm}.pdf";


            return File(
                pdf,
                "application/pdf",
                nombreArchivo
            );
        }
    }
}