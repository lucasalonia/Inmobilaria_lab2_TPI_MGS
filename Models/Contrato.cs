using System;
using System.ComponentModel.DataAnnotations;

namespace Inmobilaria_lab2_TPI_MGS.Models
{
    public class Contrato
    {
        public int Id { get; set; }                 
        public int InmuebleId { get; set; }        
        public int InquilinoId { get; set; }        

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        [Display(Name = "Estado del contrato")]
        public string Estado { get; set; } = "VIGENTE"; 

        public decimal? MontoMensual { get; set; }     
        public string Moneda { get; set; }             
        public decimal? Deposito { get; set; }        
        public string Observaciones { get; set; }    

        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }

        public ulong? CreadoPor { get; set; }         
        public ulong? ModificadoPor { get; set; }  

        public int? TerminadoPor { get; set; }
    }
}