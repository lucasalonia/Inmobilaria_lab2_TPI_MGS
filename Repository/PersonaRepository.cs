using MySql.Data.MySqlClient;
using Inmobilaria_lab2_TPI_MGS.Models;

namespace Inmobilaria_lab2_TPI_MGS.Repository
{
    public class PersonaRepository : BaseRepository
    {
        public PersonaRepository() : base()
        {
        }

        public Persona? ObtenerPorDni(string dni)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"SELECT * FROM persona WHERE dni = @Dni";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Dni", dni);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Persona
                                {
                                    Id = reader.GetInt32("id"),
                                    Dni = reader.GetString("dni"),
                                    Sexo = reader.GetString("sexo"),
                                    Nombre = reader.GetString("nombre"),
                                    Apellido = reader.GetString("apellido"),
                                    FechaNacimiento = reader.GetDateTime("fecha_nacimiento"),
                                    Email = reader.GetString("email"),
                                    Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString("telefono"),
                                    FechaCreacion = reader.GetDateTime("fecha_creacion"),
                                    FechaModificacion = reader.GetDateTime("fecha_modificacion")
                                };
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en PersonaRepository.ObtenerPorDni: {ex.Message}");
                throw;
            }
        }
    }
}