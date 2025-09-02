using Inmobilaria_lab2_TPI_MGS.Models;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public interface ContratoService
    {
        bool Alta(Contrato contrato);
        Contrato ObtenerContratoVigente(int inquilinoId);
        IList<Inmueble> ListarInmueblesDisponibles(int paginaNro = 1, int tamPagina = 10);
    }
}