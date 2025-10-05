using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Inmobilaria_lab2_TPI_MGS.Models;

namespace Inmobilaria_lab2_TPI_MGS.Repository
{
    public class ContratoRepository : BaseRepository
    {
        public ContratoRepository() : base()
        {
        }


        public bool Alta(Contrato contrato, int? idUsuario)
        {
            bool exito = false;
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"
                                    INSERT INTO contrato (inmueble_id, inquilino_id, fecha_inicio, fecha_fin, estado, monto_mensual, moneda, deposito, 
                                    observaciones, fecha_creacion, fecha_modificacion, creado_por)
                                    VALUES (@InmuebleId, @InquilinoId, @FechaInicio, @FechaFin, 
                                    @Estado, @MontoMensual, @Moneda, @Deposito, @Observaciones, NOW(), NOW(), @CreadoPor)";
                    using (var command = new MySqlCommand(query, connection))

                    {
                        command.Parameters.AddWithValue("@InmuebleId", contrato.InmuebleId);
                        command.Parameters.AddWithValue("@InquilinoId", contrato.InquilinoId);
                        command.Parameters.AddWithValue("@FechaInicio", contrato.FechaInicio);
                        command.Parameters.AddWithValue("@FechaFin", contrato.FechaFin);
                        command.Parameters.AddWithValue("@Estado", contrato.Estado);
                        command.Parameters.AddWithValue("@MontoMensual", (object)contrato.MontoMensual ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Moneda", (object)contrato.Moneda ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Deposito", (object)contrato.Deposito ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Observaciones", (object)contrato.Observaciones ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CreadoPor", idUsuario.HasValue ? idUsuario.Value : (object)DBNull.Value);
                        int rowsAffected = command.ExecuteNonQuery();
                        bool insertado = rowsAffected > 0;

                        if (insertado)
                        {

                            contrato.Id = (int)command.LastInsertedId;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error al insertar contrato: {e.Message}");
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
            return exito;
        }

        public bool Modificar(Contrato contrato, int? idUsuario)
        {
            bool exito = false;
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    var updates = new List<string>();
                    var parameters = new List<MySqlParameter>();

                    if (contrato.FechaFin != default(DateTime))
                    {
                        updates.Add("fecha_fin = @FechaFin");
                        parameters.Add(new MySqlParameter("@FechaFin", contrato.FechaFin));
                    }
                    if (!string.IsNullOrWhiteSpace(contrato.Estado))
                    {
                        updates.Add("estado = @Estado");
                        parameters.Add(new MySqlParameter("@Estado", contrato.Estado));
                    }
                    if (contrato.MontoMensual.HasValue)
                    {
                        updates.Add("monto_mensual = @MontoMensual");
                        parameters.Add(new MySqlParameter("@MontoMensual", contrato.MontoMensual.Value));
                    }
                    if (!string.IsNullOrWhiteSpace(contrato.Moneda))
                    {
                        updates.Add("moneda = @Moneda");
                        parameters.Add(new MySqlParameter("@Moneda", contrato.Moneda));
                    }
                    if (contrato.Deposito.HasValue)
                    {
                        updates.Add("deposito = @Deposito");
                        parameters.Add(new MySqlParameter("@Deposito", contrato.Deposito.Value));
                    }
                    if (!string.IsNullOrWhiteSpace(contrato.Observaciones))
                    {
                        updates.Add("observaciones = @Observaciones");
                        parameters.Add(new MySqlParameter("@Observaciones", contrato.Observaciones));
                    }


                    updates.Add("fecha_modificacion = NOW()");
                    updates.Add("modificado_por = @ModificadoPor");
                    parameters.Add(new MySqlParameter("@ModificadoPor", idUsuario.HasValue ? idUsuario.Value : (object)DBNull.Value));

                    string query = $@"
                                    UPDATE contrato
                                    SET {string.Join(", ", updates)}
                                    WHERE id = @Id";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", contrato.Id);
                        command.Parameters.AddRange(parameters.ToArray());

                        int rowsAffected = command.ExecuteNonQuery();
                        exito = rowsAffected > 0;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error al modificar contrato: {e.Message}");
                    throw;
                }
            }
            return exito;
        }
        public bool Baja(int contratoId, int? idUsuario)
        {
            bool exito = false;
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"UPDATE contrato
                                     SET estado = 'NO VIGENTE',
                                         fecha_modificacion = NOW(),
                                         modificado_por = @ModificadoPor
                                     WHERE id = @Id";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", contratoId);
                        command.Parameters.AddWithValue("@ModificadoPor", idUsuario.HasValue ? idUsuario.Value : (object)DBNull.Value);
                        int rowsAffected = command.ExecuteNonQuery();
                        exito = rowsAffected > 0;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error al eliminar contrato: {e.Message}");
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
            return exito;
        }

        public IList<Contrato> ListaContratosVigentes(int paginaNro = 1, int tamPagina = 10)
        {
            IList<Contrato> contratos = new List<Contrato>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"
                                    SELECT id, inmueble_id, inquilino_id, fecha_inicio, fecha_fin, estado, monto_mensual, moneda, deposito, observaciones
                                    FROM contrato
                                    WHERE estado = 'VIGENTE' AND fecha_fin >= CURDATE()
                                    ORDER BY fecha_inicio DESC
                                    LIMIT @TamPagina OFFSET @Offset";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        int offset = (paginaNro - 1) * tamPagina;
                        command.Parameters.AddWithValue("@TamPagina", tamPagina);
                        command.Parameters.AddWithValue("@Offset", offset);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Contrato contrato = new Contrato
                                {
                                    Id = reader.GetInt32("id"),
                                    InmuebleId = reader.GetInt32("inmueble_id"),
                                    InquilinoId = reader.GetInt32("inquilino_id"),
                                    FechaInicio = reader.GetDateTime("fecha_inicio"),
                                    FechaFin = reader.GetDateTime("fecha_fin"),
                                    Estado = reader.GetString("estado"),
                                    MontoMensual = reader.IsDBNull(reader.GetOrdinal("monto_mensual")) ? (decimal?)null : reader.GetDecimal("monto_mensual"),
                                    Moneda = reader.IsDBNull(reader.GetOrdinal("moneda")) ? null : reader.GetString("moneda"),
                                    Deposito = reader.IsDBNull(reader.GetOrdinal("deposito")) ? (decimal?)null : reader.GetDecimal("deposito"),
                                    Observaciones = reader.IsDBNull(reader.GetOrdinal("observaciones")) ? null : reader.GetString("observaciones")
                                };

                                contratos.Add(contrato);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error al obtener contratos vigentes: {e.Message}");
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }

            return contratos;


        }
        public Contrato ObtenerPorId(int contratoId)
        {
            Contrato contrato = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"
                                SELECT id, inmueble_id, inquilino_id, fecha_inicio, fecha_fin, estado, monto_mensual, moneda, deposito, observaciones, fecha_creacion, fecha_modificacion, creado_por
                                FROM contrato
                                WHERE id = @ContratoId
                                LIMIT 1";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ContratoId", contratoId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                contrato = new Contrato
                                {
                                    Id = reader.GetInt32("id"),
                                    InmuebleId = reader.GetInt32("inmueble_id"),
                                    InquilinoId = reader.GetInt32("inquilino_id"),
                                    FechaInicio = reader.GetDateTime("fecha_inicio"),
                                    FechaFin = reader.GetDateTime("fecha_fin"),
                                    Estado = reader.GetString("estado"),
                                    MontoMensual = reader.IsDBNull(reader.GetOrdinal("monto_mensual")) ? 0 : reader.GetDecimal("monto_mensual"),
                                    Moneda = reader.IsDBNull(reader.GetOrdinal("moneda")) ? null : reader.GetString("moneda"),
                                    Deposito = reader.IsDBNull(reader.GetOrdinal("deposito")) ? 0 : reader.GetDecimal("deposito"),
                                    Observaciones = reader.IsDBNull(reader.GetOrdinal("observaciones")) ? null : reader.GetString("observaciones"),
                                    FechaCreacion = reader.GetDateTime("fecha_creacion"),
                                    FechaModificacion = reader.GetDateTime("fecha_modificacion"),
                                    CreadoPor = reader.IsDBNull(reader.GetOrdinal("creado_por"))
                                        ? (ulong?)null
                                        : (ulong)reader.GetInt32("creado_por")
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener contrato: {ex.Message}");
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }

            return contrato;
        }

        public Contrato ObtenerContratoVigente(int inquilinoId)
        {
            Contrato contrato = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"
                                    SELECT id, inmueble_id, inquilino_id, fecha_inicio, fecha_fin, estado, monto_mensual, moneda, deposito, observaciones
                                    FROM contrato
                                    WHERE inquilino_id = @InquilinoId
                                    AND estado = 'VIGENTE'
                                    AND fecha_fin >= CURDATE()
                                    ORDER BY fecha_inicio DESC
                                    LIMIT 1";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@InquilinoId", inquilinoId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                contrato = new Contrato
                                {
                                    Id = reader.GetInt32("id"),
                                    InmuebleId = reader.GetInt32("inmueble_id"),
                                    InquilinoId = reader.GetInt32("inquilino_id"),
                                    FechaInicio = reader.GetDateTime("fecha_inicio"),
                                    FechaFin = reader.GetDateTime("fecha_fin"),
                                    Estado = reader.GetString("estado"),
                                    MontoMensual = reader.IsDBNull(reader.GetOrdinal("monto_mensual")) ? (decimal?)null : reader.GetDecimal("monto_mensual"),
                                    Moneda = reader.IsDBNull(reader.GetOrdinal("moneda")) ? null : reader.GetString("moneda"),
                                    Deposito = reader.IsDBNull(reader.GetOrdinal("deposito")) ? (decimal?)null : reader.GetDecimal("deposito"),
                                    Observaciones = reader.IsDBNull(reader.GetOrdinal("observaciones")) ? null : reader.GetString("observaciones")
                                };
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error al obtener contrato vigente: {e.Message}");
                    throw;
                }
            }

            return contrato;
        }

        public IList<Contrato> ObtenerTodos(int? inquilinoId = null)
        {
            IList<Contrato> contratos = new List<Contrato>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"
                                    SELECT id, inmueble_id, inquilino_id, fecha_inicio, fecha_fin, estado, monto_mensual, moneda, deposito, observaciones
                                    FROM contrato
                                    WHERE (@InquilinoId IS NULL OR inquilino_id = @InquilinoId)
                                    ORDER BY fecha_inicio DESC";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@InquilinoId", (object)inquilinoId ?? DBNull.Value);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Contrato contrato = new Contrato
                                {
                                    Id = reader.GetInt32("id"),
                                    InmuebleId = reader.GetInt32("inmueble_id"),
                                    InquilinoId = reader.GetInt32("inquilino_id"),
                                    FechaInicio = reader.GetDateTime("fecha_inicio"),
                                    FechaFin = reader.GetDateTime("fecha_fin"),
                                    Estado = reader.GetString("estado"),
                                    MontoMensual = reader.IsDBNull(reader.GetOrdinal("monto_mensual")) ? (decimal?)null : reader.GetDecimal("monto_mensual"),
                                    Moneda = reader.IsDBNull(reader.GetOrdinal("moneda")) ? null : reader.GetString("moneda"),
                                    Deposito = reader.IsDBNull(reader.GetOrdinal("deposito")) ? (decimal?)null : reader.GetDecimal("deposito"),
                                    Observaciones = reader.IsDBNull(reader.GetOrdinal("observaciones")) ? null : reader.GetString("observaciones")
                                };

                                contratos.Add(contrato);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error al obtener contratos: {e.Message}");
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }

            return contratos;
        }
        public Contrato ObtenerInmuebleSinContrato(int inmuebleId)
        {
            Contrato contrato = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"
                                    SELECT id, inmueble_id, fecha_inicio, fecha_fin, estado, monto_mensual, moneda, deposito, observaciones
                                    FROM contrato
                                    WHERE inmueble_id = @InmuebleId";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@InquilinoId", inmuebleId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                contrato = new Contrato
                                {
                                    Id = reader.GetInt32("id"),
                                    InmuebleId = reader.GetInt32("inmueble_id"),
                                    FechaInicio = reader.GetDateTime("fecha_inicio"),
                                    FechaFin = reader.GetDateTime("fecha_fin"),
                                    Estado = reader.GetString("estado"),
                                    MontoMensual = reader.IsDBNull(reader.GetOrdinal("monto_mensual")) ? (decimal?)null : reader.GetDecimal("monto_mensual"),
                                    Moneda = reader.IsDBNull(reader.GetOrdinal("moneda")) ? null : reader.GetString("moneda"),
                                    Deposito = reader.IsDBNull(reader.GetOrdinal("deposito")) ? (decimal?)null : reader.GetDecimal("deposito"),
                                    Observaciones = reader.IsDBNull(reader.GetOrdinal("observaciones")) ? null : reader.GetString("observaciones")
                                };
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error al obtener contrato vigente: {e.Message}");
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }

            return contrato;
        }
        public List<Inmueble> ListarInmueblesDisponibles(int paginaNro = 1, int tamPagina = 5)
        {
            var inmuebles = new List<Inmueble>();

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @$"
                SELECT i.id, i.direccion, i.tipo, i.estado, i.superficie_m2, i.ambientes, i.banos, i.cochera
                FROM inmueble i
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM contrato c
                    WHERE c.inmueble_id = i.id
                      AND CURDATE() BETWEEN c.fecha_inicio AND c.fecha_fin
                    )
                LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}";

                    using (var command = new MySqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inmuebles.Add(new Inmueble
                            {
                                Id = reader.GetInt32("id"),
                                Direccion = reader.GetString("direccion"),
                                Tipo = reader.GetString("tipo"),
                                Estado = reader.GetString("estado"),
                                SuperficieM2 = reader.GetInt32("superficie_m2"),
                                Ambientes = reader.GetInt32("ambientes"),
                                Banos = reader.GetInt32("banos"),
                                Cochera = reader.GetInt32("cochera")
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error al obtener inmuebles sin contrato vigente: {e.Message}");
                    throw;
                }
            }

            return inmuebles;
        }
        public Contrato? ObtenerContratoVigentePorInmuebleId(int inmuebleId)
        {
            Contrato? contrato = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
            SELECT *
            FROM contrato
            WHERE inmueble_id = @inmuebleId
              AND estado = 'VIGENTE'
              AND fecha_fin >= CURDATE()
            LIMIT 1;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@inmuebleId", inmuebleId);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            contrato = new Contrato
                            {
                                Id = reader.GetInt32("id"),
                                InmuebleId = reader.GetInt32("inmueble_id"),
                                InquilinoId = reader.GetInt32("inquilino_id"),
                                FechaInicio = reader.GetDateTime("fecha_inicio"),
                                FechaFin = reader.GetDateTime("fecha_fin"),
                                Estado = reader.GetString("estado"),
                                MontoMensual = reader.GetDecimal("monto_mensual"),
                                Moneda = reader.GetString("moneda"),
                                Deposito = reader.IsDBNull(reader.GetOrdinal("deposito")) ? (decimal?)null : reader.GetDecimal("deposito"),
                                Observaciones = reader.IsDBNull(reader.GetOrdinal("observaciones")) ? null : reader.GetString("observaciones")
                            };
                        }
                    }
                }
            }
            return contrato;
        }

    }
}
