using System.Data;
using Inmobilaria_lab2_TPI_MGS.Models;
using MySql.Data.MySqlClient;

namespace Inmobilaria_lab2_TPI_MGS.Repository
{
    public class PropietarioRepository : BaseRepository
    {
        public PropietarioRepository() : base()
        {

        }

        public IList<Propietario> ObtenerTodos()
        {
            IList<Propietario> res = new List<Propietario>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT p.id, p.persona_id, p.estado, p.fecha_creacion, p.fecha_modificacion, 
                    p.creado_por, p.modificado_por,
                    per.dni, per.sexo, per.nombre, per.apellido, per.fecha_nacimiento,
                    per.email, per.telefono, per.fecha_creacion AS persona_fecha_creacion,
                    per.fecha_modificacion AS persona_fecha_modificacion
                FROM propietario p
                INNER JOIN persona per ON p.persona_id = per.id
                WHERE p.estado = 'ACTIVO'";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Propietario pr = new Propietario
                            {
                                Id = reader.GetInt32("id"),
                                Estado = reader.GetString("estado"),
                                FechaCreacion = reader.GetDateTime("fecha_creacion"),
                                FechaModificacion = reader.GetDateTime("fecha_modificacion"),
                                CreadoPor = reader.IsDBNull(reader.GetOrdinal("creado_por")) ? (int?)null : reader.GetInt32("creado_por"),
                                ModificadoPor = reader.IsDBNull(reader.GetOrdinal("modificado_por")) ? (int?)null : reader.GetInt32("modificado_por"),



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
                                    FechaCreacion = reader.GetDateTime("fecha_creacion"),
                                    FechaModificacion = reader.GetDateTime("fecha_modificacion")
                                }
                            };
                            res.Add(pr);
                        }
                        connection.Close();
                    }
                }
            }
            return res;
        }

        public int Alta(Propietario p)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // 1. Insertar en persona
                string sqlPersona = @"
                    INSERT INTO persona 
                    (dni, sexo, nombre, apellido, fecha_nacimiento, email, telefono, 
                    fecha_creacion, fecha_modificacion)
                    VALUES (@dni, @sexo, @nombre, @apellido, @fecha_nacimiento, @email, @telefono, NOW(), NOW());
                    SELECT LAST_INSERT_ID();";

                int personaId;
                using (var command = new MySqlCommand(sqlPersona, connection))
                {
                    command.Parameters.AddWithValue("@dni", p.Persona.Dni);
                    command.Parameters.AddWithValue("@sexo", p.Persona.Sexo);
                    command.Parameters.AddWithValue("@nombre", p.Persona.Nombre);
                    command.Parameters.AddWithValue("@apellido", p.Persona.Apellido);
                    command.Parameters.AddWithValue("@fecha_nacimiento", (object?)p.Persona.FechaNacimiento ?? DBNull.Value);
                    command.Parameters.AddWithValue("@email", (object?)p.Persona.Email ?? DBNull.Value);
                    command.Parameters.AddWithValue("@telefono", (object?)p.Persona.Telefono ?? DBNull.Value);

                    personaId = Convert.ToInt32(command.ExecuteScalar());
                }

                // 2. Insertar en propietario (estado fijo en 'ACTIVO')
                string sqlPropietario = @"
                    INSERT INTO propietario 
                    (persona_id, estado, fecha_creacion, fecha_modificacion, creado_por, modificado_por)
                    VALUES (@persona_id, 'ACTIVO', NOW(), NOW(), @creado_por, @modificado_por);
                    SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sqlPropietario, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@persona_id", personaId);
                    command.Parameters.AddWithValue("@creado_por", (object?)p.CreadoPor ?? DBNull.Value);
                    command.Parameters.AddWithValue("@modificado_por", (object?)p.ModificadoPor ?? DBNull.Value);

                    res = Convert.ToInt32(command.ExecuteScalar());

                    p.Persona.Id = personaId;
                    p.Id = res;

                    connection.Close();
                }
            }
            return res;
}


        public Propietario ObtenerPorId(int id)
        {
            Propietario? p = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT pr.id, pr.estado, pr.fecha_creacion, pr.fecha_modificacion, 
                        pr.creado_por, pr.modificado_por,
                        pe.id as personaId, pe.dni, pe.sexo, pe.nombre, pe.apellido, 
                        pe.fecha_nacimiento, pe.email, pe.telefono,
                        pe.fecha_creacion as persona_creacion, 
                        pe.fecha_modificacion as persona_modificacion
                    FROM propietario pr
                    INNER JOIN persona pe ON pr.persona_id = pe.id
                    WHERE pr.id = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            p = new Propietario
                            {
                                Id = reader.GetInt32("id"),
                                Estado = reader.GetString("estado"),
                                FechaCreacion = reader.GetDateTime("fecha_creacion"),
                                FechaModificacion = reader.GetDateTime("fecha_modificacion"),
                                CreadoPor = reader["creado_por"] as int?,
                                ModificadoPor = reader["modificado_por"] as int?,
                                Persona = new Persona
                                {
                                    Id = reader.GetInt32("personaId"),
                                    Dni = reader.GetString("dni"),
                                    Sexo = reader.GetString("sexo"),
                                    Nombre = reader.GetString("nombre"),
                                    Apellido = reader.GetString("apellido"),
                                    FechaNacimiento = reader["fecha_nacimiento"] as DateTime?,
                                    Email = reader["email"].ToString(),
                                    Telefono = reader["telefono"].ToString(),
                                    FechaCreacion = reader.GetDateTime("persona_creacion"),
                                    FechaModificacion = reader.GetDateTime("persona_modificacion")
                                }
                            };
                        }
                    }
                }
            }
            return p!;
        }

        public int Modificar(Propietario p)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Update persona
                string sqlPersona = @"
                    UPDATE persona SET 
                        dni=@dni, sexo=@sexo, nombre=@nombre, apellido=@apellido, 
                        fecha_nacimiento=@fecha_nacimiento, email=@email, telefono=@telefono, 
                        fecha_modificacion=NOW()
                    WHERE id=@id;";
                using (var command = new MySqlCommand(sqlPersona, connection))
                {
                    command.Parameters.AddWithValue("@id", p.Persona.Id);
                    command.Parameters.AddWithValue("@dni", p.Persona.Dni);
                    command.Parameters.AddWithValue("@sexo", p.Persona.Sexo);
                    command.Parameters.AddWithValue("@nombre", p.Persona.Nombre);
                    command.Parameters.AddWithValue("@apellido", p.Persona.Apellido);
                    command.Parameters.AddWithValue("@fecha_nacimiento", (object?)p.Persona.FechaNacimiento ?? DBNull.Value);
                    command.Parameters.AddWithValue("@email", (object?)p.Persona.Email ?? DBNull.Value);
                    command.Parameters.AddWithValue("@telefono", (object?)p.Persona.Telefono ?? DBNull.Value);
                    command.ExecuteNonQuery();
                }

                // Update propietario
                string sqlPropietario = @"
                    UPDATE propietario SET  
                        fecha_modificacion=NOW(),
                        modificado_por=@modificado_por
                    WHERE id=@id;";
                using (var command = new MySqlCommand(sqlPropietario, connection))
                {
                    command.Parameters.AddWithValue("@id", p.Id);
                    command.Parameters.AddWithValue("@modificado_por", (object?)p.ModificadoPor ?? DBNull.Value);
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int BajaLogica(int id, int? modificadoPor = null)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = @"
                    UPDATE propietario 
                    SET estado = 'INACTIVO',
                        fecha_modificacion = NOW()
                    WHERE id = @id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    //command.Parameters.AddWithValue("@modificado_por", (object?)modificadoPor ?? DBNull.Value);
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
}

    }
}