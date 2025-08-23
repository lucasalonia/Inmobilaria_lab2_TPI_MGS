using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
namespace Inmobilaria_lab2_TPI_MGS.Models
{
    public class RepositorioInquilino : RepositorioBase
    {
        public RepositorioInquilino() : base()
        {

        }

        /*PENDIENTE: ENLAZAR CON TABLA DIRECCION*/


        //Alta teniendo en cuenta que si Inquilino o Persona existen previamente
        public bool Alta(Persona persona)
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
                                    command.Parameters.AddWithValue("@ModificadoPor", 1);
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
                                    insertInquilinoCmd.Parameters.AddWithValue("@CreadoPor", 1);
                                    insertInquilinoCmd.Parameters.AddWithValue("@ModificadoPor", 1);

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
        public bool Baja(int idPersona)
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
                        command.Parameters.AddWithValue("@ModificadoPor", 1);
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
        public bool Modificar(Persona persona)
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
                        command.Parameters.AddWithValue("@ModificadoPor", 1);
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
        public IList<Persona> ObtenerTodos()
        {
            IList<Persona> personas = new List<Persona>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"SELECT p.id, p.dni, p.sexo, p.nombre, p.apellido, p.fecha_nacimiento, p.email, p.telefono, 
                                    p.fecha_creacion, p.fecha_modificacion 
                                    FROM persona p 
                                    JOIN inquilino i ON p.id = i.persona_id 
                                    WHERE i.estado = 'ACTIVO'";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Persona persona = new Persona
                            {
                                Id = reader.GetInt32("id"),
                                Dni = reader.GetString("dni"),
                                Sexo = reader.GetString("sexo"),
                                Nombre = reader.GetString("nombre"),
                                Apellido = reader.GetString("apellido"),
                                FechaNacimiento = reader.IsDBNull(reader.GetOrdinal("fecha_nacimiento")) ? null : reader.GetDateTime("fecha_nacimiento"),
                                Email = reader.GetString("email"),
                                Telefono = reader.GetString("telefono"),
                                FechaCreacion = reader.GetDateTime("fecha_creacion"),
                                FechaModificacion = reader.GetDateTime("fecha_modificacion")
                            };
                            personas.Add(persona);
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
            return personas;
        }

    }
}