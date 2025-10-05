using System.Data;
using Inmobilaria_lab2_TPI_MGS.Models;
using MySql.Data.MySqlClient;

namespace Inmobilaria_lab2_TPI_MGS.Repository
{
    public class UsuarioRepository : BaseRepository
    {
        public UsuarioRepository() : base()
        {

        }
        
        public IList<Usuario> ObtenerTodos()
        {
            IList<Usuario> res = new List<Usuario>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT u.id, u.persona_id, u.username, u.password, u.estado, 
                    u.fecha_creacion, u.fecha_modificacion, u.ultimo_login,
                    u.creado_por, u.modificado_por, u.foto_perfil,
                    per.dni, per.sexo, per.nombre, per.apellido, per.fecha_nacimiento,
                    per.email, per.telefono, per.fecha_creacion AS persona_fecha_creacion,
                    per.fecha_modificacion AS persona_fecha_modificacion
                FROM usuario u
                INNER JOIN persona per ON u.persona_id = per.id
                ORDER BY u.id ASC";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Usuario usuario = new Usuario
                            {
                                Id = reader.GetInt32("id"),
                                UserName = reader.GetString("username"),
                                Password = reader.GetString("password"),
                                Estado = reader.GetString("estado"),
                                FechaCreacion = reader.GetDateTime("fecha_creacion"),
                                FechaModificacion = reader.GetDateTime("fecha_modificacion"),
                                UltimoLogin = reader.IsDBNull(reader.GetOrdinal("ultimo_login")) ? (DateTime?)null : reader.GetDateTime("ultimo_login"),
                                CreadoPor = reader.IsDBNull(reader.GetOrdinal("creado_por")) ? null : new Usuario { Id = reader.GetInt32("creado_por") },
                                ModificadoPor = reader.IsDBNull(reader.GetOrdinal("modificado_por")) ? null : new Usuario { Id = reader.GetInt32("modificado_por") },
                                FotoPerfil = reader.IsDBNull(reader.GetOrdinal("foto_perfil")) ? null : reader.GetString("foto_perfil"),

                                Persona = new Persona
                                {
                                    Id = reader.GetInt32("persona_id"),
                                    Dni = reader.GetString("dni"),
                                    Sexo = reader.GetString("sexo"),
                                    Nombre = reader.GetString("nombre"),
                                    Apellido = reader.GetString("apellido"),
                                    FechaNacimiento = reader.IsDBNull(reader.GetOrdinal("fecha_nacimiento")) ? (DateTime?)null : reader.GetDateTime("fecha_nacimiento"),
                                    Email = reader.GetString("email"),
                                    Telefono = reader.GetString("telefono"),
                                    FechaCreacion = reader.GetDateTime("persona_fecha_creacion"),
                                    FechaModificacion = reader.GetDateTime("persona_fecha_modificacion")
                                }
                            };
                            res.Add(usuario);
                        }
                        connection.Close();
                    }
                }
            }
            return res;
        }

        public int CrearUsuario(Usuario usuario)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"INSERT INTO usuario (persona_id, username, password, estado, fecha_creacion, fecha_modificacion, ultimo_login, creado_por, modificado_por, foto_perfil) 
                                  VALUES (@PersonaId, @UserName, @Password, @Estado, @FechaCreacion, @FechaModificacion, @UltimoLogin, @CreadoPor, @ModificadoPor, @FotoPerfil);
                                  SELECT LAST_INSERT_ID();";
                    
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@PersonaId", usuario.Persona.Id);
                        command.Parameters.AddWithValue("@UserName", usuario.UserName);
                        command.Parameters.AddWithValue("@Password", usuario.Password);
                        command.Parameters.AddWithValue("@Estado", usuario.Estado);
                        command.Parameters.AddWithValue("@FechaCreacion", usuario.FechaCreacion);
                        command.Parameters.AddWithValue("@FechaModificacion", usuario.FechaModificacion);
                        command.Parameters.AddWithValue("@UltimoLogin", usuario.UltimoLogin ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CreadoPor", usuario.CreadoPor?.Id ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ModificadoPor", usuario.ModificadoPor?.Id ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FotoPerfil", (object?)usuario.FotoPerfil ?? DBNull.Value);
                        
                        var result = command.ExecuteScalar();
                        return Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UsuarioRepository.CrearUsuario: {ex.Message}");
                throw;
            }
        }

        public Usuario? ObtenerPorId(int id)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"SELECT u.id, u.persona_id, u.username, u.password, u.estado, 
                        u.fecha_creacion, u.fecha_modificacion, u.ultimo_login,
                        u.creado_por, u.modificado_por, u.foto_perfil,
                        per.dni, per.sexo, per.nombre, per.apellido, per.fecha_nacimiento,
                        per.email, per.telefono, per.fecha_creacion AS persona_fecha_creacion,
                        per.fecha_modificacion AS persona_fecha_modificacion
                    FROM usuario u
                    INNER JOIN persona per ON u.persona_id = per.id
                    WHERE u.id = @Id";
                    
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Usuario
                                {
                                    Id = reader.GetInt32("id"),
                                    UserName = reader.GetString("username"),
                                    Password = reader.GetString("password"),
                                    Estado = reader.GetString("estado"),
                                    FechaCreacion = reader.GetDateTime("fecha_creacion"),
                                    FechaModificacion = reader.GetDateTime("fecha_modificacion"),
                                    UltimoLogin = reader.IsDBNull(reader.GetOrdinal("ultimo_login")) ? (DateTime?)null : reader.GetDateTime("ultimo_login"),
                                    CreadoPor = reader.IsDBNull(reader.GetOrdinal("creado_por")) ? null : new Usuario { Id = reader.GetInt32("creado_por") },
                                    ModificadoPor = reader.IsDBNull(reader.GetOrdinal("modificado_por")) ? null : new Usuario { Id = reader.GetInt32("modificado_por") },
                                    FotoPerfil = reader.IsDBNull(reader.GetOrdinal("foto_perfil")) ? null : reader.GetString("foto_perfil"),

                                    Persona = new Persona
                                    {
                                        Id = reader.GetInt32("persona_id"),
                                        Dni = reader.GetString("dni"),
                                        Sexo = reader.GetString("sexo"),
                                        Nombre = reader.GetString("nombre"),
                                        Apellido = reader.GetString("apellido"),
                                        FechaNacimiento = reader.IsDBNull(reader.GetOrdinal("fecha_nacimiento")) ? (DateTime?)null : reader.GetDateTime("fecha_nacimiento"),
                                        Email = reader.GetString("email"),
                                        Telefono = reader.GetString("telefono"),
                                        FechaCreacion = reader.GetDateTime("persona_fecha_creacion"),
                                        FechaModificacion = reader.GetDateTime("persona_fecha_modificacion")
                                    }
                                };
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UsuarioRepository.ObtenerPorId: {ex.Message}");
                throw;
            }
        }

        public bool ActualizarUsuario(Usuario usuario)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"UPDATE usuario SET 
                                  username = @UserName, 
                                  password = @Password, 
                                  estado = @Estado, 
                                  fecha_modificacion = @FechaModificacion,
                                  ultimo_login = @UltimoLogin,
                                  modificado_por = @ModificadoPor,
                                  foto_perfil = @FotoPerfil
                                  WHERE id = @Id";
                    
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", usuario.Id);
                        command.Parameters.AddWithValue("@UserName", usuario.UserName);
                        command.Parameters.AddWithValue("@Password", usuario.Password);
                        command.Parameters.AddWithValue("@Estado", usuario.Estado);
                        command.Parameters.AddWithValue("@FechaModificacion", usuario.FechaModificacion);
                        command.Parameters.AddWithValue("@UltimoLogin", usuario.UltimoLogin ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ModificadoPor", usuario.ModificadoPor?.Id ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FotoPerfil", (object?)usuario.FotoPerfil ?? DBNull.Value);
                        
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UsuarioRepository.ActualizarUsuario: {ex.Message}");
                throw;
            }
        }

        public bool DeshabilitarUsuario(int id)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"UPDATE usuario SET 
                                  estado = 'INACTIVO', 
                                  fecha_modificacion = @FechaModificacion
                                  WHERE id = @Id";
                    
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@FechaModificacion", DateTime.Now);
                        
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UsuarioRepository.DeshabilitarUsuario: {ex.Message}");
                throw;
            }
        }

        public Usuario? ObtenerPorUsername(string username)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"SELECT u.id, u.persona_id, u.username, u.password, u.estado, 
                        u.fecha_creacion, u.fecha_modificacion, u.ultimo_login,
                        u.creado_por, u.modificado_por, u.foto_perfil,
                        per.dni, per.sexo, per.nombre, per.apellido, per.fecha_nacimiento,
                        per.email, per.telefono, per.fecha_creacion AS persona_fecha_creacion,
                        per.fecha_modificacion AS persona_fecha_modificacion
                    FROM usuario u
                    INNER JOIN persona per ON u.persona_id = per.id
                    WHERE u.username = @Username";
                    
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Usuario
                                {
                                    Id = reader.GetInt32("id"),
                                    UserName = reader.GetString("username"),
                                    Password = reader.GetString("password"),
                                    Estado = reader.GetString("estado"),
                                    FechaCreacion = reader.GetDateTime("fecha_creacion"),
                                    FechaModificacion = reader.GetDateTime("fecha_modificacion"),
                                    UltimoLogin = reader.IsDBNull(reader.GetOrdinal("ultimo_login")) ? (DateTime?)null : reader.GetDateTime("ultimo_login"),
                                    CreadoPor = reader.IsDBNull(reader.GetOrdinal("creado_por")) ? null : new Usuario { Id = reader.GetInt32("creado_por") },
                                ModificadoPor = reader.IsDBNull(reader.GetOrdinal("modificado_por")) ? null : new Usuario { Id = reader.GetInt32("modificado_por") },
                                FotoPerfil = reader.IsDBNull(reader.GetOrdinal("foto_perfil")) ? null : reader.GetString("foto_perfil"),

                                    Persona = new Persona
                                    {
                                        Id = reader.GetInt32("persona_id"),
                                        Dni = reader.GetString("dni"),
                                        Sexo = reader.GetString("sexo"),
                                        Nombre = reader.GetString("nombre"),
                                        Apellido = reader.GetString("apellido"),
                                        FechaNacimiento = reader.IsDBNull(reader.GetOrdinal("fecha_nacimiento")) ? (DateTime?)null : reader.GetDateTime("fecha_nacimiento"),
                                        Email = reader.GetString("email"),
                                        Telefono = reader.GetString("telefono"),
                                        FechaCreacion = reader.GetDateTime("persona_fecha_creacion"),
                                        FechaModificacion = reader.GetDateTime("persona_fecha_modificacion")
                                    }
                                };
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UsuarioRepository.ObtenerPorUsername: {ex.Message}");
                throw;
            }
        }
    }
}