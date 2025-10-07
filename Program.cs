using Inmobilaria_lab2_TPI_MGS.Repository;
using Inmobilaria_lab2_TPI_MGS.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Autenticacion
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("ADMINISTRADOR"));
    options.AddPolicy("UserOrAdmin", policy => policy.RequireRole("EMPLEADO", "ADMINISTRADOR"));
    options.AddPolicy("RequireAuthentication", policy => policy.RequireAuthenticatedUser());
});

// Repos
builder.Services.AddScoped<PropietarioRepository>();
builder.Services.AddScoped<InquilinoRepository>();
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<RolRepository>();
builder.Services.AddScoped<UsuarioRolRepository>();
builder.Services.AddScoped<PagoRepository>();
builder.Services.AddScoped<TipoInmuebleRepository>();
// HEAD
builder.Services.AddScoped<InmuebleRepository>();
builder.Services.AddScoped<PersonaRepository>();
builder.Services.AddScoped<ImagenRepository>();
builder.Services.AddScoped<ContratoRepository>();
// Modulo-Contrato

// Servicios
builder.Services.AddScoped<PropietarioService, PropietarioServiceImpl>();
builder.Services.AddScoped<InquilinoService, InquilinoServiceImpl>();
builder.Services.AddScoped<UsuarioService, UsuarioServiceImpl>();
builder.Services.AddScoped<RolService, RolServiceImpl>();
builder.Services.AddScoped<UsuarioRolService, UsuarioRolServiceImpl>();
builder.Services.AddScoped<PagoService, PagoServiceImpl>();
builder.Services.AddScoped<TipoInmuebleService, TipoInmuebleServiceImpl>();

// HEAD
builder.Services.AddScoped<InmuebleService, InmuebleServiceImpl>();
builder.Services.AddScoped<PersonService, PersonServiceImpl>();
builder.Services.AddScoped<ImagenService, ImagenServiceImpl>();
builder.Services.AddScoped<ContratoService, ContratoServiceImpl>();
// Modulo-Contrato

// Auth Service
builder.Services.AddScoped<AuthService, AuthServiceImpl>();

// Dashboard Service
builder.Services.AddScoped<DashboardService, DashboardServiceImpl>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Exponer fotos de perfil desde C:\inmobiliaria\perfil bajo /perfil
var perfilRoot = @"C:\\inmobiliaria\\perfil";
try { Directory.CreateDirectory(perfilRoot); } catch { /* ignore */ }
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(perfilRoot),
    RequestPath = "/perfil"
});

app.UseRouting();

// Middleware 
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
