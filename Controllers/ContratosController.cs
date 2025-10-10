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
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Inmobilaria_lab2_TPI_MGS.Controllers
{
    [Authorize(Policy = "UserOrAdmin")]
    public class ContratosController : Controller
    {
        private readonly InquilinoService inquilinoService;
        private readonly ContratoService contratoService;
        private readonly InmuebleService inmuebleService;
        private readonly PagoService pagoService;
        private readonly UsuarioService usuarioService;

        public ContratosController(InquilinoService inquilinoService, ContratoService contratoService, InmuebleService inmuebleService, PagoService pagoService, UsuarioService usuarioService)
        {
            this.inquilinoService = inquilinoService;
            this.contratoService = contratoService;
            this.inmuebleService = inmuebleService;
            this.pagoService = pagoService;
            this.usuarioService = usuarioService;
        }

        // GET: ContratosController
        [HttpGet]
        public ActionResult Index(int pagina = 1)
        {
            try
            {
                int tamPagina = 10;
                var totalRegistros = inquilinoService.ContarInquilinosActivos();
                var totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamPagina);

                var listaInquilinos = inquilinoService.ObtenerTodos(pagina, tamPagina);


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
            var totalRegistros = contratoService.ObtenerTodos().Count;
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
                else if (contratoPrevio.Estado == "NO VIGENTE" && contratoPrevio.FechaFin < contrato.FechaInicio)
                {
                    int? idUsuario = int.Parse(User.FindFirstValue("UserId"));
                    contratoService.Alta(contrato, idUsuario);


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
        public async Task<ActionResult> RenovarContrato(Contrato contrato, int FechaVencimiento)
        {
            try
            {

                if (contrato.InmuebleId <= 0)
                    ModelState.AddModelError("", "El Inmueble es obligatorio.");
                if (contrato.InquilinoId <= 0)
                    ModelState.AddModelError("", "El Inquilino es obligatorio.");
                if (contrato.FechaInicio == default)
                    ModelState.AddModelError("", "La Fecha de Inicio es obligatoria.");
                if (contrato.FechaFin == default)
                    ModelState.AddModelError("", "La Fecha de Fin es obligatoria.");
                if (contrato.FechaFin < contrato.FechaInicio.AddMonths(6))
                    ModelState.AddModelError("", "La Fecha Fin debe ser al menos 6 meses después de la Fecha Inicio.");
                if (contrato.MontoMensual <= 0)
                    ModelState.AddModelError("", "El Monto Mensual debe ser mayor que cero.");
                if (string.IsNullOrWhiteSpace(contrato.Moneda))
                    ModelState.AddModelError("", "La Moneda es obligatoria.");
                if (FechaVencimiento < 1 || FechaVencimiento > 25)
                    ModelState.AddModelError("", "El día de vencimiento debe estar entre 1 y 25.");

                if (!ModelState.IsValid)
                    return View("Index", contrato);

                int userId = int.Parse(User.FindFirstValue("UserId"));


                var contratoPrevio = contratoService.ObtenerContratoVigentePorInmuebleId(contrato.InmuebleId);

                if (contratoPrevio != null)
                {

                    await contratoService.BajaAsync(contratoPrevio.Id, userId);
                }

                await contratoService.AltaAsync(contrato, userId); // contrato.Id se actualizará dentro de AltaAsync

                Console.WriteLine($"Nuevo ContratoId: {contrato.Id}, Estado: {contrato.Estado}");


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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelState.AddModelError("", "Ocurrió un error al renovar el contrato.");
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
                if (contrato.FechaFin.Date < contrato.FechaInicio)
                {
                    TempData["Error"] = "La fecha fin no puede ser menor que la fecha de inicio.";
                    return RedirectToAction("Lista");
                    contrato.Estado = "NO VIGENTE";
                }

                if (contrato.FechaFin.Date < DateTime.Now.Date)
                {
                    contrato.Estado = "NO VIGENTE";
                }


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

                if (contrato.CreadoPor.HasValue)
                {
                    var usuarioCreador = usuarioService.ObtenerPorId((int)contrato.CreadoPor.Value);
                    ViewBag.UsuarioCreador = usuarioCreador;
                }
                else
                {
                    ViewBag.UsuarioCreador = null;
                }

                if (contrato.TerminadoPor.HasValue)
                {
                    var usuarioTerminador = usuarioService.ObtenerPorId(contrato.TerminadoPor.Value);
                    ViewBag.UsuarioTerminador = usuarioTerminador;
                }
                else
                {
                    ViewBag.UsuarioTerminador = null;
                }

                return View(contrato);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("[controller]/Renovar/{contratoId}")]
        public ActionResult Renovar(int contratoId)
        {

            try
            {
                var contrato = contratoService.ObtenerPorId(contratoId);
                if (contrato == null)
                {
                    return NotFound();
                }
                ViewBag.Monedas = new SelectList(new List<string> { "ARS", "USD" }, contrato.Moneda);
                var inquilino = inquilinoService.ObtenerPorId(contrato.InquilinoId);
                Inmueble inmueble = inmuebleService.ObtenerPorId(contrato.InmuebleId);

                ViewBag.Persona = inquilino.Persona;
                ViewBag.Inmueble = inmueble;



                return View(contrato);
            }
            catch (Exception ex)
            {
                return RedirectToAction(ex.Message);
            }
        }



        [HttpGet]
        [Route("[controller]/Rescindir/{contratoId}")]
        public ActionResult Rescindir(int contratoId)
        {

            try
            {
                var contrato = contratoService.ObtenerPorId(contratoId);
                if (contrato == null)
                {
                    return NotFound();
                }

                if (contrato.FechaInicio.AddMonths(6).Date <= DateTime.Now.Date)
                {

                    Inquilino inquilino = inquilinoService.ObtenerPorId(contrato.InquilinoId);
                    Inmueble inmueble = inmuebleService.ObtenerPorId(contrato.InmuebleId);

                    if (contrato.FechaInicio.AddMonths(12).Date >= DateTime.Now.Date)
                    {

                        ViewBag.Multa = (contrato.MontoMensual ?? 0) * 1.5m;
                    }
                    else
                    {

                        ViewBag.Multa = contrato.MontoMensual;

                    }

                    ViewBag.Persona = inquilino.Persona;
                    ViewBag.Inmueble = inmueble;
                    ViewBag.PuedeRescindir = true;
                    return View(contrato);

                }
                else
                {
                    ViewBag.PuedeRescindir = false;
                    return View(contrato);
                }

            }
            catch (Exception ex)
            {
                return RedirectToAction(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult RescindirContrato(int ContratoId)
        {
            try
            {
                int? idUsuario = int.Parse(User.FindFirstValue("UserId"));
                contratoService.Baja(ContratoId, idUsuario);
                return RedirectToAction("Lista");
            }
            catch (Exception ex)
            {
                return RedirectToAction(ex.Message);
            }
        }
    }
}