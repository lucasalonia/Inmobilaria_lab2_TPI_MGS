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
                    u.creado_por, u.modificado_por,
                    per.dni, per.sexo, per.nombre, per.apellido, per.fecha_nacimiento,
                    per.email, per.telefono, per.fecha_creacion AS persona_fecha_creacion,
                    per.fecha_modificacion AS persona_fecha_modificacion
                FROM usuario u
                INNER JOIN persona per ON u.persona_id = per.id
                ORDER BY u.fecha_creacion DESC";
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
    }
}