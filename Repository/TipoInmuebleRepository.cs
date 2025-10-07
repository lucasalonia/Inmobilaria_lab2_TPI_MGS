using Inmobilaria_lab2_TPI_MGS.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace Inmobilaria_lab2_TPI_MGS.Repository
{
    public class TipoInmuebleRepository : BaseRepository
    {
        public TipoInmuebleRepository() : base() { }

        // Obtener todos los tipos de inmueble (para dropdowns)
        public IList<TipoInmueble> ObtenerTodos()
        {
            var lista = new List<TipoInmueble>();

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT id, nombre, descripcion 
                               FROM tipo_inmueble
                               ORDER BY nombre ASC;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new TipoInmueble
                            {
                                Id = reader.GetInt32("id"),
                                Nombre = reader.GetString("nombre"),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion"))
                                    ? null
                                    : reader.GetString("descripcion")
                            });
                        }
                    }
                }
            }

            return lista;
        }

        // Obtener un tipo por ID (para detalles o validaciones)
        public TipoInmueble? ObtenerPorId(int id)
        {
            TipoInmueble? tipo = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT id, nombre, descripcion 
                               FROM tipo_inmueble
                               WHERE id = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            tipo = new TipoInmueble
                            {
                                Id = reader.GetInt32("id"),
                                Nombre = reader.GetString("nombre"),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion"))
                                    ? null
                                    : reader.GetString("descripcion")
                            };
                        }
                    }
                }
            }

            return tipo;
        }

        public int Alta(TipoInmueble tipo)
        {
            int nuevoId = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO tipo_inmueble (nombre, descripcion)
                               VALUES (@nombre, @descripcion);
                               SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", tipo.Nombre);
                    command.Parameters.AddWithValue("@descripcion", (object?)tipo.Descripcion ?? DBNull.Value);
                    connection.Open();
                    nuevoId = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return nuevoId;
        }


        public int Modificar(TipoInmueble tipo)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE tipo_inmueble 
                               SET nombre = @nombre, descripcion = @descripcion 
                               WHERE id = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", tipo.Id);
                    command.Parameters.AddWithValue("@nombre", tipo.Nombre);
                    command.Parameters.AddWithValue("@descripcion", (object?)tipo.Descripcion ?? DBNull.Value);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        // Eliminar (baja física)
        public int Eliminar(int id)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"DELETE FROM tipo_inmueble WHERE id = @id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }
    }
}
