using Google.Protobuf;
using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inmobilaria_lab2_TPI_MGS.Controllers
{

    public class PropietariosController : Controller
    {
        private readonly PropietarioService propietarioService;
        public PropietariosController(PropietarioService propietarioService)
        {
            this.propietarioService = propietarioService;
        }

        [Route("[controller]/Index")]
        public ActionResult Index()
        {
            try
            {
                var lista = propietarioService.ObtenerTodos();
                return View(lista);// Views/Propietarios/Index.cshtml
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // GET: Propietarios/Create
        [HttpGet]
        public IActionResult Create()
        {
            // inicializa Persona
            var modelo = new Propietario
            {
                Persona = new Persona(),
                Estado = "ACTIVO",
            };
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Propietario modelo)
        {
            if (modelo.Persona == null)
                modelo.Persona = new Persona();

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            try
            {
                // Este Alta inserta PERSONA y luego PROPIETARIO
                var nuevoId = propietarioService.Alta(modelo);

                TempData["Msg"] = $"Propietario creado (ID {nuevoId}).";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "No se pudo guardar: " + ex.Message);
                return View(modelo);
            }
        }


        // GET: Propietarios/Edit/5
        public IActionResult Edit(int id)
        {
            var propietario = propietarioService.ObtenerPorId(id);
            if (propietario == null)
            {
                return NotFound();
            }

            return View(propietario); //misma vista que Create, pero con datos cargados
        }

        // POST: Propietarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Propietario p)
        {
            if (id != p.Id)
            {
                return BadRequest();// ID no coincide
            }

            if (ModelState.IsValid)
            {
                try
                {
                    propietarioService.Modificar(p);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Manejo de errores si algo falla en el update
                    ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                }
            }
            return View(p);
        }
        
        
        // GET: Propietarios/Delete/5
        public IActionResult Delete(int id)
        {
            var propietario = propietarioService.ObtenerPorId(id);
            if (propietario == null)
            {
                return NotFound();
            }
            return View(propietario); // opcional: vista de confirmación
        }

        // POST: Propietarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                propietarioService.BajaLogica(id, 1); // acá podrías pasar el usuario logueado como modificadoPor
                TempData["Mensaje"] = "Propietario eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

    }
}