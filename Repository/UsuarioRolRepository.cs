using MySql.Data.MySqlClient;
using Inmobilaria_lab2_TPI_MGS.Models;

namespace Inmobilaria_lab2_TPI_MGS.Repository
{
    public class UsuarioRolRepository : BaseRepository
    {
        public IList<UsuarioRol> ObtenerPorUsuarioId(int usuarioId)
        {
            var usuarioRoles = new List<UsuarioRol>();
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"SELECT ur.id, ur.usuario_id, ur.rol_id, ur.estado, ur.fecha_creacion, 
                                  ur.fecha_modificacion, ur.creado_por, ur.modificado_por,
                                  r.nombre as rol_nombre, r.codigo as rol_codigo
                                  FROM usuario_rol ur
                                  INNER JOIN rol r ON ur.rol_id = r.id
                                  WHERE ur.usuario_id = @UsuarioId AND ur.estado = 'ACTIVO'";
                    
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UsuarioId", usuarioId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                usuarioRoles.Add(new UsuarioRol
                                {
                                    Id = reader.GetInt32("id"),
                                    Usuario = new Usuario { Id = reader.GetInt32("usuario_id") },
                                    Rol = new Rol
                                    {
                                        Id = reader.GetInt32("rol_id"),
                                        Nombre = reader.GetString("rol_nombre"),
                                        Codigo = reader.GetString("rol_codigo")
                                    },
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
                Console.WriteLine($"Error en UsuarioRolRepository.ObtenerPorUsuarioId: {ex.Message}");
                throw;
            }
            return usuarioRoles;
        }

        public IList<UsuarioRol> ObtenerTodosPorUsuarioId(int usuarioId)
        {
            var usuarioRoles = new List<UsuarioRol>();
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"SELECT ur.id, ur.usuario_id, ur.rol_id, ur.estado, ur.fecha_creacion, 
                                  ur.fecha_modificacion, ur.creado_por, ur.modificado_por,
                                  r.nombre as rol_nombre, r.codigo as rol_codigo
                                  FROM usuario_rol ur
                                  INNER JOIN rol r ON ur.rol_id = r.id
                                  WHERE ur.usuario_id = @UsuarioId
                                  ORDER BY ur.fecha_creacion DESC";
                    
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UsuarioId", usuarioId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                usuarioRoles.Add(new UsuarioRol
                                {
                                    Id = reader.GetInt32("id"),
                                    Usuario = new Usuario { Id = reader.GetInt32("usuario_id") },
                                    Rol = new Rol
                                    {
                                        Id = reader.GetInt32("rol_id"),
                                        Nombre = reader.GetString("rol_nombre"),
                                        Codigo = reader.GetString("rol_codigo")
                                    },
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
                Console.WriteLine($"Error en UsuarioRolRepository.ObtenerTodosPorUsuarioId: {ex.Message}");
                throw;
            }
            return usuarioRoles;
        }

        public UsuarioRol? ObtenerActivoPorUsuarioId(int usuarioId)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"SELECT ur.id, ur.usuario_id, ur.rol_id, ur.estado, ur.fecha_creacion, 
                                  ur.fecha_modificacion, ur.creado_por, ur.modificado_por,
                                  r.nombre as rol_nombre, r.codigo as rol_codigo
                                  FROM usuario_rol ur
                                  INNER JOIN rol r ON ur.rol_id = r.id
                                  WHERE ur.usuario_id = @UsuarioId AND ur.estado = 'ACTIVO'
                                  LIMIT 1";
                    
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UsuarioId", usuarioId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new UsuarioRol
                                {
                                    Id = reader.GetInt32("id"),
                                    Usuario = new Usuario { Id = reader.GetInt32("usuario_id") },
                                    Rol = new Rol
                                    {
                                        Id = reader.GetInt32("rol_id"),
                                        Nombre = reader.GetString("rol_nombre"),
                                        Codigo = reader.GetString("rol_codigo")
                                    },
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
                Console.WriteLine($"Error en UsuarioRolRepository.ObtenerActivoPorUsuarioId: {ex.Message}");
                throw;
            }
            return null;
        }

        public int CrearUsuarioRol(UsuarioRol usuarioRol)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"INSERT INTO usuario_rol (usuario_id, rol_id, estado, fecha_creacion, fecha_modificacion, creado_por)
                                  VALUES (@UsuarioId, @RolId, @Estado, @FechaCreacion, @FechaModificacion, @CreadoPor);
                                  SELECT LAST_INSERT_ID();";
                    
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UsuarioId", usuarioRol.Usuario.Id);
                        command.Parameters.AddWithValue("@RolId", usuarioRol.Rol.Id);
                        command.Parameters.AddWithValue("@Estado", usuarioRol.Estado);
                        command.Parameters.AddWithValue("@FechaCreacion", usuarioRol.FechaCreacion);
                        command.Parameters.AddWithValue("@FechaModificacion", usuarioRol.FechaModificacion);
                        command.Parameters.AddWithValue("@CreadoPor", usuarioRol.CreadoPor?.Id ?? (object)DBNull.Value);
                        
                        var result = command.ExecuteScalar();
                        return Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UsuarioRolRepository.CrearUsuarioRol: {ex.Message}");
                throw;
            }
        }

        public bool ActualizarUsuarioRol(UsuarioRol usuarioRol)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"UPDATE usuario_rol SET 
                                  rol_id = @RolId, 
                                  estado = @Estado, 
                                  fecha_modificacion = @FechaModificacion,
                                  modificado_por = @ModificadoPor
                                  WHERE id = @Id";
                    
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", usuarioRol.Id);
                        command.Parameters.AddWithValue("@RolId", usuarioRol.Rol.Id);
                        command.Parameters.AddWithValue("@Estado", usuarioRol.Estado);
                        command.Parameters.AddWithValue("@FechaModificacion", usuarioRol.FechaModificacion);
                        command.Parameters.AddWithValue("@ModificadoPor", usuarioRol.ModificadoPor?.Id ?? (object)DBNull.Value);
                        
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UsuarioRolRepository.ActualizarUsuarioRol: {ex.Message}");
                throw;
            }
        }

        public bool DeshabilitarRolesUsuario(int usuarioId)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"UPDATE usuario_rol SET 
                                  estado = 'INACTIVO', 
                                  fecha_modificacion = @FechaModificacion
                                  WHERE usuario_id = @UsuarioId AND estado = 'ACTIVO'";
                    
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UsuarioId", usuarioId);
                        command.Parameters.AddWithValue("@FechaModificacion", DateTime.Now);
                        
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected >= 0; // Puede ser 0 si no había roles activos
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UsuarioRolRepository.DeshabilitarRolesUsuario: {ex.Message}");
                throw;
            }
        }

        public bool ReactivarRolUsuario(int usuarioId, int rolId, int? modificadoPor = null)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"UPDATE usuario_rol SET 
                                  estado = 'ACTIVO', 
                                  fecha_modificacion = @FechaModificacion,
                                  modificado_por = @ModificadoPor
                                  WHERE usuario_id = @UsuarioId AND rol_id = @RolId";
                    
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UsuarioId", usuarioId);
                        command.Parameters.AddWithValue("@RolId", rolId);
                        command.Parameters.AddWithValue("@FechaModificacion", DateTime.Now);
                        command.Parameters.AddWithValue("@ModificadoPor", modificadoPor ?? (object)DBNull.Value);
                        
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UsuarioRolRepository.ReactivarRolUsuario: {ex.Message}");
                throw;
            }
        }
    }
}
