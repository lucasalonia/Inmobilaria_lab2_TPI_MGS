using Inmobilaria_lab2_TPI_MGS.Models;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public interface PersonService
    {
        Persona ObtenerPorDni(string dni);
    }
}