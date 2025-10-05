using Inmobilaria_lab2_TPI_MGS.Models;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public interface InquilinoService
    {
        IList<Inquilino> ObtenerTodos(int paginaNro = 1, int tamPagina = 10);
        IList<Inquilino> ListarInquilinosSinContrato(int pagina = 1, int tamPagina = 10);

        bool Modificar(Persona persona, int? idUsuario);

        bool Baja(int idPersona, int? idUsuario);

        bool Alta(Persona persona, int? idUsuario);
        int ContarInquilinosActivos();
        Persona? ObtenerPorDni(string dni);

        public int ContarInquilinosActivosSinContrato();

        public Inquilino ObtenerInquilinoPorContrato(int contratoId);
        public Inquilino ObtenerPorId(int id);
    }
}