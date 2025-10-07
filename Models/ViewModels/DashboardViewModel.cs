using Inmobilaria_lab2_TPI_MGS.Models;

namespace Inmobilaria_lab2_TPI_MGS.Models.ViewModels
{
    public class DashboardViewModel
    {
        // Métricas principales
        public int TotalPropietarios { get; set; }
        public int TotalInquilinos { get; set; }
        public int TotalInmuebles { get; set; }
        public int TotalContratosVigentes { get; set; }
        public int TotalContratosActivos { get; set; }
        public int TotalPagos { get; set; }
        public decimal IngresosMensuales { get; set; }
        public decimal IngresosAnuales { get; set; }

        // Datos para gráficos
        public List<ContratoPorMes> ContratosPorMes { get; set; } = new List<ContratoPorMes>();
        public List<IngresosPorMes> IngresosPorMes { get; set; } = new List<IngresosPorMes>();
        public List<InmueblesPorTipo> InmueblesPorTipo { get; set; } = new List<InmueblesPorTipo>();
        public List<ContratosProximosVencer> ContratosProximosVencer { get; set; } = new List<ContratosProximosVencer>();
        public List<PagosPendientes> PagosPendientes { get; set; } = new List<PagosPendientes>();

        // Accesos directos
        public List<AccesoDirecto> AccesosDirectos { get; set; } = new List<AccesoDirecto>();
    }

    public class ContratoPorMes
    {
        public string Mes { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }

    public class IngresosPorMes
    {
        public string Mes { get; set; } = string.Empty;
        public decimal Monto { get; set; }
    }

    public class InmueblesPorTipo
    {
        public string Tipo { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }

    public class ContratosProximosVencer
    {
        public int Id { get; set; }
        public string Inmueble { get; set; } = string.Empty;
        public string Inquilino { get; set; } = string.Empty;
        public DateTime FechaVencimiento { get; set; }
        public int DiasRestantes { get; set; }
        public decimal MontoMensual { get; set; }
    }

    public class PagosPendientes
    {
        public int Id { get; set; }
        public string Inquilino { get; set; } = string.Empty;
        public string Inmueble { get; set; } = string.Empty;
        public int Mes { get; set; }
        public int Año { get; set; }
        public decimal Monto { get; set; }
        public int DiasVencido { get; set; }
    }

    public class AccesoDirecto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Icono { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public string Controlador { get; set; } = string.Empty;
    }
}
