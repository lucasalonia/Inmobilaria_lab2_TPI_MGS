using System.Data;
using System.Data.SqlClient;
using Inmobilaria_lab2_TPI_MGS.Models;
using MySql.Data.MySqlClient;

namespace Inmobilaria_lab2_TPI_MGS.Repository
{
    public class InquilinoRepository : BaseRepository
    {
        public InquilinoRepository() : base()
        {

        }

        /*PENDIENTE: ENLAZAR CON TABLA DIRECCION*/


        //Alta teniendo en cuenta que si Inquilino o Persona existen previamente
        public bool Alta(Persona persona, int? idUsuario)
        {
            bool resultado = false;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string checkQuery = @"SELECT i.estado 
                                                    FROM persona p
                                                    INNER JOIN inquilino i ON p.id = i.persona_id
                                                    WHERE p.dni = @Dni;";


                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@Dni", persona.Dni);

                        var estadoObj = checkCmd.ExecuteScalar();
                        Console.WriteLine($"Estado del inquilino: {estadoObj}");

                        //Verificamos si el inquilino ya existe. Si NO encuentra un Dni que concida, 
                        // el estado será nulo. Si no es nulo es porque existe algun registro previo - LS
                        if (estadoObj != null)
                        {
                            string estado = estadoObj.ToString();
                            //Si el inquilino ya existe, verificamos su estado - LS
                            if (estado == "ACTIVO")
                            {
                                //Si el inquilino ya existe y está activo, no hacemos nada - LS
                                Console.WriteLine("El inquilino ya existe y está activo.");
                                return resultado;

                            }
                            //Si el inquilino ya existe pero está inactivo, lo activamos - LS
                            else if (estado == "INACTIVO")
                            {
                                Console.WriteLine("El inquilino ya existe pero está inactivo. Se actualizará su estado a activo.");

                                string query = @"UPDATE inquilino 
                                 SET estado = 'ACTIVO', fecha_modificacion = @FechaModificacion, modificado_por = @ModificadoPor
                                 WHERE persona_id = (SELECT id FROM persona WHERE dni = @Dni)";

                                using (MySqlCommand command = new MySqlCommand())
                                {
                                    command.Connection = connection;
                                    command.CommandText = query;

                                    command.Parameters.AddWithValue("@FechaModificacion", DateTime.Now);
                                    command.Parameters.AddWithValue("@ModificadoPor", idUsuario.HasValue ? idUsuario.Value : (object)DBNull.Value);
                                    command.Parameters.AddWithValue("@Dni", persona.Dni);

                                    int rowsAffected = command.ExecuteNonQuery();

                                    if (rowsAffected > 0)
                                    {
                                        resultado = true;
                                    }

                                }
                            }

                        }
                        //En el caso que no exista regitro del inquilino, hacemos uno nuevo - LS
                        //Verificacion de usuario o persona existente sin ser inquilino - LS
                        else
                        {
                            int personaId = 0;
                            string checkQueryDNI = @"SELECT id FROM persona WHERE dni = @Dni;";

                            using (MySqlCommand checkCommand = new MySqlCommand(checkQueryDNI, connection))
                            {
                                checkCommand.Parameters.AddWithValue("@Dni", persona.Dni);

                                var result = checkCommand.ExecuteScalar();

                                if (result != null)
                                {

                                    // Si ya existe una persona con el mismo DNI, obtenemos su ID. Esto es en el caso que la persona 
                                    // tenga un registro previo pero no lo tenga en la tabla inquilino. Puede ser en el caso de los usuarios por ejemplo - LS
                                    personaId = Convert.ToInt32(result);
                                    Console.WriteLine($"La persona ya existe con ID {personaId}, se procede a crear solo el inquilino.");
                                }
                                else
                                {

                                    Console.WriteLine("La persona no existe, se procederá a crear una nueva.");

                                    string insertPersonaQuery = @"INSERT INTO persona (dni, sexo, nombre, apellido, fecha_nacimiento, email, telefono, fecha_creacion, fecha_modificacion)
                                          VALUES (@Dni, @Sexo, @Nombre, @Apellido, @FechaNacimiento, @Email, @Telefono, @FechaCreacion, @FechaModificacion);
                                          SELECT LAST_INSERT_ID();";

                                    using (MySqlCommand insertPersonaCmd = new MySqlCommand(insertPersonaQuery, connection))
                                    {
                                        insertPersonaCmd.Parameters.AddWithValue("@Dni", persona.Dni);
                                        insertPersonaCmd.Parameters.AddWithValue("@Sexo", persona.Sexo);
                                        insertPersonaCmd.Parameters.AddWithValue("@Nombre", persona.Nombre);
                                        insertPersonaCmd.Parameters.AddWithValue("@Apellido", persona.Apellido);
                                        insertPersonaCmd.Parameters.AddWithValue("@FechaNacimiento", (object)persona.FechaNacimiento ?? DBNull.Value);
                                        insertPersonaCmd.Parameters.AddWithValue("@Email", persona.Email);
                                        insertPersonaCmd.Parameters.AddWithValue("@Telefono", persona.Telefono);
                                        insertPersonaCmd.Parameters.AddWithValue("@FechaCreacion", DateTime.Now);
                                        insertPersonaCmd.Parameters.AddWithValue("@FechaModificacion", DateTime.Now);


                                        personaId = Convert.ToInt32(insertPersonaCmd.ExecuteScalar());
                                    }
                                }
                            }

                            // Si se obtuvo un ID de persona, ya sea existente o una nueva, se crear el inquilino - LS
                            if (personaId > 0)
                            {
                                string insertInquilinoQuery = @"INSERT INTO inquilino (persona_id, estado, fecha_creacion, fecha_modificacion, creado_por, modificado_por)
                                        VALUES (@personaId, @Estado, @FechaCreacion, @FechaModificacion, @CreadoPor, @ModificadoPor);
                                        SELECT LAST_INSERT_ID();";

                                using (MySqlCommand insertInquilinoCmd = new MySqlCommand(insertInquilinoQuery, connection))
                                {
                                    insertInquilinoCmd.Parameters.AddWithValue("@personaId", personaId);
                                    insertInquilinoCmd.Parameters.AddWithValue("@Estado", "ACTIVO");
                                    insertInquilinoCmd.Parameters.AddWithValue("@FechaCreacion", DateTime.Now);
                                    insertInquilinoCmd.Parameters.AddWithValue("@FechaModificacion", DateTime.Now);
                                    insertInquilinoCmd.Parameters.AddWithValue("@CreadoPor", idUsuario.HasValue ? idUsuario.Value : (object)DBNull.Value);
                                    insertInquilinoCmd.Parameters.AddWithValue("@ModificadoPor", idUsuario.HasValue ? idUsuario.Value : (object)DBNull.Value);

                                    int inquilinoId = Convert.ToInt32(insertInquilinoCmd.ExecuteScalar());
                                    if (inquilinoId > 0)
                                    {
                                        resultado = true;
                                    }
                                }
                            }
                        }
                    }


                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error al insertar inquilino: {e.Message}");
                    resultado = false;
                    throw;

                }
                finally
                {
                    connection.Close();
                }
            }
            return resultado;
        }



        //Baja de inquilinos modificando el estado a inactivo
        public bool Baja(int idPersona, int? idUsuario)
        {
            bool resultado = false;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"UPDATE inquilino 
                                 SET estado = 'INACTIVO', fecha_modificacion = @FechaModificacion, modificado_por = @ModificadoPor
                                 WHERE persona_id = @idPersona";

                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = query;

                        command.Parameters.AddWithValue("@FechaModificacion", DateTime.Now);
                        command.Parameters.AddWithValue("@ModificadoPor", idUsuario.HasValue ? idUsuario.Value : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@idPersona", idPersona);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            resultado = true;
                        }

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error al eliminar inquilino: {e.Message}");
                    resultado = false;
                    throw;

                }
                finally
                {

                    connection.Close();

                }
            }

            return resultado;
        }


        //Modificacion tanto de datos de persona como de inquilino
        public bool Modificar(Persona persona, int? idUsuario)
        {
            bool resultado = false;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"UPDATE persona p
                                        JOIN inquilino i ON p.id = i.persona_id
                                        SET p.nombre = @Nombre,
                                            p.apellido = @Apellido,
                                            p.dni = @Dni,
                                            p.telefono = @Telefono,
                                            p.email = @Email,
                                            p.fecha_modificacion = @FechaModificacion,
                                            i.fecha_modificacion = @FechaModificacion,
                                            i.modificado_por = @ModificadoPor
                                        WHERE p.id = @idPersona;";

                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = query;

                        command.Parameters.AddWithValue("@Nombre", persona.Nombre);
                        command.Parameters.AddWithValue("@Apellido", persona.Apellido);
                        command.Parameters.AddWithValue("@Dni", persona.Dni);
                        command.Parameters.AddWithValue("@Telefono", persona.Telefono);
                        command.Parameters.AddWithValue("@Email", persona.Email);
                        command.Parameters.AddWithValue("@FechaModificacion", DateTime.Now);
                        command.Parameters.AddWithValue("@ModificadoPor", idUsuario.HasValue ? idUsuario.Value : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@idPersona", persona.Id);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            resultado = true;
                        }

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error al modificar persona : {e.Message}");
                    resultado = false;
                    throw;

                }
                finally
                {

                    connection.Close();

                }
            }

            return resultado;
        }


        //Lista todos los inquilinos activos
        //LO QUE QUEREMOS ES LISTAR INQUILINOS CON LOS DATOS DE LA PERSONA. EN ESTE CASO A DIFERENCIA DE LISTAR INQUILINOS SIN CONTRATO
        // SE OBTIENE TANTO EL ID DE INQUILINO COMO EL DE PERSONA PARA PODER MODIFICAR SUS DATOS - LS
        public IList<Inquilino> ObtenerTodos(int paginaNro = 1, int tamPagina = 10)
        {
            IList<Inquilino> inquilinos = new List<Inquilino>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"
                                SELECT i.id AS InquilinoId, i.estado, i.fecha_creacion AS InqFechaCreacion, i.fecha_modificacion AS InqFechaModificacion,
                                    p.id AS PersonaId, p.dni, p.sexo, p.nombre, p.apellido, p.fecha_nacimiento, p.email, p.telefono,
                                    p.fecha_creacion AS PersFechaCreacion, p.fecha_modificacion AS PersFechaModificacion
                                FROM inquilino i
                                JOIN persona p ON p.id = i.persona_id
                                WHERE i.estado = 'ACTIVO'
                                ORDER BY i.id
                                LIMIT @tamPagina OFFSET @offset;";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        int offset = (paginaNro - 1) * tamPagina;
                        command.Parameters.AddWithValue("@tamPagina", tamPagina);
                        command.Parameters.AddWithValue("@offset", offset);
                        command.CommandType = CommandType.Text;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Inquilino inquilino = new Inquilino
                                {
                                    Id = reader.GetInt32("InquilinoId"),
                                    Estado = reader.GetString("estado"),
                                    FechaCreacion = reader.GetDateTime("InqFechaCreacion"),
                                    FechaModificacion = reader.GetDateTime("InqFechaModificacion"),
                                    Persona = new Persona
                                    {
                                        Id = reader.GetInt32("PersonaId"),
                                        Dni = reader.GetString("dni"),
                                        Sexo = reader.GetString("sexo"),
                                        Nombre = reader.GetString("nombre"),
                                        Apellido = reader.GetString("apellido"),
                                        FechaNacimiento = reader.IsDBNull(reader.GetOrdinal("fecha_nacimiento"))
                                                            ? null
                                                            : reader.GetDateTime("fecha_nacimiento"),
                                        Email = reader.GetString("email"),
                                        Telefono = reader.GetString("telefono"),
                                        FechaCreacion = reader.GetDateTime("PersFechaCreacion"),
                                        FechaModificacion = reader.GetDateTime("PersFechaModificacion")
                                    }
                                };
                                inquilinos.Add(inquilino);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error al obtener inquilinos: {e.Message}");
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
            return inquilinos;
        }



        public IList<Inquilino> ListarInquilinosSinContrato(int pagina = 1, int tamPagina = 10)
        {
            IList<Inquilino> inquilinos = new List<Inquilino>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"
                                SELECT 
                                    i.id AS InquilinoId,
                                    i.estado AS InqEstado,
                                    i.fecha_creacion AS InqFechaCreacion,
                                    i.fecha_modificacion AS InqFechaModificacion,
                                    p.id AS PersonaId,
                                    p.dni,
                                    p.sexo,
                                    p.nombre,
                                    p.apellido,
                                    p.fecha_nacimiento,
                                    p.email,
                                    p.telefono,
                                    p.fecha_creacion AS PersFechaCreacion,
                                    p.fecha_modificacion AS PersFechaModificacion
                                FROM persona p
                                JOIN inquilino i ON p.id = i.persona_id
                                LEFT JOIN contrato c ON i.id = c.inquilino_id 
                                WHERE c.id IS NULL AND i.estado='ACTIVO'
                                ORDER BY i.id
                                LIMIT @tamPagina OFFSET @offset;";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        int offset = (pagina - 1) * tamPagina;
                        cmd.Parameters.AddWithValue("@tamPagina", tamPagina);
                        cmd.Parameters.AddWithValue("@offset", offset);
                        cmd.CommandType = CommandType.Text;


                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            //EN ESTE CASO LA PERSONA SE CONFIGURA CON EL ID DE INQUILINO. NO PASA LO MISMO CON OBTENERTODOS (Ver especificacion en el metodo) - LS
                            while (reader.Read())
                            {
                                Inquilino inquilino = new Inquilino
                                {
                                    Id = reader.GetInt32("InquilinoId"),
                                    Estado = reader.GetString("InqEstado"),
                                    FechaCreacion = reader.GetDateTime("InqFechaCreacion"),
                                    FechaModificacion = reader.GetDateTime("InqFechaModificacion"),
                                    Persona = new Persona
                                    {
                                        Id = reader.GetInt32("PersonaId"),
                                        Dni = reader.GetString("dni"),
                                        Sexo = reader.GetString("sexo"),
                                        Nombre = reader.GetString("nombre"),
                                        Apellido = reader.GetString("apellido"),
                                        FechaNacimiento = reader.IsDBNull(reader.GetOrdinal("fecha_nacimiento"))
                                                            ? null
                                                            : reader.GetDateTime("fecha_nacimiento"),
                                        Email = reader.GetString("email"),
                                        Telefono = reader.GetString("telefono"),
                                        FechaCreacion = reader.GetDateTime("PersFechaCreacion"),
                                        FechaModificacion = reader.GetDateTime("PersFechaModificacion")
                                    }
                                };
                                inquilinos.Add(inquilino);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error al obtener inquilinos: {e.Message}");
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
            return inquilinos;
        }

        public int ContarInquilinosActivos()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(*) FROM inquilino WHERE estado = 'ACTIVO'";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
        public int ContarInquilinosActivosSinContrato()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
            SELECT COUNT(*)
            FROM inquilino i
            LEFT JOIN contrato c ON i.id = c.inquilino_id AND c.estado = 'VIGENTE'
            WHERE i.estado = 'ACTIVO' AND c.id IS NULL";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
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
            if (!r.Read()) return null;

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
        public Inquilino ObtenerInquilinoPorContrato(int contratoId)
        {
            Inquilino inquilino = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"
                                    SELECT i.id AS InquilinoId,
                                        i.estado AS InquilinoEstado,
                                        i.fecha_creacion AS InquilinoFechaCreacion,
                                        i.fecha_modificacion AS InquilinoFechaModificacion,
                                        i.creado_por AS InquilinoCreadoPor,
                                        i.modificado_por AS InquilinoModificadoPor,
                                        p.id AS PersonaId,
                                        p.nombre,
                                        p.apellido,
                                        p.dni,
                                        p.sexo,
                                        p.fecha_nacimiento,
                                        p.email,
                                        p.telefono,
                                        p.fecha_creacion AS PersonaFechaCreacion,
                                        p.fecha_modificacion AS PersonaFechaModificacion
                                    FROM inquilino i
                                    JOIN persona p ON i.persona_id = p.id
                                    JOIN contrato c ON c.inquilino_id = i.id
                                    WHERE c.id = @ContratoId
                                    LIMIT 1;
                                ";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ContratoId", contratoId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                inquilino = new Inquilino
                                {
                                    Id = reader.GetInt32("InquilinoId"),
                                    Estado = reader.GetString("InquilinoEstado"),
                                    FechaCreacion = reader.GetDateTime("InquilinoFechaCreacion"),
                                    FechaModificacion = reader.GetDateTime("InquilinoFechaModificacion"),
                                    CreadoPor = reader.IsDBNull(reader.GetOrdinal("InquilinoCreadoPor")) ? (int?)null : reader.GetInt32("InquilinoCreadoPor"),
                                    ModificadoPor = reader.IsDBNull(reader.GetOrdinal("InquilinoModificadoPor")) ? (int?)null : reader.GetInt32("InquilinoModificadoPor"),
                                    Persona = new Persona
                                    {
                                        Id = reader.GetInt32("PersonaId"),
                                        Nombre = reader.GetString("nombre"),
                                        Apellido = reader.GetString("apellido"),
                                        Dni = reader.GetString("dni"),
                                        Sexo = reader.GetString("sexo"),
                                        FechaNacimiento = reader.IsDBNull(reader.GetOrdinal("fecha_nacimiento")) ? (DateTime?)null : reader.GetDateTime("fecha_nacimiento"),
                                        Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString("email"),
                                        Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString("telefono"),
                                        FechaCreacion = reader.GetDateTime("PersonaFechaCreacion"),
                                        FechaModificacion = reader.GetDateTime("PersonaFechaModificacion")
                                    }
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener inquilino por contrato: {ex.Message}");
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }

            return inquilino;
        }

        public Inquilino ObtenerPorId(int id)
        {
            Inquilino inquilino = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        i.id AS InquilinoId,
                        i.estado AS EstadoInquilino,
                        i.fecha_creacion AS FechaCreacion,
                        i.fecha_modificacion AS FechaModificacion,
                        i.creado_por AS CreadoPor,
                        i.modificado_por AS ModificadoPor,
                        p.id AS PersonaId,
                        p.dni,
                        p.nombre,
                        p.apellido,
                        p.email,
                        p.telefono,
                        p.sexo,
                        p.fecha_nacimiento
                    FROM inquilino i
                    INNER JOIN persona p ON i.persona_id = p.id
                    WHERE i.id = @id;
                ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var persona = new Persona
                            {
                                Id = Convert.ToInt32(reader["PersonaId"]),
                                Dni = reader["dni"].ToString(),
                                Nombre = reader["nombre"].ToString(),
                                Apellido = reader["apellido"].ToString(),
                                Email = reader["email"].ToString(),
                                Telefono = reader["telefono"] != DBNull.Value ? reader["telefono"].ToString() : null,
                                Sexo = reader["sexo"].ToString(),
                                FechaNacimiento = Convert.ToDateTime(reader["fecha_nacimiento"])
                            };

                            inquilino = new Inquilino
                            {
                                Id = Convert.ToInt32(reader["InquilinoId"]),
                                Persona = persona,
                                Estado = reader["EstadoInquilino"].ToString(),
                                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                                FechaModificacion = Convert.ToDateTime(reader["FechaModificacion"]),
                                CreadoPor = reader["CreadoPor"] != DBNull.Value ? Convert.ToInt32(reader["CreadoPor"]) : null,
                                ModificadoPor = reader["ModificadoPor"] != DBNull.Value ? Convert.ToInt32(reader["ModificadoPor"]) : null
                            };
                        }
                    }
                }
            }

            return inquilino;
        }
    }


}