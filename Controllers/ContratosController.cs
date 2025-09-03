using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Services;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Inmobilaria_lab2_TPI_MGS.Models.ViewModels;

namespace Inmobilaria_lab2_TPI_MGS.Controllers
{
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
                int tamaño = 5;


                var listaInquilinos = inquilinoService.ListarInquilinosSinContrato();
                var listaInmuebles = contratoService.ListarInmueblesDisponibles(Math.Max(pagina, 1), tamaño);
                var total = inmuebleService.ObtenerCantidadInmueblesActivos();


                ViewBag.ListaInquilinos = listaInquilinos;
                ViewBag.ListaInmuebles = listaInmuebles;


                ViewBag.Pagina = pagina;
                ViewBag.TotalPaginas = total % tamaño == 0 ? total / tamaño : total / tamaño + 1;

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
            var listaContratos = contratoService.ListaContratosVigentes();

            return View(listaContratos);
        }


        [HttpPost]
        [Route("[controller]/Crear")]
        public ActionResult Crear(Contrato contrato)
        {
            try
            {

                contratoService.Alta(contrato);


                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Route("[controller]/Editar")]
        public IActionResult Editar(Contrato contrato)
        {
            try
            {
                contratoService.Modificar(contrato);
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
                Console.WriteLine(contrato.Id);
                contratoService.Baja(contrato.Id);

                return RedirectToAction("Lista");
            }
            catch
            {
                return RedirectToAction("Lista");
            }
        }
    }
}