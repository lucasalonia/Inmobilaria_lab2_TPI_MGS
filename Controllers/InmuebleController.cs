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

    }
}