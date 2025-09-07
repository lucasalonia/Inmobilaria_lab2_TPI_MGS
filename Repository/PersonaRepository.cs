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

        public int CrearPersona(Persona persona)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"INSERT INTO persona (dni, sexo, nombre, apellido, fecha_nacimiento, email, telefono, fecha_creacion, fecha_modificacion) 
                                  VALUES (@Dni, @Sexo, @Nombre, @Apellido, @FechaNacimiento, @Email, @Telefono, @FechaCreacion, @FechaModificacion);
                                  SELECT LAST_INSERT_ID();";
                    
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Dni", persona.Dni);
                        command.Parameters.AddWithValue("@Sexo", persona.Sexo);
                        command.Parameters.AddWithValue("@Nombre", persona.Nombre);
                        command.Parameters.AddWithValue("@Apellido", persona.Apellido);
                        command.Parameters.AddWithValue("@FechaNacimiento", persona.FechaNacimiento);
                        command.Parameters.AddWithValue("@Email", persona.Email);
                        command.Parameters.AddWithValue("@Telefono", persona.Telefono ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FechaCreacion", persona.FechaCreacion);
                        command.Parameters.AddWithValue("@FechaModificacion", persona.FechaModificacion);
                        
                        var result = command.ExecuteScalar();
                        return Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en PersonaRepository.CrearPersona: {ex.Message}");
                throw;
            }
        }

        public bool ActualizarPersona(Persona persona)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"UPDATE persona SET 
                                  dni = @Dni, 
                                  sexo = @Sexo, 
                                  nombre = @Nombre, 
                                  apellido = @Apellido, 
                                  fecha_nacimiento = @FechaNacimiento, 
                                  email = @Email, 
                                  telefono = @Telefono, 
                                  fecha_modificacion = @FechaModificacion
                                  WHERE id = @Id";
                    
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", persona.Id);
                        command.Parameters.AddWithValue("@Dni", persona.Dni);
                        command.Parameters.AddWithValue("@Sexo", persona.Sexo);
                        command.Parameters.AddWithValue("@Nombre", persona.Nombre);
                        command.Parameters.AddWithValue("@Apellido", persona.Apellido);
                        command.Parameters.AddWithValue("@FechaNacimiento", persona.FechaNacimiento);
                        command.Parameters.AddWithValue("@Email", persona.Email);
                        command.Parameters.AddWithValue("@Telefono", persona.Telefono ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FechaModificacion", persona.FechaModificacion);
                        
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en PersonaRepository.ActualizarPersona: {ex.Message}");
                throw;
            }
        }
    }
}