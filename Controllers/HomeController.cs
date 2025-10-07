using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Services;
using Inmobilaria_lab2_TPI_MGS.Models.ViewModels;

namespace Inmobilaria_lab2_TPI_MGS.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly DashboardService _dashboardService;

    public HomeController(ILogger<HomeController> logger, DashboardService dashboardService)
    {
        _logger = logger;
        _dashboardService = dashboardService;
    }

    [Authorize]
    public IActionResult Index()
    {
        var dashboardData = _dashboardService.ObtenerDatosDashboard();
        return View(dashboardData);
    }

    [Authorize]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
