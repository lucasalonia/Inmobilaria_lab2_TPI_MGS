using Inmobilaria_lab2_TPI_MGS.Repository;
using Inmobilaria_lab2_TPI_MGS.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Repos
builder.Services.AddScoped<PropietarioRepository>();
builder.Services.AddScoped<InquilinoRepository>();
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<RolRepository>();
builder.Services.AddScoped<UsuarioRolRepository>();
// HEAD
builder.Services.AddScoped<InmuebleRepository>();
builder.Services.AddScoped<PersonaRepository>();

builder.Services.AddScoped<ContratoRepository>();
// Modulo-Contrato

// Servicios
builder.Services.AddScoped<PropietarioService, PropietarioServiceImpl>();
builder.Services.AddScoped<InquilinoService, InquilinoServiceImpl>();
builder.Services.AddScoped<UsuarioService, UsuarioServiceImpl>();
builder.Services.AddScoped<RolService, RolServiceImpl>();
builder.Services.AddScoped<UsuarioRolService, UsuarioRolServiceImpl>();

// HEAD
builder.Services.AddScoped<InmuebleService, InmuebleServiceImpl>();
builder.Services.AddScoped<PersonService, PersonServiceImpl>();

builder.Services.AddScoped<ContratoService, ContratoServiceImpl>();
// Modulo-Contrato

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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
