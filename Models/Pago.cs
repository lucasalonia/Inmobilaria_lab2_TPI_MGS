using System;

namespace Inmobilaria_lab2_TPI_MGS.Models
{
    public class Pago
    {
        public ulong Id { get; set; }

        public int ContratoId { get; set; }

        public string Estado { get; set; } = "PENDIENTE";

        public short PeriodoAnio { get; set; }

        public byte PeriodoMes { get; set; }

        public DateTime FechaVencimiento { get; set; }

        public decimal Importe { get; set; } = 0.00m;

        public decimal Recargo { get; set; } = 0.00m;

        public decimal Descuento { get; set; } = 0.00m;

        public decimal? ImportePagado { get; set; }

        public DateTime? FechaPago { get; set; }

        public string? MedioPago { get; set; }

        public string? Observaciones { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        public ulong? CreadoPor { get; set; }
        public Usuario? Creador { get; set; }

        public ulong? ModificadoPor { get; set; }
        public Usuario? Modificador { get; set; }
    }
}