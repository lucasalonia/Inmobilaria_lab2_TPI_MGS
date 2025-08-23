using Inmobilaria_lab2_TPI_MGS.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;

namespace Inmobilaria_lab2_TPI_MGS.Controllers
{
    public class InquilinosController : Controller
    {
        private readonly RepositorioInquilino repositorio;

        public InquilinosController()
        {
            this.repositorio = new RepositorioInquilino();
        }



        /*PENDIENTE: CONTROLAR INGRESO DE INFORMACION. SI ES CORRECTA O SI EXISTE
        GENERAR MODALES ACORDES QUE SURGAN SEGUN EL RESULTADO DE LOS METODOS DEL REPO - LS*/ 

        // GET: InquilinosController
        [Route("[controller]/Index")]
        public ActionResult Index()
        {
            try
            {
                var lista = repositorio.ObtenerTodos();
                return View(lista);
            }
            catch (Exception ex)
            {
                throw;
            }
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




        //POST: InquilinosController/Create
        [HttpPost]
        [Route("[controller]/Create")]
        public ActionResult Create(Persona persona)
        {
            try
            {

                repositorio.Alta(persona);
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
                repositorio.Modificar(persona);

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
                Console.WriteLine(persona.Id);
                repositorio.Baja(persona.Id);

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

    }
}