using System.Data;
using Inmobilaria_lab2_TPI_MGS.Models;
using MySql.Data.MySqlClient;

namespace Inmobilaria_lab2_TPI_MGS.Repository
{
    public class InmuebleRepository : BaseRepository
    {
        public InmuebleRepository() : base()
        {

        }
        public IList<Inmueble> ObtenerTodos(int paginaNro = 1, int tamPagina = 10)
        {
            IList<Inmueble> res = new List<Inmueble>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT id, tipo, estado, superficie_m2, ambientes, banos, cochera, direccion, descripcion, fecha_creacion, fecha_modificacion
                               FROM inmueble
                               WHERE estado = 'ACTIVO'";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Inmueble inmueble = new Inmueble
                        {
                            Id = reader.GetInt32("id"),
                            Estado = reader.GetString("estado"),
                            Tipo = reader.IsDBNull(reader.GetOrdinal("tipo")) ? null : reader.GetString("tipo"),
                            SuperficieM2 = reader.IsDBNull(reader.GetOrdinal("superficie_m2")) ? (int?)null : reader.GetInt32("superficie_m2"),
                            Ambientes = reader.IsDBNull(reader.GetOrdinal("ambientes")) ? null : reader.GetInt32("ambientes"),
                            Banos = reader.IsDBNull(reader.GetOrdinal("banos")) ? null : reader.GetInt32("banos"),
                            Cochera = reader.GetInt32("cochera"),
                            Direccion = reader.IsDBNull(reader.GetOrdinal("direccion")) ? null : reader.GetString("direccion"),
                            Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : reader.GetString("descripcion"),
                            FechaCreacion = reader.GetDateTime("fecha_creacion"),
                            FechaModificacion = reader.GetDateTime("fecha_modificacion"),
                        };
                        res.Add(inmueble);
                    }
                    connection.Close();
                }
            }
            return res;
        }
        public int Alta(Inmueble inmueble)
        {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            int InmuebleId = 0;
            string sql = @"INSERT INTO inmueble 
            (propietario_id, estado, tipo, superficie_m2, ambientes, banos, cochera, direccion, descripcion, fecha_creacion, fecha_modificacion, creado_por, modificado_por)
            VALUES (@propietario_id, @estado, @tipo, @superficie_m2, @ambientes, @banos, @cochera, @direccion, @descripcion, NOW(), NOW(), 1, 1);
            SELECT LAST_INSERT_ID();";

            using (var cmdInm = new MySqlCommand(sql, connection))
            {
                cmdInm.Parameters.AddWithValue("@propietario_id", inmueble.PropietarioId);
                cmdInm.Parameters.AddWithValue("@estado", inmueble.Estado ?? "ACTIVO");
                cmdInm.Parameters.AddWithValue("@tipo", (object?)inmueble.Tipo ?? DBNull.Value);
                cmdInm.Parameters.AddWithValue("@superficie_m2", (object?)inmueble.SuperficieM2 ?? DBNull.Value);
                cmdInm.Parameters.AddWithValue("@ambientes", (object?)inmueble.Ambientes ?? DBNull.Value);
                cmdInm.Parameters.AddWithValue("@banos", (object?)inmueble.Banos ?? DBNull.Value);
                cmdInm.Parameters.AddWithValue("@cochera", inmueble.Cochera);
                cmdInm.Parameters.AddWithValue("@direccion", (object?)inmueble.Direccion ?? DBNull.Value);
                cmdInm.Parameters.AddWithValue("@descripcion", inmueble.Descripcion);
                cmdInm.Parameters.AddWithValue("@creado_por", (object?)inmueble.CreadoPor ?? DBNull.Value);
                cmdInm.Parameters.AddWithValue("@modificado_por", (object?)inmueble.ModificadoPor ?? DBNull.Value);

                InmuebleId = Convert.ToInt32(cmdInm.ExecuteScalar());
                inmueble.Id = InmuebleId; // asignamos Id recién creado
            }

            return InmuebleId;
        }

    }
}