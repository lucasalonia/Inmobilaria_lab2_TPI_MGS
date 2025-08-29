using Inmobilaria_lab2_TPI_MGS.Models;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public interface InquilinoService
    {
        IList<Inquilino> ObtenerTodos();
        IList<Persona> ListarInquilinosSinContrato();

        bool Modificar(Persona persona);

        bool Baja(int idPersona);

        bool Alta(Persona persona);
    }
}