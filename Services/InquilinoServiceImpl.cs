using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Repository;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public class InquilinoServiceImpl : InquilinoService
    {
        private readonly InquilinoRepository inquilinoRepository;

        public InquilinoServiceImpl(InquilinoRepository inquilinoRepository)
        {
            this.inquilinoRepository = inquilinoRepository;
        }

        public bool Alta(Persona persona)
        {
            return inquilinoRepository.Alta(persona);
        }

        public bool Baja(int idPersona)
        {
            return inquilinoRepository.Baja(idPersona);
        }

        public bool Modificar(Persona persona)
        {
            return inquilinoRepository.Modificar(persona);
        }

        public IList<Inquilino> ObtenerTodos()
        {
            return inquilinoRepository.ObtenerTodos();
        }
        public IList<Inquilino> ListarInquilinosSinContrato()
        {
            return inquilinoRepository.ListarInquilinosSinContrato();
        }
    }
}