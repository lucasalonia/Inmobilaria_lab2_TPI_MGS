using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Models.ViewModels;
using Inmobilaria_lab2_TPI_MGS.Repository;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public interface DashboardService
    {
        DashboardViewModel ObtenerDatosDashboard();
        List<ContratoPorMes> ObtenerContratosPorMes();
        List<IngresosPorMes> ObtenerIngresosPorMes();
        List<InmueblesPorTipo> ObtenerInmueblesPorTipo();
        List<ContratosProximosVencer> ObtenerContratosProximosVencer();
        List<PagosPendientes> ObtenerPagosPendientes();
        List<AccesoDirecto> ObtenerAccesosDirectos();
    }
}
