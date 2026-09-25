using ClosedXML.Excel;
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
        // VISUALIZAR PDF DENTRO DE BIGGAME
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> VerPdf(
            DateTime? fechaInicio,
            DateTime? fechaFin)
        {
            var validacion =
                ValidarFechas(
                    fechaInicio,
                    fechaFin
                );


            if (!validacion.EsValido)
            {
                return Content(
                    validacion.Mensaje,
                    "text/plain"
                );
            }


            var reportModel =
                await _query.ExecuteAsync(
                    fechaInicio!.Value,
                    fechaFin!.Value
                );


            var pdf =
                _pdfRenderer.Render(
                    reportModel
                );


            // Al no enviar nombre de archivo,
            // el navegador intenta mostrarlo en línea.
            return File(
                pdf,
                "application/pdf"
            );
        }


        // ==========================================
        // DESCARGAR PDF
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> DescargarPdf(
            DateTime? fechaInicio,
            DateTime? fechaFin)
        {
            var validacion =
                ValidarFechas(
                    fechaInicio,
                    fechaFin
                );


            if (!validacion.EsValido)
            {
                TempData["ReportError"] =
                    validacion.Mensaje;


                return RedirectToAction(
                    nameof(Index),
                    new
                    {
                        fechaInicio =
                            fechaInicio?.ToString(
                                "yyyy-MM-dd"
                            ),

                        fechaFin =
                            fechaFin?.ToString(
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
                    fechaInicio!.Value,
                    fechaFin!.Value
                );


            // ======================================
            // RENDERER
            // ======================================

            var pdf =
                _pdfRenderer.Render(
                    reportModel
                );


            // ======================================
            // NOMBRE DEL ARCHIVO
            // ======================================

            var nombreArchivo =
                $"BigGame_Ventas_" +
                $"{fechaInicio.Value:yyyyMMdd}_" +
                $"{fechaFin.Value:yyyyMMdd}_" +
                $"{reportModel.FechaGeneracion:HHmm}.pdf";


            // ======================================
            // ENTREGA
            // ======================================

            return File(
                pdf,
                "application/pdf",
                nombreArchivo
            );
        }


        // ==========================================
        // DESCARGAR EXCEL
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> DescargarExcel(
            DateTime? fechaInicio,
            DateTime? fechaFin)
        {
            var validacion =
                ValidarFechas(
                    fechaInicio,
                    fechaFin
                );


            if (!validacion.EsValido)
            {
                TempData["ReportError"] =
                    validacion.Mensaje;


                return RedirectToAction(
                    nameof(Index),
                    new
                    {
                        fechaInicio =
                            fechaInicio?.ToString(
                                "yyyy-MM-dd"
                            ),

                        fechaFin =
                            fechaFin?.ToString(
                                "yyyy-MM-dd"
                            )
                    }
                );
            }


            // ======================================
            // CONSULTAR DATOS
            // ======================================

            var reportModel =
                await _query.ExecuteAsync(
                    fechaInicio!.Value,
                    fechaFin!.Value
                );


            // ======================================
            // CREAR LIBRO EXCEL
            // ======================================

            using var workbook =
                new XLWorkbook();


            var worksheet =
                workbook.Worksheets.Add(
                    "Ventas por período"
                );


            // ======================================
            // TÍTULO
            // ======================================

            worksheet.Range(
                    "A1:G1"
                )
                .Merge();


            worksheet.Cell(
                    "A1"
                )
                .Value =
                    "BIGGAME - REPORTE DE VENTAS POR PERÍODO";


            worksheet.Cell(
                    "A1"
                )
                .Style.Font.Bold =
                    true;


            worksheet.Cell(
                    "A1"
                )
                .Style.Font.FontSize =
                    16;


            worksheet.Cell(
                    "A1"
                )
                .Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;


            worksheet.Cell(
                    "A1"
                )
                .Style.Fill.BackgroundColor =
                    XLColor.FromHtml(
                        "#0F172A"
                    );


            worksheet.Cell(
                    "A1"
                )
                .Style.Font.FontColor =
                    XLColor.White;


            worksheet.Row(1)
                .Height =
                    28;


            // ======================================
            // INFORMACIÓN DEL REPORTE
            // ======================================

            worksheet.Cell(
                    "A3"
                )
                .Value =
                    "Fecha inicial";


            worksheet.Cell(
                    "B3"
                )
                .Value =
                    reportModel.FechaInicio;


            worksheet.Cell(
                    "B3"
                )
                .Style.DateFormat.Format =
                    "dd/MM/yyyy";


            worksheet.Cell(
                    "D3"
                )
                .Value =
                    "Fecha final";


            worksheet.Cell(
                    "E3"
                )
                .Value =
                    reportModel.FechaFin;


            worksheet.Cell(
                    "E3"
                )
                .Style.DateFormat.Format =
                    "dd/MM/yyyy";


            worksheet.Cell(
                    "A4"
                )
                .Value =
                    "Generado";


            worksheet.Cell(
                    "B4"
                )
                .Value =
                    reportModel.FechaGeneracion;


            worksheet.Cell(
                    "B4"
                )
                .Style.DateFormat.Format =
                    "dd/MM/yyyy HH:mm";


            // ======================================
            // RESUMEN
            // ======================================

            worksheet.Cell(
                    "A6"
                )
                .Value =
                    "RESUMEN";


            worksheet.Cell(
                    "A6"
                )
                .Style.Font.Bold =
                    true;


            worksheet.Cell(
                    "A7"
                )
                .Value =
                    "Total pedidos";


            worksheet.Cell(
                    "B7"
                )
                .Value =
                    reportModel.TotalPedidos;


            worksheet.Cell(
                    "D7"
                )
                .Value =
                    "Productos vendidos";


            worksheet.Cell(
                    "E7"
                )
                .Value =
                    reportModel.TotalProductosVendidos;


            worksheet.Cell(
                    "A8"
                )
                .Value =
                    "Total ventas";


            worksheet.Cell(
                    "B8"
                )
                .Value =
                    reportModel.TotalVentas;


            worksheet.Cell(
                    "B8"
                )
                .Style.NumberFormat.Format =
                    "\"Bs\" #,##0.00";


            worksheet.Cell(
                    "D8"
                )
                .Value =
                    "Promedio por venta";


            worksheet.Cell(
                    "E8"
                )
                .Value =
                    reportModel.PromedioVenta;


            worksheet.Cell(
                    "E8"
                )
                .Style.NumberFormat.Format =
                    "\"Bs\" #,##0.00";


            // ======================================
            // CABECERA DE TABLA
            // ======================================

            const int headerRow =
                10;


            worksheet.Cell(
                    headerRow,
                    1
                )
                .Value =
                    "Pedido";


            worksheet.Cell(
                    headerRow,
                    2
                )
                .Value =
                    "Fecha";


            worksheet.Cell(
                    headerRow,
                    3
                )
                .Value =
                    "Cliente";


            worksheet.Cell(
                    headerRow,
                    4
                )
                .Value =
                    "Correo";


            worksheet.Cell(
                    headerRow,
                    5
                )
                .Value =
                    "Estado";


            worksheet.Cell(
                    headerRow,
                    6
                )
                .Value =
                    "Productos";


            worksheet.Cell(
                    headerRow,
                    7
                )
                .Value =
                    "Total";


            var headerRange =
                worksheet.Range(
                    headerRow,
                    1,
                    headerRow,
                    7
                );


            headerRange.Style
                .Font.Bold =
                    true;


            headerRange.Style
                .Font.FontColor =
                    XLColor.White;


            headerRange.Style
                .Fill.BackgroundColor =
                    XLColor.FromHtml(
                        "#2563EB"
                    );


            headerRange.Style
                .Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;


            // ======================================
            // DATOS
            // ======================================

            var row =
                headerRow + 1;


            foreach (
                var venta
                in reportModel.Ventas
            )
            {
                worksheet.Cell(
                        row,
                        1
                    )
                    .Value =
                        venta.PedidoId;


                worksheet.Cell(
                        row,
                        2
                    )
                    .Value =
                        venta.Fecha;


                worksheet.Cell(
                        row,
                        2
                    )
                    .Style.DateFormat.Format =
                        "dd/MM/yyyy HH:mm";


                worksheet.Cell(
                        row,
                        3
                    )
                    .Value =
                        venta.Cliente;


                worksheet.Cell(
                        row,
                        4
                    )
                    .Value =
                        venta.Correo;


                worksheet.Cell(
                        row,
                        5
                    )
                    .Value =
                        venta.Estado;


                worksheet.Cell(
                        row,
                        6
                    )
                    .Value =
                        venta.CantidadProductos;


                worksheet.Cell(
                        row,
                        7
                    )
                    .Value =
                        venta.Total;


                worksheet.Cell(
                        row,
                        7
                    )
                    .Style.NumberFormat.Format =
                        "\"Bs\" #,##0.00";


                row++;
            }


            // ======================================
            // TOTAL FINAL
            // ======================================

            var totalRow =
                row + 1;


            worksheet.Cell(
                    totalRow,
                    6
                )
                .Value =
                    "TOTAL";


            worksheet.Cell(
                    totalRow,
                    6
                )
                .Style.Font.Bold =
                    true;


            worksheet.Cell(
                    totalRow,
                    7
                )
                .Value =
                    reportModel.TotalVentas;


            worksheet.Cell(
                    totalRow,
                    7
                )
                .Style.Font.Bold =
                    true;


            worksheet.Cell(
                    totalRow,
                    7
                )
                .Style.NumberFormat.Format =
                    "\"Bs\" #,##0.00";


            // ======================================
            // BORDES
            // ======================================

            if (
                reportModel.Ventas.Count > 0
            )
            {
                var datosRange =
                    worksheet.Range(
                        headerRow,
                        1,
                        row - 1,
                        7
                    );


                datosRange.Style
                    .Border.OutsideBorder =
                        XLBorderStyleValues.Thin;


                datosRange.Style
                    .Border.InsideBorder =
                        XLBorderStyleValues.Thin;
            }
            else
            {
                headerRange.Style
                    .Border.OutsideBorder =
                        XLBorderStyleValues.Thin;
            }


            // ======================================
            // AJUSTAR COLUMNAS
            // ======================================

            worksheet.Column(1)
                .Width =
                    12;


            worksheet.Column(2)
                .Width =
                    20;


            worksheet.Column(3)
                .Width =
                    25;


            worksheet.Column(4)
                .Width =
                    32;


            worksheet.Column(5)
                .Width =
                    18;


            worksheet.Column(6)
                .Width =
                    14;


            worksheet.Column(7)
                .Width =
                    18;


            worksheet.SheetView
                .FreezeRows(
                    headerRow
                );


            // ======================================
            // GUARDAR EN MEMORIA
            // ======================================

            using var stream =
                new MemoryStream();


            workbook.SaveAs(
                stream
            );


            var contenido =
                stream.ToArray();


            // ======================================
            // NOMBRE
            // ======================================

            var nombreArchivo =
                $"BigGame_Ventas_" +
                $"{fechaInicio.Value:yyyyMMdd}_" +
                $"{fechaFin.Value:yyyyMMdd}_" +
                $"{reportModel.FechaGeneracion:HHmm}.xlsx";


            // ======================================
            // ENTREGA
            // ======================================

            return File(
                contenido,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                nombreArchivo
            );
        }


        // ==========================================
        // VALIDACIÓN CENTRAL DE FECHAS
        // ==========================================

        private static (
            bool EsValido,
            string Mensaje
        ) ValidarFechas(
            DateTime? fechaInicio,
            DateTime? fechaFin)
        {
            if (
                fechaInicio == null
                ||
                fechaFin == null
            )
            {
                return (
                    false,
                    "Debes seleccionar una fecha inicial y una fecha final."
                );
            }


            if (
                fechaInicio.Value.Date
                >
                fechaFin.Value.Date
            )
            {
                return (
                    false,
                    "La fecha inicial no puede ser posterior a la fecha final."
                );
            }


            return (
                true,
                string.Empty
            );
        }
    }
}