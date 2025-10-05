using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Inmobilaria_lab2_TPI_MGS.Models.ViewModels;
using System.Security.Claims;

namespace Inmobilaria_lab2_TPI_MGS.Controllers
{
    [Authorize(Policy = "UserOrAdmin")]
    public class ContratosController : Controller
    {
        private readonly InquilinoService inquilinoService;
        private readonly ContratoService contratoService;
        private readonly InmuebleService inmuebleService;
        private readonly PagoService pagoService;

        public ContratosController(InquilinoService inquilinoService, ContratoService contratoService, InmuebleService inmuebleService, PagoService pagoService)
        {
            this.inquilinoService = inquilinoService;
            this.contratoService = contratoService;
            this.inmuebleService = inmuebleService;
            this.pagoService = pagoService;
        }

        // GET: ContratosController
        [HttpGet]
        public ActionResult Index(int pagina = 1)
        {
            try
            {
                int tamPagina = 10;
                var totalRegistros = inquilinoService.ContarInquilinosActivosSinContrato();
                var totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamPagina);

                var listaInquilinos = inquilinoService.ListarInquilinosSinContrato(pagina, tamPagina);


                ViewBag.ListaInquilinos = listaInquilinos;
                ViewBag.Pagina = pagina;
                ViewBag.TotalPaginas = totalPaginas;
                ViewBag.TotalRegistros = totalRegistros;

                return View();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [Route("[controller]/Lista")]
        public ActionResult Lista(int pagina = 1)
        {
            int tamPagina = 10;
            var totalRegistros = contratoService.ListaContratosVigentes().Count;
            var totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamPagina);
            var listaContratos = contratoService.ListaContratosVigentes(pagina, tamPagina);

            ViewBag.Pagina = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.TotalRegistros = totalRegistros;
            return View(listaContratos);
        }


        [HttpPost]
        public ActionResult Crear(Contrato contrato, int FechaVencimiento)
        {
            try
            {

                if (contrato.InmuebleId <= 0)
                {
                    ModelState.AddModelError("", "El Inmueble es obligatorio.");
                }

                if (contrato.InquilinoId <= 0)
                {
                    ModelState.AddModelError("", "El Inquilino es obligatorio.");
                }

                if (contrato.FechaInicio == default)
                {
                    ModelState.AddModelError("", "La Fecha de Inicio es obligatoria.");
                }

                if (contrato.FechaFin == default)
                {
                    ModelState.AddModelError("", "La Fecha de Fin es obligatoria.");
                }

                if (contrato.FechaFin < contrato.FechaInicio.AddMonths(6))
                {
                    ModelState.AddModelError("", "La Fecha Fin debe ser al menos 6 meses después de la Fecha Inicio.");
                }

                if (contrato.MontoMensual <= 0)
                {
                    ModelState.AddModelError("", "El Monto Mensual debe ser mayor que cero.");
                }

                if (string.IsNullOrWhiteSpace(contrato.Moneda))
                {
                    ModelState.AddModelError("", "La Moneda es obligatoria.");
                }


                if (!ModelState.IsValid)
                {

                    return View("Index", contrato);
                }

                if (FechaVencimiento < 1 || FechaVencimiento > 25)
                {
                    ModelState.AddModelError("", "El día de vencimiento debe estar entre 1 y 25.");
                    return View("Index", contrato);
                }

                Contrato contratoPrevio = contratoService.ObtenerContratoVigentePorInmuebleId(contrato.InmuebleId);
                Console.WriteLine(contratoPrevio);
                if (contratoPrevio == null)
                {
                    int? idUsuario = int.Parse(User.FindFirstValue("UserId"));
                    contratoService.Alta(contrato, idUsuario);

                    //Para calcular la cantidad de pagos que se van a generar necesitamos la cantidad de meses entre las dos fechas de contrato
                    int totalMeses = (contrato.FechaFin.Year - contrato.FechaInicio.Year) * 12
                        + contrato.FechaFin.Month - contrato.FechaInicio.Month + 1;

                    var fecha = new DateTime(contrato.FechaInicio.Year, contrato.FechaInicio.Month, FechaVencimiento);
                    for (int i = 0; i < totalMeses; i++)
                    {
                        Pago pago = new Pago
                        {
                            ContratoId = contrato.Id,
                            Estado = "PENDIENTE",
                            PeriodoAnio = (short)fecha.Year,
                            PeriodoMes = (byte)fecha.Month,
                            FechaVencimiento = fecha,
                            Importe = contrato.MontoMensual ?? 0,
                            Descuento = 0,
                            Recargo = 0
                        };

                        pagoService.Alta(pago, null);


                        fecha = fecha.AddMonths(1);
                    }


                    return RedirectToAction("Lista");
                }
                else
                {
                    ModelState.AddModelError("", "El inmueble ya tiene un contrato vigente.");
                    return View("Index", contrato);
                }




            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);

                ModelState.AddModelError("", "Ocurrió un error al crear el contrato.");
                return View("Index", contrato);
            }
        }

        [HttpPost]
        [Route("[controller]/Editar")]
        public IActionResult Editar(Contrato contrato)
        {
            try
            {

                int? idUsuario = int.Parse(User.FindFirstValue("UserId"));
                contratoService.Modificar(contrato, idUsuario);
                return RedirectToAction("Lista");
            }
            catch
            {
                return RedirectToAction("Lista");
            }
        }

        [HttpPost]
        [Route("[controller]/Eliminar")]
        public IActionResult Eliminar(Contrato contrato)
        {
            try
            {
                int? idUsuario = int.Parse(User.FindFirstValue("UserId"));
                Console.WriteLine(contrato.Id);
                contratoService.Baja(contrato.Id, idUsuario);

                return RedirectToAction("Lista");
            }
            catch
            {
                return RedirectToAction("Lista");
            }
        }
        
        public ActionResult Detalle(int id)
        {
            try
            {
                var contrato = contratoService.ObtenerPorId(id);
                if (contrato == null)
                {
                    return NotFound();
                }

                var inquilino = inquilinoService.ObtenerPorId(contrato.InquilinoId);
                var inmueble = inmuebleService.ObtenerPorId(contrato.InmuebleId);

                ViewBag.Inquilino = inquilino;
                ViewBag.Inmueble = inmueble;
    
                return View(contrato);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}