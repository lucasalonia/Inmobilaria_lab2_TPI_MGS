using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inmobilaria_lab2_TPI_MGS.Controllers
{

    [Authorize(Policy = "UserOrAdmin")]
    public class InmuebleController : Controller
    {
        private readonly InmuebleService InmuebleService;
        private readonly PropietarioService propietarioService;
        private readonly ImagenService imagenService;
        public InmuebleController(InmuebleService InmuebleService, PropietarioService propietarioService, ImagenService imagenService)
        {
            this.InmuebleService = InmuebleService;
            this.propietarioService = propietarioService;
            this.imagenService = imagenService;
        }

        public IActionResult Index(int pagina = 1)
        {
            int tamPagina = 10;
            var totalRegistros = InmuebleService.ObtenerCantidadInmuebles();
            var totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamPagina);
            try
            {
                var lista = InmuebleService.ObtenerTodos(pagina, tamPagina);
                ViewBag.Pagina = pagina;
                ViewBag.TotalPaginas = totalPaginas;
                ViewBag.TotalRegistros = totalRegistros;
                return View(lista);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // GET: Inmueble/Disponibles
        public IActionResult Disponibles()
        {
            try
            {
                var lista = InmuebleService.BuscarDisponibles();
                ViewBag.Titulo = "Inmuebles Disponibles";
                return View("Index", lista); // reutiliza la misma vista Index
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al obtener inmuebles disponibles: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }


        // GET: Inmueble/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //PorPropietario
        public IActionResult PorPropietario(int idPropietario)
        {
            try
            {
                var lista = InmuebleService.BuscarPorPropietario(idPropietario);
                ViewBag.PropietarioId = idPropietario;
                return View("Index", lista); // Reusa la vista Index si querés
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al obtener inmuebles del propietario: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
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
            // Para mostrar el nombre del propietario actual
            string propietarioNombre = "";
            if (inmueble.PropietarioId > 0)
            {
                var propietario = propietarioService.ObtenerPorId(inmueble.PropietarioId);
                if (propietario != null)
                {
                    propietarioNombre = propietario.Persona.Nombre + " " + propietario.Persona.Apellido + " (DNI: " + propietario.Persona.Dni + ")";
                }
            }

            ViewBag.PropietarioNombre = propietarioNombre;
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
                    int? usuarioId = null;
                    var claimValue = User.FindFirst("UserId")?.Value;
                    if (int.TryParse(claimValue, out int parsedId))
                        usuarioId = parsedId;
                        
                    InmuebleService.Modificar(i, usuarioId);
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
                int? usuarioId = null;
                    var claimValue = User.FindFirst("UserId")?.Value;
                    if (int.TryParse(claimValue, out int parsedId))
                        usuarioId = parsedId;

                InmuebleService.BajaLogica(id, usuarioId);
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

        // GET: InmuebleController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                var inmueble = InmuebleService.ObtenerPorId(id);
                if (inmueble == null)
                {
                    TempData["ErrorMessage"] = "Inmueble no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                return View(inmueble);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en InmuebleController.Details: {ex.Message}");
                TempData["ErrorMessage"] = $"Error al cargar inmueble: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Inmuebles/Imagenes/5
            public ActionResult Imagenes(int id)
            {
                var inmueble = InmuebleService.ObtenerPorId(id);
                inmueble.Imagenes = imagenService.BuscarPorInmueble(id);
                return View(inmueble);
            }

        // POST: Inmuebles/Portada
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Portada(int inmuebleId, IFormFile portadaFile, [FromServices] IWebHostEnvironment env)
        {
            try
            {
                if (portadaFile == null || portadaFile.Length == 0)
                {
                    TempData["Error"] = "No se seleccionó ningún archivo.";
                    return RedirectToAction(nameof(Imagenes), new { id = inmuebleId });
                }

                string uploadsPath = Path.Combine(env.WebRootPath, "Uploads", "Inmuebles");
                if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);

                string fileName = "portada_" + inmuebleId + Path.GetExtension(portadaFile.FileName);
                string rutaFisica = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(rutaFisica, FileMode.Create))
                {
                    portadaFile.CopyTo(stream);
                }

                string url = $"/Uploads/Inmuebles/{fileName}";

                InmuebleService.ModificarPortada(inmuebleId, url);

                TempData["Mensaje"] = $"Portada actualizada correctamente.";
                return RedirectToAction(nameof(Imagenes), new { id = inmuebleId });
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error en Portada: " + ex);
                TempData["Error"] = "Error al subir la portada: " + ex.Message;
                return RedirectToAction(nameof(Imagenes), new { id = inmuebleId });
            }
        }


            // POST: Inmueble/AgregarImagen
            [HttpPost]
            [ValidateAntiForgeryToken]
            public IActionResult AgregarImagen(Imagen entidad, [FromServices] IWebHostEnvironment environment)
            {
                try
                {
                    if (entidad.Archivo != null)
                    {
                        string wwwPath = environment.WebRootPath;
                        string path = Path.Combine(wwwPath, "Uploads", "Inmuebles", entidad.InmuebleId.ToString());

                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);

                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(entidad.Archivo.FileName);
                        string rutaFisicaCompleta = Path.Combine(path, fileName);

                        using (var stream = new FileStream(rutaFisicaCompleta, FileMode.Create))
                        {
                            entidad.Archivo.CopyTo(stream);
                        }

                        entidad.Url = $"/Uploads/Inmuebles/{entidad.InmuebleId}/{fileName}";
                        imagenService.Alta(entidad);
                    }

                    TempData["Mensaje"] = "Imagen agregada correctamente";
                    return RedirectToAction(nameof(Imagenes), new { id = entidad.InmuebleId });
                }
                catch (Exception ex)
                {
                    TempData["Error"] = ex.Message;
                    return RedirectToAction(nameof(Imagenes), new { id = entidad.InmuebleId });
                }
            }

        //POST: Inmueble/
        [HttpPost]
            [ValidateAntiForgeryToken]
            public IActionResult EliminarPortada(int inmuebleId, [FromServices] IWebHostEnvironment environment)
            {
                try
                {
                    // Obtener el inmueble
                    var inmueble = InmuebleService.ObtenerPorId(inmuebleId);
                    if (inmueble == null)
                    {
                        TempData["Error"] = "Inmueble no encontrado.";
                        return RedirectToAction(nameof(Index));
                    }

                    // Si hay portada, eliminar el archivo
                    if (!string.IsNullOrEmpty(inmueble.Portada))
                    {
                        string rutaEliminar = Path.Combine(environment.WebRootPath, inmueble.Portada.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        if (System.IO.File.Exists(rutaEliminar))
                        {
                            System.IO.File.Delete(rutaEliminar);
                        }
                    }
                    // Actualizar la base de datos para quitar la portada
                    InmuebleService.QuitarPortada(inmuebleId);

                    TempData["Mensaje"] = "Portada eliminada correctamente.";
                    return RedirectToAction(nameof(Imagenes), new { id = inmuebleId });
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al eliminar la portada: " + ex.Message;
                    return RedirectToAction(nameof(Imagenes), new { id = inmuebleId });
                }
            }


        // POST: Inmueble/EliminarPortada
        [HttpPost]
            [ValidateAntiForgeryToken]
            public IActionResult EliminarImagen(int id, int inmuebleId, [FromServices] IWebHostEnvironment environment)
            {
                try
                {
                    var imagen = imagenService.BuscarPorInmueble(inmuebleId).FirstOrDefault(x => x.Id == id);
                    if (imagen != null)
                    {
                        string rutaFisica = Path.Combine(environment.WebRootPath, imagen.Url.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                        if (System.IO.File.Exists(rutaFisica))
                        {
                            System.IO.File.Delete(rutaFisica);
                        }

                        imagenService.Eliminar(id);
                    }

                    TempData["Mensaje"] = "Imagen eliminada correctamente";
                    return RedirectToAction(nameof(Imagenes), new { id = inmuebleId });
                }
                catch (Exception ex)
                {
                    TempData["Error"] = ex.Message;
                    return RedirectToAction(nameof(Imagenes), new { id = inmuebleId });
                }
            }
        [HttpGet]
        public IActionResult Contratos(int id, int pagina = 1)
        {
            try
            {
                const int tamPagina = 10;
                var paginados = InmuebleService.ObtenerContratosPorInmuebleId(id, pagina, tamPagina);
                ViewBag.InmuebleId = id;
                return PartialView("_ListaContratosInmuebleModal", paginados);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}