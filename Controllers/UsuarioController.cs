using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Services;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;

namespace Inmobilaria_lab2_TPI_MGS.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UsuarioService usuarioService;

        public UsuarioController(UsuarioService usuarioService)
        {
            this.usuarioService = usuarioService;
        }

        // GET: UsuarioController
        public ActionResult Index()
        {
            try
            {
                var lista = usuarioService.ObtenerTodos();
                return View(lista);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UsuarioController.Index: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                ViewBag.ErrorMessage = $"Error al cargar usuarios: {ex.Message}";
                return View(new List<Usuario>());
            }
        }

        // GET: UsuarioController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsuarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO llamada al service para crear el usuario
                // Por ahora solo redirigimos al Index
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UsuarioController.Create: {ex.Message}");
                ViewBag.ErrorMessage = $"Error al crear usuario: {ex.Message}";
                return View();
            }
        }

        // GET: UsuarioController/BuscarPersona
        [HttpGet]
        public JsonResult BuscarPersona(string dni)
        {
            try
            {
                var persona = usuarioService.BuscarPersona(dni);
                if (persona != null)
                {
                    return Json(new { 
                        success = true, 
                        persona = new {
                            dni = persona.Dni,
                            sexo = persona.Sexo,
                            nombre = persona.Nombre,
                            apellido = persona.Apellido,
                            email = persona.Email,
                            telefono = persona.Telefono,
                            fechaNacimiento = persona.FechaNacimiento?.ToString("yyyy-MM-dd")
                        }
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Persona no encontrada" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

    }
}