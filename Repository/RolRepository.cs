using MySql.Data.MySqlClient;
using Inmobilaria_lab2_TPI_MGS.Models;

namespace Inmobilaria_lab2_TPI_MGS.Repository
{
    public class RolRepository : BaseRepository
    {
        public IList<Rol> ObtenerTodos()
        {
            var roles = new List<Rol>();
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"SELECT id, nombre, codigo, estado, fecha_creacion, fecha_modificacion, 
                                  creado_por, modificado_por
                                  FROM rol 
                                  WHERE estado = 'ACTIVO'
                                  ORDER BY nombre";
                    
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                roles.Add(new Rol
                                {
                                    Id = reader.GetInt32("id"),
                                    Nombre = reader.GetString("nombre"),
                                    Codigo = reader.GetString("codigo"),
                                    Estado = reader.GetString("estado"),
                                    FechaCreacion = reader.GetDateTime("fecha_creacion"),
                                    FechaModificacion = reader.GetDateTime("fecha_modificacion"),
                                    CreadoPor = reader.IsDBNull(reader.GetOrdinal("creado_por")) ? null : new Usuario { Id = reader.GetInt32("creado_por") },
                                    ModificadoPor = reader.IsDBNull(reader.GetOrdinal("modificado_por")) ? null : new Usuario { Id = reader.GetInt32("modificado_por") }
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en RolRepository.ObtenerTodos: {ex.Message}");
                throw;
            }
            return roles;
        }

        public Rol? ObtenerPorId(int id)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"SELECT id, nombre, codigo, estado, fecha_creacion, fecha_modificacion, 
                                  creado_por, modificado_por
                                  FROM rol 
                                  WHERE id = @Id";
                    
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Rol
                                {
                                    Id = reader.GetInt32("id"),
                                    Nombre = reader.GetString("nombre"),
                                    Codigo = reader.GetString("codigo"),
                                    Estado = reader.GetString("estado"),
                                    FechaCreacion = reader.GetDateTime("fecha_creacion"),
                                    FechaModificacion = reader.GetDateTime("fecha_modificacion"),
                                    CreadoPor = reader.IsDBNull(reader.GetOrdinal("creado_por")) ? null : new Usuario { Id = reader.GetInt32("creado_por") },
                                    ModificadoPor = reader.IsDBNull(reader.GetOrdinal("modificado_por")) ? null : new Usuario { Id = reader.GetInt32("modificado_por") }
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en RolRepository.ObtenerPorId: {ex.Message}");
                throw;
            }
            return null;
        }

        public Rol? ObtenerPorCodigo(string codigo)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"SELECT id, nombre, codigo, estado, fecha_creacion, fecha_modificacion, 
                                  creado_por, modificado_por
                                  FROM rol 
                                  WHERE codigo = @Codigo AND estado = 'ACTIVO'";
                    
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Codigo", codigo);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Rol
                                {
                                    Id = reader.GetInt32("id"),
                                    Nombre = reader.GetString("nombre"),
                                    Codigo = reader.GetString("codigo"),
                                    Estado = reader.GetString("estado"),
                                    FechaCreacion = reader.GetDateTime("fecha_creacion"),
                                    FechaModificacion = reader.GetDateTime("fecha_modificacion"),
                                    CreadoPor = reader.IsDBNull(reader.GetOrdinal("creado_por")) ? null : new Usuario { Id = reader.GetInt32("creado_por") },
                                    ModificadoPor = reader.IsDBNull(reader.GetOrdinal("modificado_por")) ? null : new Usuario { Id = reader.GetInt32("modificado_por") }
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en RolRepository.ObtenerPorCodigo: {ex.Message}");
                throw;
            }
            return null;
        }
    }
}
