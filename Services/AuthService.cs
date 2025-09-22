using Inmobilaria_lab2_TPI_MGS.Models;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public interface AuthService
    {
        Task<Usuario?> AutenticarUsuarioAsync(string username, string password);
        Task<bool> ActualizarUltimoLoginAsync(int userId);
        Task<bool> VerificarContraseñaAsync(string password, string hashedPassword);
        string HashearContraseña(string password);
    }
}
