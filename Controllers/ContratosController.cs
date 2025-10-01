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



        public ContratosController(InquilinoService inquilinoService, ContratoService contratoService, InmuebleService inmuebleService)
        {
            this.inquilinoService = inquilinoService;
            this.contratoService = contratoService;
            this.inmuebleService = inmuebleService;
        }

        // GET: ContratosController
        [Route("[controller]/Index")]
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
        [Route("[controller]/Crear")]
        public ActionResult Crear(Contrato contrato)
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
                Contrato contratoPrevio = contratoService.ObtenerContratoVigentePorInmuebleId(contrato.InmuebleId);
                Console.WriteLine(contratoPrevio);
                if (contratoPrevio == null)
                {
                    int? idUsuario = int.Parse(User.FindFirstValue("UserId"));
                    contratoService.Alta(contrato, idUsuario);
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
    }
}