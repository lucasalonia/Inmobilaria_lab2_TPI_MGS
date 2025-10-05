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

        public IList<Propietario> ObtenerTodos(int paginaNro = 1, int tamPagina = 10)
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
                    ORDER BY p.id
                    LIMIT @tamPagina OFFSET @offset;
                    ";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    int offset = (paginaNro - 1) * tamPagina;
                    command.Parameters.AddWithValue("@tamPagina", tamPagina);
                    command.Parameters.AddWithValue("@offset", offset);

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
                                    FechaNacimiento = reader.IsDBNull(reader.GetOrdinal("fecha_nacimiento")) ? null : reader.GetDateTime("fecha_nacimiento"),
                                    Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString("email"),
                                    Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString("telefono"),
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
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            // 1. Usar el Id de Persona si ya existe (viene del formulario)
            int personaId = p.Persona.Id;

            // 2. Si no viene Id, buscar por DNI en la base de datos
            if (personaId == 0)
            {
                var checkSql = "SELECT id FROM persona WHERE dni = @dni";
                using (var checkCmd = new MySqlCommand(checkSql, connection))
                {
                    checkCmd.Parameters.AddWithValue("@dni", p.Persona.Dni);
                    var result = checkCmd.ExecuteScalar();
                    personaId = result != null ? Convert.ToInt32(result) : 0;
                }
            }

            // 3. Insertar persona solo si personaId sigue siendo 0
            if (personaId == 0)
            {
                string sqlPersona = @"
                    INSERT INTO persona 
                    (dni, sexo, nombre, apellido, fecha_nacimiento, email, telefono, fecha_creacion, fecha_modificacion)
                    VALUES 
                    (@dni, @sexo, @nombre, @apellido, @fecha_nacimiento, @email, @telefono, NOW(), NOW());
                    SELECT LAST_INSERT_ID();";

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
                    p.Persona.Id = personaId; // asignamos Id recién creado
                }
            }
            // 4. Verificar si ya existe un propietario para esa persona
            var checkPropSql = "SELECT id, estado FROM propietario WHERE persona_id = @persona_id";
            using (var checkPropCmd = new MySqlCommand(checkPropSql, connection))
            {
                checkPropCmd.Parameters.AddWithValue("@persona_id", personaId);
                using var reader = checkPropCmd.ExecuteReader();
                if (reader.Read())
                {
                    int propietarioIdExistente = reader.GetInt32("id");
                    string estadoActual = reader.GetString("estado");
                    reader.Close();

                    if (estadoActual == "INACTIVO")
                    {
                        // Actualizar a ACTIVO
                        var updateSql = @"
                                UPDATE propietario 
                                SET estado = 'ACTIVO', fecha_modificacion = NOW(), modificado_por = @modificado_por 
                                WHERE id = @id";
                        using var updateCmd = new MySqlCommand(updateSql, connection);
                        updateCmd.Parameters.AddWithValue("@modificado_por", (object?)p.ModificadoPor ?? DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@id", propietarioIdExistente);
                        updateCmd.ExecuteNonQuery();

                        p.Id = propietarioIdExistente;
                        return propietarioIdExistente;
                    }
                    else
                    {
                        // Ya existe como propietario ACTIVO
                        throw new Exception("La persona ya es un propietario activo.");
                    }
                }
                reader.Close();
            }
            // 4. Insertar propietario usando personaId
            string sqlPropietario = @"
                INSERT INTO propietario 
                (persona_id, estado, fecha_creacion, fecha_modificacion, creado_por, modificado_por)
                VALUES 
                (@persona_id, @estado, NOW(), NOW(), @creado_por, @modificado_por);
                SELECT LAST_INSERT_ID();";

            using var cmdProp = new MySqlCommand(sqlPropietario, connection);
            cmdProp.Parameters.AddWithValue("@persona_id", personaId);
            cmdProp.Parameters.AddWithValue("@estado", p.Estado ?? "ACTIVO");
            cmdProp.Parameters.AddWithValue("@creado_por", (object?)p.CreadoPor ?? DBNull.Value);
            cmdProp.Parameters.AddWithValue("@modificado_por", (object?)p.ModificadoPor ?? DBNull.Value);

            int propietarioId = Convert.ToInt32(cmdProp.ExecuteScalar());
            p.Id = propietarioId; // asignamos Id recién creado

            return propietarioId;
        }


        public Propietario ObtenerPorId(int id)
        {
            Propietario? p = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                     pr.id, pr.estado, pr.fecha_creacion, pr.fecha_modificacion, 
                pr.creado_por, pr.modificado_por,
                uc.username AS creado_por_username,
                um.username AS modificado_por_username,
                pe.id AS personaId, pe.dni, pe.sexo, pe.nombre, pe.apellido, 
                pe.fecha_nacimiento, pe.email, pe.telefono,
                pe.fecha_creacion AS persona_creacion, 
                pe.fecha_modificacion AS persona_modificacion
            FROM propietario pr
            INNER JOIN persona pe ON pr.persona_id = pe.id
            LEFT JOIN usuario uc ON pr.creado_por = uc.id
            LEFT JOIN usuario um ON pr.modificado_por = um.id
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
                                CreadoPor = reader["creado_por"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["creado_por"]),
                                ModificadoPor = reader["modificado_por"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["modificado_por"]),
                                CreadoPorNombre = reader["creado_por_username"] == DBNull.Value ? null : reader.GetString("creado_por_username"),
                                ModificadoPorNombre = reader["modificado_por_username"] == DBNull.Value ? null : reader.GetString("modificado_por_username"),
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

        public Persona? ObtenerPorDni(string dni)
        {
            using var connection = new MySqlConnection(connectionString);
            const string sql = @"
                SELECT 
                    id AS personaId,
                    dni, sexo, nombre, apellido,
                    fecha_nacimiento, email, telefono
                FROM persona
                WHERE dni = @dni
                LIMIT 1;";

            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@dni", dni);
            connection.Open();

            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null; // no existe la persona

            return new Persona
            {
                Id = r.GetInt32("personaId"),
                Dni = r.GetString("dni"),
                Sexo = r["sexo"] == DBNull.Value ? "" : r.GetString("sexo"),
                Nombre = r["nombre"] == DBNull.Value ? "" : r.GetString("nombre"),
                Apellido = r["apellido"] == DBNull.Value ? "" : r.GetString("apellido"),
                FechaNacimiento = r["fecha_nacimiento"] == DBNull.Value ? null : r.GetDateTime("fecha_nacimiento"),
                Email = r["email"] == DBNull.Value ? "" : r.GetString("email"),
                Telefono = r["telefono"] == DBNull.Value ? "" : r.GetString("telefono")
            };
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

        public int ContarPropietariosActivos()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(*) FROM propietario WHERE estado = 'ACTIVO'";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
        
        public int ContarPropietarios()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(*) FROM propietario";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

    }
}