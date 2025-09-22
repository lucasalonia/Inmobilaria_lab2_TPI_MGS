
using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inmobilaria_lab2_TPI_MGS.Controllers
{

    [Authorize(Policy = "UserOrAdmin")]
    public class PropietariosController : Controller
    {
        private readonly PropietarioService propietarioService;
        public PropietariosController(PropietarioService propietarioService)
        {
            this.propietarioService = propietarioService;
        }



        public IActionResult Index(int pagina = 1)
        {
            int tamPagina = 10;
            var totalRegistros = propietarioService.ContarPropietarios();
            var totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamPagina);

            var lista = propietarioService.ObtenerTodos(pagina, tamPagina);

            ViewBag.Pagina = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.TotalRegistros = totalRegistros;
            return View(lista);
        }

        public IActionResult ListarParaSeleccion(int pagina = 1)
        {
            int tamPagina = 5;
            var totalRegistros = propietarioService.ContarPropietariosActivos();
            var totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamPagina);

            var lista = propietarioService.ObtenerTodos(pagina, tamPagina);

            ViewBag.Pagina = pagina;
            ViewBag.TotalPaginas = totalPaginas;

            return PartialView("_ListaPropietariosSeleccion", lista);
        }


        [HttpGet]
        public IActionResult BuscarPorDni(string dni)
        {
            var persona = propietarioService.ObtenerPorDni(dni);

            var modelo = new Propietario
            {
                Persona = persona ?? new Persona(),
                Estado = "ACTIVO"
            };

            if (persona == null)
                TempData["Msg"] = $"No existe persona con DNI {dni}, puede crear nuevo propietario.";
            else
                TempData["Msg"] = $"Persona encontrada. Los datos han sido cargados.";

            return View("Create", modelo); // <-- devuelve Propietario, no Persona
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

            if (string.IsNullOrEmpty(modelo.Persona.Dni) || string.IsNullOrEmpty(modelo.Persona.Nombre) || string.IsNullOrEmpty(modelo.Persona.Apellido) || string.IsNullOrEmpty(modelo.Persona.Sexo))
            {
                ModelState.AddModelError("", "Debe completar DNI, sexo, Nombre y Apellido obligatoriamente.");
                return View(modelo);
            }
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
            return View(propietario);
        }

        // POST: Propietarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                propietarioService.BajaLogica(id, 1);
                TempData["Mensaje"] = "Propietario eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: PropietarioController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                var propietario = propietarioService.ObtenerPorId(id);
                if (propietario == null)
                {
                    TempData["ErrorMessage"] = "Propietario no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                return View(propietario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en PropietarioController.Details: {ex.Message}");
                TempData["ErrorMessage"] = $"Error al cargar Propietario: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

    }
}