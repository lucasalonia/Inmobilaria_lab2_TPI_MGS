using Inmobilaria_lab2_TPI_MGS.Models;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public interface InquilinoService
    {
        IList<Persona> ObtenerTodos();

        bool Modificar(Persona persona);

        bool Baja(int idPersona);

        bool Alta(Persona persona);
    }
}