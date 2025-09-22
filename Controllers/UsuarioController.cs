using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Models.ViewModels;
using Inmobilaria_lab2_TPI_MGS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;

namespace Inmobilaria_lab2_TPI_MGS.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class UsuarioController : Controller
    {
        private readonly UsuarioService usuarioService;
        private readonly RolService rolService;
        private readonly UsuarioRolService usuarioRolService;
        private readonly AuthService authService;

        public UsuarioController(UsuarioService usuarioService, RolService rolService, UsuarioRolService usuarioRolService, AuthService authService)
        {
            this.usuarioService = usuarioService;
            this.rolService = rolService;
            this.usuarioRolService = usuarioRolService;
            this.authService = authService;
        }

        // GET: UsuarioController
        public ActionResult Index(int pageIndex = 1)
        {
            try
            {
                pageIndex = Math.Max(1, pageIndex);
                const int pageSize = 10;
                
                var usuariosPaginados = usuarioService.ObtenerTodosPaginados(pageIndex, pageSize);
                
                // Cargar roles para cada usuario
                foreach (var usuario in usuariosPaginados.Items)
                {
                    var rolActual = usuarioRolService.ObtenerActivoPorUsuarioId(usuario.Id);
                    if (rolActual != null)
                    {
                        usuario.RolActual = rolActual.Rol;
                    }
                }
                
                return View(usuariosPaginados);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UsuarioController.Index: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                ViewBag.ErrorMessage = $"Error al cargar usuarios: {ex.Message}";
                return View(new PaginatedListViewModel<Usuario>(new List<Usuario>(), 0, 1, 10));
            }
        }

        // GET: UsuarioController/Create
        public ActionResult Create()
        {
            try
            {
                var roles = rolService.ObtenerTodos();
                ViewBag.Roles = roles;
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UsuarioController.Create GET: {ex.Message}");
                ViewBag.ErrorMessage = $"Error al cargar roles: {ex.Message}";
            return View();
            }
        }

        // POST: UsuarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Usuario usuario, int? rolId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    usuario.Password = authService.HashearContraseña(usuario.Password);
                    
                    int usuarioId = usuarioService.CrearUsuario(usuario);
                    
                    if (usuarioId > 0)
                    {
                        if (rolId.HasValue && rolId.Value > 0)
                        {
                            bool rolAsignado = usuarioRolService.AsignarRolAUsuario(usuarioId, rolId.Value);
                            if (!rolAsignado)
                            {
                                Console.WriteLine($"Advertencia: No se pudo asignar el rol {rolId.Value} al usuario {usuarioId}");
                            }
                        }
                        
                        TempData["SuccessMessage"] = "Usuario creado exitosamente.";
                return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Error al crear el usuario. Intente nuevamente.";
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Por favor, corrija los errores en el formulario.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UsuarioController.Create: {ex.Message}");
                ViewBag.ErrorMessage = $"Error al crear usuario: {ex.Message}";
            }
            
            try
            {
                var roles = rolService.ObtenerTodos();
                ViewBag.Roles = roles;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al recargar roles: {ex.Message}");
            }
            
            return View(usuario);
        }


        // GET: UsuarioController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                var usuario = usuarioService.ObtenerPorId(id);
                if (usuario == null)
                {
                    TempData["ErrorMessage"] = "Usuario no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                var rolActual = usuarioRolService.ObtenerActivoPorUsuarioId(id);
                if (rolActual != null)
                {
                    usuario.RolActual = rolActual.Rol;
                }

                return View(usuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UsuarioController.Details: {ex.Message}");
                TempData["ErrorMessage"] = $"Error al cargar usuario: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: UsuarioController/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var usuario = usuarioService.ObtenerPorId(id);
                if (usuario == null)
                {
                    TempData["ErrorMessage"] = "Usuario no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                var roles = rolService.ObtenerTodos();
                ViewBag.Roles = roles;

                var rolActual = usuarioRolService.ObtenerActivoPorUsuarioId(id);
                ViewBag.RolActual = rolActual?.Rol?.Id;

                return View(usuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UsuarioController.Edit GET: {ex.Message}");
                TempData["ErrorMessage"] = $"Error al cargar usuario: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: UsuarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Usuario usuario, int? rolId, string? nuevaPassword, string? confirmarPassword)
        {
            try
            {
                if (id != usuario.Id)
                {
                    ViewBag.ErrorMessage = "ID de usuario no coincide.";
                    return View(usuario);
                }

                // Validar que las contraseñas coincidan si se proporcionan
                if (!string.IsNullOrEmpty(nuevaPassword) && nuevaPassword != confirmarPassword)
                {
                    ViewBag.ErrorMessage = "Las contraseñas no coinciden.";
                    ModelState.AddModelError("confirmarPassword", "Las contraseñas no coinciden.");
                }

                if (ModelState.IsValid)
                {
                    var usuarioActual = usuarioService.ObtenerPorId(id);
                    if (usuarioActual != null)
                    {
                        // Si se proporciona una nueva contraseña, hashearla con BCrypt
                        if (!string.IsNullOrEmpty(nuevaPassword))
                        {
                            usuario.Password = authService.HashearContraseña(nuevaPassword);
                        }
                        else
                        {
                            // Mantener la contraseña actual si no se proporciona una nueva
                            usuario.Password = usuarioActual.Password;
                        }
                    }

                    bool actualizado = usuarioService.ActualizarUsuario(usuario);
                    if (actualizado)
                    {
                        if (rolId.HasValue)
                        {
                            if (rolId.Value > 0)
                            {
                                bool rolActualizado = usuarioRolService.CambiarRolUsuario(id, rolId.Value);
                                if (!rolActualizado)
                                {
                                    Console.WriteLine($"Advertencia: No se pudo actualizar el rol del usuario {id}");
                                }
                            }
                            else
                            {
                                usuarioRolService.DeshabilitarRolesUsuario(id);
                            }
                        }

                        TempData["SuccessMessage"] = "Usuario actualizado exitosamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Error al actualizar el usuario. Intente nuevamente.";
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Por favor, corrija los errores en el formulario.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UsuarioController.Edit POST: {ex.Message}");
                ViewBag.ErrorMessage = $"Error al actualizar usuario: {ex.Message}";
            }
            
            try
            {
                var roles = rolService.ObtenerTodos();
                ViewBag.Roles = roles;
                var rolActual = usuarioRolService.ObtenerActivoPorUsuarioId(id);
                ViewBag.RolActual = rolActual?.Rol?.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al recargar datos: {ex.Message}");
            }
            
            return View(usuario);
        }

        // GET: UsuarioController/Disable/5
        public ActionResult Disable(int id)
        {
            try
            {
                var usuario = usuarioService.ObtenerPorId(id);
                if (usuario == null)
                {
                    TempData["ErrorMessage"] = "Usuario no encontrado.";
                    return RedirectToAction(nameof(Index));
                }
                return View(usuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UsuarioController.Disable GET: {ex.Message}");
                TempData["ErrorMessage"] = $"Error al cargar usuario: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: UsuarioController/Disable/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disable(int id, IFormCollection collection)
        {
            try
            {
                bool deshabilitado = usuarioService.DeshabilitarUsuario(id);
                if (deshabilitado)
                {
                    TempData["SuccessMessage"] = "Usuario deshabilitado exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al deshabilitar el usuario. Intente nuevamente.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UsuarioController.Disable POST: {ex.Message}");
                TempData["ErrorMessage"] = $"Error al deshabilitar usuario: {ex.Message}";
            }
            
            return RedirectToAction(nameof(Index));
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