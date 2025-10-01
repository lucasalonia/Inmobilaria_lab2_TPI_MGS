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

        public bool Alta(Persona persona, int? idUsuario)
        {
            return inquilinoRepository.Alta(persona, idUsuario );
        }

        public bool Baja(int idPersona, int? idUsuario)
        {
            return inquilinoRepository.Baja(idPersona, idUsuario);
        }

        public bool Modificar(Persona persona, int? idUsuario)
        {
            return inquilinoRepository.Modificar(persona, idUsuario);
        }

        public IList<Inquilino> ObtenerTodos(int paginaNro = 1, int tamPagina = 10)
        {
            return inquilinoRepository.ObtenerTodos(paginaNro, tamPagina);
        }
        public IList<Inquilino> ListarInquilinosSinContrato( int pagina = 1, int tamPagina = 10)
        {
            return inquilinoRepository.ListarInquilinosSinContrato(pagina, tamPagina);
        }
        public int ContarInquilinosActivos()
        {
            return inquilinoRepository.ContarInquilinosActivos();
        }
        public int ContarInquilinosActivosSinContrato()
        {
            return inquilinoRepository.ContarInquilinosActivosSinContrato();
        }
        public Persona? ObtenerPorDni(string dni)
        {
            return inquilinoRepository.ObtenerPorDni(dni);
        }
    }
}