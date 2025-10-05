using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Inmobilaria_lab2_TPI_MGS.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService authService;

        public AuthController(AuthService authService)
        {
            this.authService = authService;
        }

        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.ReturnUrl = returnUrl;
                    return View(model);
                }

                var usuario = await authService.AutenticarUsuarioAsync(model.Username, model.Password);
                
                if (usuario == null)
                {
                    ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                    ViewBag.ReturnUrl = returnUrl;
                    return View(model);
                }

                // Crear claims para el usuario
                string? fotoPerfilClaim = null;
                if (!string.IsNullOrWhiteSpace(usuario.FotoPerfil))
                {
                    var fp = usuario.FotoPerfil;
                    if (fp!.Contains("inmobiliaria\\perfil"))
                    {
                        try
                        {
                            var fileName = System.IO.Path.GetFileName(fp);
                            fotoPerfilClaim = $"/perfil/{usuario.Id}/{fileName}";
                        }
                        catch { fotoPerfilClaim = null; }
                    }
                    else
                    {
                        fotoPerfilClaim = fp.Replace('\\', '/');
                    }
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.UserName),
                    new Claim(ClaimTypes.Email, usuario.Persona.Email),
                    new Claim("FullName", $"{usuario.Persona.Nombre} {usuario.Persona.Apellido}"),
                    new Claim("UserId", usuario.Id.ToString())
                };

                if (!string.IsNullOrWhiteSpace(fotoPerfilClaim))
                {
                    claims.Add(new Claim("FotoPerfil", fotoPerfilClaim));
                }

                // Agregar rol si existe
                if (usuario.RolActual != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, usuario.RolActual.Codigo));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(8)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                // Actualizar último login
                await authService.ActualizarUltimoLoginAsync(usuario.Id);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AuthController.Login: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Error interno del servidor. Intente nuevamente.");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }
    }
}
