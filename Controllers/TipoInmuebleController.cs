using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inmobilaria_lab2_TPI_MGS.Controllers
{
    [Authorize]
    public class TipoInmuebleController : Controller
    {
        private readonly TipoInmuebleService tipoInmuebleService;

        public TipoInmuebleController(TipoInmuebleService tipoInmuebleService)
        {
            this.tipoInmuebleService = tipoInmuebleService;
        }

        // GET: TipoInmueble
        public IActionResult Index()
        {
            var lista = tipoInmuebleService.ObtenerTodos();
            return View(lista);
        }

        // GET: TipoInmueble/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoInmueble/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TipoInmueble entidad)
        {
            if (!ModelState.IsValid)
                return View(entidad);

            try
            {
                tipoInmuebleService.Alta(entidad);
                TempData["Mensaje"] = "Tipo de inmueble creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al crear el tipo de inmueble: " + ex.Message;
                return View(entidad);
            }
        }

        // GET: TipoInmueble/Edit/5
        public IActionResult Edit(int id)
        {
            var tipo = tipoInmuebleService.ObtenerPorId(id);
            if (tipo == null)
                return NotFound();

            return View(tipo);
        }

        // POST: TipoInmueble/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TipoInmueble entidad)
        {
            if (id != entidad.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(entidad);

            try
            {
                tipoInmuebleService.Modificar(entidad);
                TempData["Mensaje"] = "Tipo de inmueble modificado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al modificar: " + ex.Message;
                return View(entidad);
            }
        }

        // POST: TipoInmueble/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                tipoInmuebleService.Eliminar(id);
                TempData["Mensaje"] = "Tipo de inmueble eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
