using EcommerceApp.Data;
using EcommerceApp.Reports.Queries;
using EcommerceApp.Reports.Renderers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VentasPeriodoReportsController : Controller
    {
        private readonly VentasPeriodoQuery _query;
        private readonly VentasPeriodoPdfRenderer _pdfRenderer;


        public VentasPeriodoReportsController(
            ApplicationDbContext context)
        {
            _query =
                new VentasPeriodoQuery(
                    context
                );

            _pdfRenderer =
                new VentasPeriodoPdfRenderer();
        }


        // ==========================================
        // PANTALLA DEL REPORTE
        // ==========================================

        [HttpGet]
        public IActionResult Index(
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null)
        {
            var hoy =
                DateTime.UtcNow
                    .AddHours(-4)
                    .Date;


            var inicio =
                fechaInicio
                ?? hoy.AddDays(-30);


            var fin =
                fechaFin
                ?? hoy;


            ViewBag.FechaInicio =
                inicio.ToString(
                    "yyyy-MM-dd"
                );


            ViewBag.FechaFin =
                fin.ToString(
                    "yyyy-MM-dd"
                );


            return View();
        }


        // ==========================================
        // DESCARGAR PDF
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> DescargarPdf(
            DateTime? fechaInicio,
            DateTime? fechaFin)
        {
            // ======================================
            // VALIDAR FECHAS
            // ======================================

            if (
                fechaInicio == null ||
                fechaFin == null
            )
            {
                TempData["ReportError"] =
                    "Debes seleccionar una fecha inicial y una fecha final.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            if (
                fechaInicio.Value.Date >
                fechaFin.Value.Date
            )
            {
                TempData["ReportError"] =
                    "La fecha inicial no puede ser posterior a la fecha final.";

                return RedirectToAction(
                    nameof(Index),
                    new
                    {
                        fechaInicio =
                            fechaInicio.Value
                                .ToString(
                                    "yyyy-MM-dd"
                                ),

                        fechaFin =
                            fechaFin.Value
                                .ToString(
                                    "yyyy-MM-dd"
                                )
                    }
                );
            }


            // ======================================
            // CONSULTA
            // ======================================

            var reportModel =
                await _query.ExecuteAsync(
                    fechaInicio.Value,
                    fechaFin.Value
                );


            // ======================================
            // RENDERER
            // ======================================

            var pdf =
                _pdfRenderer.Render(
                    reportModel
                );


            // ======================================
            // ENTREGA
            // ======================================

            var nombreArchivo =
                $"BigGame_Ventas_" +
                $"{fechaInicio.Value:yyyyMMdd}_" +
                $"{fechaFin.Value:yyyyMMdd}_" +
                $"{reportModel.FechaGeneracion:HHmm}.pdf";


            return File(
                pdf,
                "application/pdf",
                nombreArchivo
            );
        }
    }
}