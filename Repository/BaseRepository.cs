
using Microsoft.Extensions.Configuration;

namespace Inmobilaria_lab2_TPI_MGS.Repository
{
    public abstract class BaseRepository
    {
        protected readonly IConfiguration configuration;
        protected readonly string connectionString;


        // Temporalmente usamos conexión local a la base de datos 
        //true para local, false para remota - LS
        protected BaseRepository(bool usarLocal = true)
        {

            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            //Esto sera utilizado por los repositorios que hereden de esta clase
            //Se utiliza en "(MySqlConnection connection = new MySqlConnection(connectionString))" la cual establece la conexion con la base de datos - LS
            
            connectionString = usarLocal ? configuration["ConnectionStrings:MySqlLocal"] : configuration["ConnectionStrings:MySqlRemote"];
        }
    }
}