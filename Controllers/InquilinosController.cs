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
    public class InquilinosController : Controller
    {
        private readonly InquilinoService inquilinoService;
        private readonly ContratoService contratoService;

        public InquilinosController(InquilinoService inquilinoService, ContratoService contratoService)
        {
            this.inquilinoService = inquilinoService;
            this.contratoService = contratoService;
        }



        /*PENDIENTE: CONTROLAR INGRESO DE INFORMACION. SI ES CORRECTA O SI EXISTE
        GENERAR MODALES ACORDES QUE SURGAN SEGUN EL RESULTADO DE LOS METODOS DEL REPO - LS*/

        // GET: InquilinosController
        [HttpGet]
        public IActionResult Index(int pagina = 1)
        {

            int tamPagina = 10;
            var totalRegistros = inquilinoService.ContarInquilinosActivos();
            var totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamPagina);

            var listaInquilinos = inquilinoService.ObtenerTodos(pagina, tamPagina);
            var listaViewModel = new List<InquilinoViewModel>();

            foreach (var inquilino in listaInquilinos)
            {
                var contrato = contratoService.ObtenerContratoVigente(inquilino.Id);

                if (contrato == null)
                {
                    contrato = new Contrato
                    {
                        Estado = "SIN CONTRATO"
                    };
                }

                listaViewModel.Add(new InquilinoViewModel
                {
                    Persona = inquilino.Persona,
                    Inquilino = inquilino,
                    Contrato = contrato
                });
            }
            ViewBag.Pagina = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.TotalRegistros = totalRegistros;
            return View(listaViewModel);
        }

        [Route("[controller]/Agregar")]
        public ActionResult Agregar()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
         [HttpGet]
        public IActionResult BuscarPorDni(string dni)
        {
            var persona = inquilinoService.ObtenerPorDni(dni);

            var modelo = new Inquilino
            {
                Persona = persona ?? new Persona(),
                Estado = "ACTIVO"
            };

            if (persona == null)
                TempData["MsgNull"] = $"No existe persona con DNI {dni}, puede crear nuevo propietario.";
            else
                TempData["Msg"] = $"Persona encontrada. Los datos han sido cargados.";

            return View("Agregar", modelo); 
        }




        //POST: InquilinosController/Create
        [HttpPost]
        [Route("[controller]/Create")]
        public ActionResult Create(Persona persona)
        {
            try
            {
                int? idUsuario = int.Parse(User.FindFirstValue("UserId"));

                inquilinoService.Alta(persona, idUsuario);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
                throw;
            }
        }

        [HttpPost]
        public IActionResult Editar(Persona persona)
        {
            try
            {
                int? idUsuario = int.Parse(User.FindFirstValue("UserId"));
                inquilinoService.Modificar(persona, idUsuario);

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Eliminar(Persona persona)
        {
            try
            {
                int? idUsuario = int.Parse(User.FindFirstValue("UserId"));
                inquilinoService.Baja(persona.Id, idUsuario);

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

    }
}