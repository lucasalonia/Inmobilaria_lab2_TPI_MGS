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

    }
}