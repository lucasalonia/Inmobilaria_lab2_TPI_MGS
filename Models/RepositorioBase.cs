
namespace Inmobilaria_lab2_TPI_MGS.Models
{
    public abstract class RepositorioBase
    {
        protected readonly IConfiguration configuration;
        protected readonly string connectionString;

        protected RepositorioBase()
        {
           
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) 
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            connectionString = configuration["ConnectionStrings:MySql"];
        }
    }
}