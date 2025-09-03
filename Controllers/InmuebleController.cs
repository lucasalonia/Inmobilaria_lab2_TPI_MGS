using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inmobilaria_lab2_TPI_MGS.Controllers
{

    public class InmuebleController : Controller
    {
        private readonly InmuebleService InmuebleService;
        public InmuebleController(InmuebleService InmuebleService)
        {
            this.InmuebleService = InmuebleService;
        }

        [Route("[controller]/Index")]
        public ActionResult Index()
        {
            try
            {
                var lista = InmuebleService.ObtenerTodos();
                return View(lista);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // GET: Inmueble/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inmueble/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inmueble inmueble)
        {
            if (ModelState.IsValid)
            {
                InmuebleService.Alta(inmueble);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View(inmueble);
            }
        }

        // GET: Inmueble/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var inmueble = InmuebleService.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            return View(inmueble);
        }

        // POST: Inmueble/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inmueble i)
        {
            if (id != i.Id)
            {
                return BadRequest();// ID no coincide
            }

            if (ModelState.IsValid)
            {
                try
                {
                    InmuebleService.Modificar(i);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Manejo de errores si algo falla en el update
                    ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                }
            }
            return View(i);
        }

        // GET: Inmueble/Delete/5
        public IActionResult Delete(int id)
        {
            var inmueble = InmuebleService.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            return View(inmueble);
        }

        // POST: Inmueble/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                InmuebleService.BajaLogica(id, 1);
                TempData["Mensaje"] = "Inmueble eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }


        public IActionResult ListarParaSeleccionInmuebles(int pagina = 1)
        {
            try
            {
                int tamPagina = 5;
                var totalRegistros = InmuebleService.ObtenerCantidadInmueblesActivos();
                var totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamPagina);


                var lista = InmuebleService.ObtenerTodosParaContratos(pagina, tamPagina);

                ViewBag.Pagina = pagina;
                ViewBag.TotalPaginas = totalPaginas;

                return PartialView("_ListaInmueblesSeleccion", lista);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}