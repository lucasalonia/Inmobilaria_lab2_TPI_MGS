using Inmobilaria_lab2_TPI_MGS.Models;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public interface PropietarioService
    {
        IList<Propietario> ObtenerTodos();

        IList<Propietario> ObtenerTodos(int paginaNro = 1, int tamPagina = 10);

        int ContarPropietariosActivos();

        int Modificar(Propietario propietario);

        int Baja(int idPropietario);

        int BajaLogica(int id, int? modificadoPor = null);

        Propietario ObtenerPorId(int propietarioId);

        Persona? ObtenerPorDni(string dni);

        int Alta(Propietario propietario);
    }
}