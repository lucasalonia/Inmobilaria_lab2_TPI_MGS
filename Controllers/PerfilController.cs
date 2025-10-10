using Inmobilaria_lab2_TPI_MGS.Models.ViewModels;
using Inmobilaria_lab2_TPI_MGS.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Inmobilaria_lab2_TPI_MGS.Controllers
{
    [Authorize]
    public class PerfilController : Controller
    {
        private readonly AuthService authService;
        private readonly UsuarioService usuarioService;

        public PerfilController(AuthService authService, UsuarioService usuarioService)
        {
            this.authService = authService;
            this.usuarioService = usuarioService;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel model, IFormFile? FotoPerfil)
        {
            try
            {
                bool isAjax = string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
                if (!ModelState.IsValid)
                {
                    var firstError = ModelState.Values.SelectMany(v => v.Errors)
                        .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? e.Exception?.Message : e.ErrorMessage)
                        .FirstOrDefault() ?? "Datos inválidos.";
                    if (isAjax) return Json(new { success = false, message = firstError });
                    TempData["ErrorMessage"] = firstError;
                    return RedirectToAction("Index", "Home");
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    if (isAjax) return Json(new { success = false, message = "No se pudo identificar al usuario actual." });
                    TempData["ErrorMessage"] = "No se pudo identificar al usuario actual.";
                    return RedirectToAction("Login", "Auth");
                }

                var usuario = usuarioService.ObtenerPorId(userId);
                if (usuario == null)
                {
                    if (isAjax) return Json(new { success = false, message = "Usuario no encontrado." });
                    TempData["ErrorMessage"] = "Usuario no encontrado.";
                    return RedirectToAction("Login", "Auth");
                }

                // Validar contraseña actual
                var passwordOk = await authService.VerificarContraseñaAsync(model.CurrentPassword, usuario.Password);
                if (!passwordOk)
                {
                    if (isAjax) return Json(new { success = false, message = "La contraseña actual es incorrecta." });
                    TempData["ErrorMessage"] = "La contraseña actual es incorrecta.";
                    return RedirectToAction("Index", "Home");
                }

                if (!string.IsNullOrWhiteSpace(model.NewPassword))
                {
                    if (model.NewPassword.Length < 6)
                    {
                        if (isAjax) return Json(new { success = false, message = "La nueva contraseña debe tener al menos 6 caracteres." });
                        TempData["ErrorMessage"] = "La nueva contraseña debe tener al menos 6 caracteres.";
                        return RedirectToAction("Index", "Home");
                    }
                    if (string.IsNullOrWhiteSpace(model.ConfirmPassword))
                    {
                        if (isAjax) return Json(new { success = false, message = "Debe confirmar la nueva contraseña." });
                        TempData["ErrorMessage"] = "Debe confirmar la nueva contraseña.";
                        return RedirectToAction("Index", "Home");
                    }
                    if (model.NewPassword != model.ConfirmPassword)
                    {
                        if (isAjax) return Json(new { success = false, message = "Las contraseñas no coinciden." });
                        TempData["ErrorMessage"] = "Las contraseñas no coinciden.";
                        return RedirectToAction("Index", "Home");
                    }
                    usuario.Password = authService.HashearContraseña(model.NewPassword);
                }
                else if (!string.IsNullOrWhiteSpace(model.ConfirmPassword))
                {
                    if (isAjax) return Json(new { success = false, message = "Debe ingresar la nueva contraseña." });
                    TempData["ErrorMessage"] = "Debe ingresar la nueva contraseña.";
                    return RedirectToAction("Index", "Home");
                }

                usuario.UserName = model.Username?.Trim() ?? usuario.UserName;

                if (model.RemoveFotoPerfil)
                {
                    if (!string.IsNullOrWhiteSpace(usuario.FotoPerfil))
                    {
                        try
                        {
                            var baseDir = Path.Combine("C:\\inmobiliaria\\perfil", usuario.Id.ToString());
                            var filePath = usuario.FotoPerfil.Replace("/perfil/", "C:\\inmobiliaria\\perfil\\").Replace("/", "\\");
                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error al eliminar archivo de foto: {ex.Message}");
                        }
                    }
                    usuario.FotoPerfil = null;
                }
                // Guardar foto si fue enviada
                else if (FotoPerfil != null && FotoPerfil.Length > 0)
                {
                    if (FotoPerfil.Length > 2 * 1024 * 1024)
                    {
                        var msg = "La imagen no puede superar 2MB";
                        if (isAjax) return Json(new { success = false, message = msg });
                        TempData["ErrorMessage"] = msg;
                        return RedirectToAction("Index", "Home");
                    }
                    var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
                    if (!allowed.Contains(FotoPerfil.ContentType))
                    {
                        var msg = "Formato de imagen no soportado (jpg, png, webp)";
                        if (isAjax) return Json(new { success = false, message = msg });
                        TempData["ErrorMessage"] = msg;
                        return RedirectToAction("Index", "Home");
                    }

                    var baseDir = Path.Combine("C:\\inmobiliaria\\perfil", usuario.Id.ToString());
                    Directory.CreateDirectory(baseDir);
                    var ext = Path.GetExtension(FotoPerfil.FileName);
                    var safeExt = string.IsNullOrWhiteSpace(ext) ? ".jpg" : ext.ToLowerInvariant();
                    var fileName = $"perfil_{DateTime.UtcNow.Ticks}{safeExt}";
                    var fullPath = Path.Combine(baseDir, fileName);
                    using (var stream = System.IO.File.Create(fullPath))
                    {
                        await FotoPerfil.CopyToAsync(stream);
                    }
                    // Guardar URL pública en BD y claims
                    var publicUrl = $"/perfil/{usuario.Id}/{fileName}";
                    usuario.FotoPerfil = publicUrl;
                }
                var actualizado = usuarioService.ActualizarUsuario(usuario);
                if (!actualizado)
                {
                    if (isAjax) return Json(new { success = false, message = "No se pudo actualizar el perfil." });
                    TempData["ErrorMessage"] = "No se pudo actualizar el perfil.";
                    return RedirectToAction("Index", "Home");
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.UserName),
                    new Claim(ClaimTypes.Email, usuario.Persona.Email),
                    new Claim("FullName", $"{usuario.Persona.Nombre} {usuario.Persona.Apellido}"),
                    new Claim("UserId", usuario.Id.ToString())
                };
                if (!string.IsNullOrWhiteSpace(usuario.FotoPerfil))
                {
                    claims.Add(new Claim("FotoPerfil", usuario.FotoPerfil));
                }

                // Cargar todos los roles activos del usuario
                var rolesActivos = usuarioService.ObtenerRolesActivosPorUsuarioId(usuario.Id);
                foreach (var rol in rolesActivos)
                {
                    claims.Add(new Claim(ClaimTypes.Role, rol.Codigo));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                if (isAjax) return Json(new { success = true, message = "Perfil actualizado correctamente.", photoUrl = usuario.FotoPerfil });
                TempData["SuccessMessage"] = "Perfil actualizado correctamente.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en PerfilController.UpdateProfile: {ex.Message}");
                bool isAjax = string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
                if (isAjax) return Json(new { success = false, message = "Error interno al actualizar perfil." });
                TempData["ErrorMessage"] = "Error interno al actualizar perfil.";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}