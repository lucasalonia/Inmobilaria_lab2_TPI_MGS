using Inmobilaria_lab2_TPI_MGS.Models;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public interface ContratoService
    {
        bool Alta(Contrato contrato, int? idUsuario);
        bool Modificar(Contrato contrato, int? idUsuario);
        bool Baja(int id, int? idUsuario);
        Contrato ObtenerContratoVigente(int inquilinoId);
        IList<Inmueble> ListarInmueblesDisponibles(int paginaNro = 1, int tamPagina = 10);
        public IList<Contrato> ListaContratosVigentes(int paginaNro = 1, int tamPagina = 10);

        public Contrato ObtenerPorId(int contratoId);
         public Contrato? ObtenerContratoVigentePorInmuebleId(int inmuebleId);
        IList<Contrato> ObtenerPorInmuebleId(int inmuebleId);
        IList<Contrato> ObtenerTodos(int? inquilinoId = null);
        
    }
}