using Inmobilaria_lab2_TPI_MGS.Models;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public interface PagoService
    {
        bool Alta(Pago pago, int? idUsuario);
        bool Modificar(Pago pago, int? idUsuario);
        bool Baja(int id, int? idUsuario);
        Pago ObtenerPorId(ulong id);
        IList<Pago> ListarPorContrato(int contratoId);
        bool ActualizarEstado(ulong id, string nuevoEstado);

    }
}