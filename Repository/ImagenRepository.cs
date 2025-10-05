using Inmobilaria_lab2_TPI_MGS.Models;
using MySql.Data.MySqlClient;

namespace Inmobilaria_lab2_TPI_MGS.Repository
{
    public class ImagenRepository : BaseRepository
    {

        public ImagenRepository() : base()
        {

        }

        public IList<Imagen> BuscarPorInmueble(int inmuebleId)
        {
            var lista = new List<Imagen>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT id, inmueble_id, url
                               FROM imagen WHERE inmueble_id = @inmuebleId";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@inmuebleId", inmuebleId);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Imagen
                            {
                                Id = reader.GetInt32("id"),
                                InmuebleId = reader.GetInt32("inmueble_id"),
                                Url = reader.GetString("url"),
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public int Alta(Imagen imagen)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO imagen (inmueble_id, url)
                               VALUES (@inmuebleId, @url)";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@inmuebleId", imagen.InmuebleId);
                    command.Parameters.AddWithValue("@url", imagen.Url);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return res;
        }
        
        public void Eliminar(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "DELETE FROM imagen WHERE id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
