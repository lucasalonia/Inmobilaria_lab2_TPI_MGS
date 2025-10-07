using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Models.ViewModels;
using MySql.Data.MySqlClient;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public class DashboardServiceImpl : DashboardService
    {
        private readonly string connectionString;

        public DashboardServiceImpl(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("MySqlRemote") ?? 
                              configuration.GetConnectionString("MySqlLocal") ?? 
                              throw new InvalidOperationException("Connection string not found");
        }

        public DashboardViewModel ObtenerDatosDashboard()
        {
            return new DashboardViewModel
            {
                TotalPropietarios = ObtenerTotalPropietarios(),
                TotalInquilinos = ObtenerTotalInquilinos(),
                TotalInmuebles = ObtenerTotalInmuebles(),
                TotalContratosVigentes = ObtenerTotalContratosVigentes(),
                TotalContratosActivos = ObtenerTotalContratosActivos(),
                TotalPagos = ObtenerTotalPagos(),
                IngresosMensuales = ObtenerIngresosMensuales(),
                IngresosAnuales = ObtenerIngresosAnuales(),
                ContratosPorMes = ObtenerContratosPorMes(),
                IngresosPorMes = ObtenerIngresosPorMes(),
                InmueblesPorTipo = ObtenerInmueblesPorTipo(),
                ContratosProximosVencer = ObtenerContratosProximosVencer(),
                PagosPendientes = ObtenerPagosPendientes(),
                AccesosDirectos = ObtenerAccesosDirectos()
            };
        }

        private int ObtenerTotalPropietarios()
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var sql = "SELECT COUNT(*) FROM propietario WHERE estado = 'ACTIVO'";
            using var command = new MySqlCommand(sql, connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        private int ObtenerTotalInquilinos()
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var sql = "SELECT COUNT(*) FROM inquilino WHERE estado = 'ACTIVO'";
            using var command = new MySqlCommand(sql, connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        private int ObtenerTotalInmuebles()
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var sql = "SELECT COUNT(*) FROM inmueble WHERE estado = 'ACTIVO'";
            using var command = new MySqlCommand(sql, connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        private int ObtenerTotalContratosVigentes()
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var sql = @"SELECT COUNT(*) FROM contrato 
                       WHERE estado = 'VIGENTE' 
                       AND fecha_fin >= CURDATE() 
                       AND fecha_inicio <= CURDATE()";
            using var command = new MySqlCommand(sql, connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        private int ObtenerTotalContratosActivos()
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var sql = "SELECT COUNT(*) FROM contrato WHERE estado = 'VIGENTE'";
            using var command = new MySqlCommand(sql, connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        private int ObtenerTotalPagos()
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var sql = "SELECT COUNT(*) FROM pago";
            using var command = new MySqlCommand(sql, connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        private decimal ObtenerIngresosMensuales()
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var sql = @"SELECT COALESCE(SUM(p.importe_pagado), 0) 
                       FROM pago p 
                       WHERE p.fecha_pago IS NOT NULL
                       AND MONTH(p.fecha_pago) = MONTH(CURDATE()) 
                       AND YEAR(p.fecha_pago) = YEAR(CURDATE())";
            using var command = new MySqlCommand(sql, connection);
            var result = command.ExecuteScalar();
            return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
        }

        private decimal ObtenerIngresosAnuales()
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var sql = @"SELECT COALESCE(SUM(p.importe_pagado), 0) 
                       FROM pago p 
                       WHERE p.fecha_pago IS NOT NULL
                       AND YEAR(p.fecha_pago) = YEAR(CURDATE())";
            using var command = new MySqlCommand(sql, connection);
            var result = command.ExecuteScalar();
            return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
        }

        public List<ContratoPorMes> ObtenerContratosPorMes()
        {
            var contratos = new List<ContratoPorMes>();
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var sql = @"SELECT 
                       MONTHNAME(fecha_inicio) as mes,
                       COUNT(*) as cantidad
                       FROM contrato 
                       WHERE YEAR(fecha_inicio) = YEAR(CURDATE())
                       GROUP BY MONTH(fecha_inicio), MONTHNAME(fecha_inicio)
                       ORDER BY MONTH(fecha_inicio)";
            using var command = new MySqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                contratos.Add(new ContratoPorMes
                {
                    Mes = reader.GetString("mes"),
                    Cantidad = reader.GetInt32("cantidad")
                });
            }
            return contratos;
        }

        public List<IngresosPorMes> ObtenerIngresosPorMes()
        {
            var ingresos = new List<IngresosPorMes>();
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var sql = @"SELECT 
                       MONTHNAME(fecha_pago) as mes,
                       COALESCE(SUM(importe_pagado), 0) as monto
                       FROM pago 
                       WHERE fecha_pago IS NOT NULL
                       AND YEAR(fecha_pago) = YEAR(CURDATE())
                       GROUP BY MONTH(fecha_pago), MONTHNAME(fecha_pago)
                       ORDER BY MONTH(fecha_pago)";
            using var command = new MySqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                ingresos.Add(new IngresosPorMes
                {
                    Mes = reader.GetString("mes"),
                    Monto = reader.GetDecimal("monto")
                });
            }
            return ingresos;
        }

        public List<InmueblesPorTipo> ObtenerInmueblesPorTipo()
        {
            var inmuebles = new List<InmueblesPorTipo>();
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var sql = @"SELECT 
                       COALESCE(t.nombre, 'Sin tipo') as tipo,
                       COUNT(*) as cantidad
                       FROM inmueble i
                       LEFT JOIN tipo_inmueble t ON i.tipo_inmueble_id = t.id
                       WHERE i.estado = 'ACTIVO'
                       GROUP BY t.nombre
                       ORDER BY cantidad DESC";
            using var command = new MySqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                inmuebles.Add(new InmueblesPorTipo
                {
                    Tipo = reader.GetString("tipo"),
                    Cantidad = reader.GetInt32("cantidad")
                });
            }
            return inmuebles;
        }

        public List<ContratosProximosVencer> ObtenerContratosProximosVencer()
        {
            var contratos = new List<ContratosProximosVencer>();
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var sql = @"SELECT 
                       c.id,
                       CONCAT(i.direccion, ' - ', COALESCE(t.nombre, 'Sin tipo')) as inmueble,
                       CONCAT(p.nombre, ' ', p.apellido) as inquilino,
                       c.fecha_fin,
                       DATEDIFF(c.fecha_fin, CURDATE()) as dias_restantes,
                       c.monto_mensual
                       FROM contrato c
                       JOIN inmueble i ON c.inmueble_id = i.id
                       LEFT JOIN tipo_inmueble t ON i.tipo_inmueble_id = t.id
                       JOIN inquilino inq ON c.inquilino_id = inq.id
                       JOIN persona p ON inq.persona_id = p.id
                       WHERE c.estado = 'VIGENTE'
                       AND c.fecha_fin BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL 30 DAY)
                       ORDER BY c.fecha_fin ASC
                       LIMIT 5";
            using var command = new MySqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                contratos.Add(new ContratosProximosVencer
                {
                    Id = reader.GetInt32("id"),
                    Inmueble = reader.GetString("inmueble"),
                    Inquilino = reader.GetString("inquilino"),
                    FechaVencimiento = reader.GetDateTime("fecha_fin"),
                    DiasRestantes = reader.GetInt32("dias_restantes"),
                    MontoMensual = reader.GetDecimal("monto_mensual")
                });
            }
            return contratos;
        }

        public List<PagosPendientes> ObtenerPagosPendientes()
        {
            var pagos = new List<PagosPendientes>();
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var sql = @"SELECT 
                       pa.id,
                       CONCAT(p.nombre, ' ', p.apellido) as inquilino,
                       CONCAT(i.direccion, ' - ', COALESCE(t.nombre, 'Sin tipo')) as inmueble,
                       pa.periodo_mes as mes,
                       pa.periodo_anio as año,
                       pa.importe as monto,
                       DATEDIFF(CURDATE(), pa.fecha_vencimiento) as dias_vencido
                       FROM pago pa
                       JOIN contrato c ON pa.contrato_id = c.id
                       JOIN inmueble i ON c.inmueble_id = i.id
                       LEFT JOIN tipo_inmueble t ON i.tipo_inmueble_id = t.id
                       JOIN inquilino inq ON c.inquilino_id = inq.id
                       JOIN persona p ON inq.persona_id = p.id
                       WHERE c.estado = 'VIGENTE'
                       AND pa.fecha_pago IS NULL
                       AND pa.fecha_vencimiento < CURDATE()
                       ORDER BY dias_vencido DESC
                       LIMIT 10";
            using var command = new MySqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                pagos.Add(new PagosPendientes
                {
                    Id = reader.GetInt32("id"),
                    Inquilino = reader.GetString("inquilino"),
                    Inmueble = reader.GetString("inmueble"),
                    Mes = reader.GetInt32("mes"),
                    Año = reader.GetInt32("año"),
                    Monto = reader.GetDecimal("monto"),
                    DiasVencido = reader.GetInt32("dias_vencido")
                });
            }
            return pagos;
        }

        public List<AccesoDirecto> ObtenerAccesosDirectos()
        {
            return new List<AccesoDirecto>
            {
                new AccesoDirecto
                {
                    Titulo = "Nuevo Contrato",
                    Descripcion = "Crear un nuevo contrato de alquiler",
                    Icono = "fas fa-file-contract",
                    Color = "success",
                    Controlador = "Contratos",
                    Accion = "Index"
                },
                new AccesoDirecto
                {
                    Titulo = "Registrar Pago",
                    Descripcion = "Registrar un nuevo pago",
                    Icono = "fas fa-money-bill-wave",
                    Color = "info",
                    Controlador = "Pagos",
                    Accion = "Index"
                },
                new AccesoDirecto
                {
                    Titulo = "Agregar Inmueble",
                    Descripcion = "Registrar un nuevo inmueble",
                    Icono = "fas fa-home",
                    Color = "primary",
                    Controlador = "Inmueble",
                    Accion = "Create"
                },
                new AccesoDirecto
                {
                    Titulo = "Nuevo Inquilino",
                    Descripcion = "Registrar un nuevo inquilino",
                    Icono = "fas fa-user-plus",
                    Color = "warning",
                    Controlador = "Inquilinos",
                    Accion = "Agregar"
                },
                new AccesoDirecto
                {
                    Titulo = "Nuevo Propietario",
                    Descripcion = "Registrar un nuevo propietario",
                    Icono = "fas fa-user-tie",
                    Color = "secondary",
                    Controlador = "Propietarios",
                    Accion = "Create"
                },
                new AccesoDirecto
                {
                    Titulo = "Reportes",
                    Descripcion = "Ver reportes y estadísticas",
                    Icono = "fas fa-chart-bar",
                    Color = "dark",
                    Controlador = "Home",
                    Accion = "Reportes"
                }
            };
        }
    }
}
