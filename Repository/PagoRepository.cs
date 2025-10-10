using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Inmobilaria_lab2_TPI_MGS.Models;

namespace Inmobilaria_lab2_TPI_MGS.Repository
{
    public class PagoRepository : BaseRepository
    {
        public PagoRepository() : base()
        {
        }

        public bool Alta(Pago pago, int? idUsuario)
        {
            bool exito = false;

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"
                        INSERT INTO pago 
                        (contrato_id, estado, periodo_anio, periodo_mes, fecha_vencimiento, importe, recargo, descuento,
                         importe_pagado, fecha_pago, medio_pago, observaciones, fecha_creacion, fecha_modificacion, creado_por)
                        VALUES 
                        (@ContratoId, @Estado, @PeriodoAnio, @PeriodoMes, @FechaVencimiento, @Importe, @Recargo, @Descuento,
                         @ImportePagado, @FechaPago, @MedioPago, @Observaciones, NOW(), NOW(), @CreadoPor);";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ContratoId", pago.ContratoId);
                        command.Parameters.AddWithValue("@Estado", pago.Estado ?? "PENDIENTE");
                        command.Parameters.AddWithValue("@PeriodoAnio", pago.PeriodoAnio);
                        command.Parameters.AddWithValue("@PeriodoMes", pago.PeriodoMes);
                        command.Parameters.AddWithValue("@FechaVencimiento", pago.FechaVencimiento);
                        command.Parameters.AddWithValue("@Importe", pago.Importe);
                        command.Parameters.AddWithValue("@Recargo", pago.Recargo);
                        command.Parameters.AddWithValue("@Descuento", pago.Descuento);
                        command.Parameters.AddWithValue("@ImportePagado", (object?)pago.ImportePagado ?? DBNull.Value);
                        command.Parameters.AddWithValue("@FechaPago", (object?)pago.FechaPago ?? DBNull.Value);
                        command.Parameters.AddWithValue("@MedioPago", (object?)pago.MedioPago ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Observaciones", (object?)pago.Observaciones ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CreadoPor", idUsuario.HasValue ? idUsuario.Value : (object)DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();
                        exito = rowsAffected > 0;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error al insertar pago: {e.Message}");
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }

            return exito;
        }

        public bool Modificar(Pago pago, int? idUsuario)
        {
            bool exito = false;

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    var updates = new List<string>();
                    var parameters = new List<MySqlParameter>();

                    updates.Add("estado = @Estado");
                    parameters.Add(new MySqlParameter("@Estado", pago.Estado ?? "PENDIENTE"));

                    if (pago.ImportePagado.HasValue)
                    {
                        updates.Add("importe_pagado = @ImportePagado");
                        parameters.Add(new MySqlParameter("@ImportePagado", pago.ImportePagado.Value));
                    }

                    if (pago.FechaPago.HasValue)
                    {
                        updates.Add("fecha_pago = @FechaPago");
                        parameters.Add(new MySqlParameter("@FechaPago", pago.FechaPago.Value));
                    }

                    if (!string.IsNullOrWhiteSpace(pago.MedioPago))
                    {
                        updates.Add("medio_pago = @MedioPago");
                        parameters.Add(new MySqlParameter("@MedioPago", pago.MedioPago));
                    }

                    if (!string.IsNullOrWhiteSpace(pago.Observaciones))
                    {
                        updates.Add("observaciones = @Observaciones");
                        parameters.Add(new MySqlParameter("@Observaciones", pago.Observaciones));
                    }

                    updates.Add("recargo = @Recargo");
                    parameters.Add(new MySqlParameter("@Recargo", pago.Recargo));
                    updates.Add("descuento = @Descuento");
                    parameters.Add(new MySqlParameter("@Descuento", pago.Descuento));

                    updates.Add("importe = @Importe");
                    parameters.Add(new MySqlParameter("@Importe", pago.Importe));

                    updates.Add("fecha_modificacion = NOW()");
                    updates.Add("modificado_por = @ModificadoPor");
                    parameters.Add(new MySqlParameter("@ModificadoPor", idUsuario.HasValue ? idUsuario.Value : (object)DBNull.Value));


                    string query = $@"
                        UPDATE pago 
                        SET {string.Join(", ", updates)}
                        WHERE id = @Id";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", pago.Id);
                        command.Parameters.AddRange(parameters.ToArray());

                        int rowsAffected = command.ExecuteNonQuery();
                        exito = rowsAffected > 0;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error al modificar pago: {e.Message}");
                    throw;
                }
            }

            return exito;
        }

        public bool Baja(int pagoId, int? idUsuario)
        {
            bool exito = false;

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"
                        UPDATE pago
                        SET estado = 'CANCELADO',
                            fecha_modificacion = NOW(),
                            modificado_por = @ModificadoPor
                        WHERE id = @Id;";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", pagoId);
                        command.Parameters.AddWithValue("@ModificadoPor", idUsuario.HasValue ? idUsuario.Value : (object)DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();
                        exito = rowsAffected > 0;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error al eliminar pago: {e.Message}");
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }

            return exito;
        }

        public Pago ObtenerPorId(ulong pagoId)
        {
            Pago? pago = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"
                        SELECT * FROM pago
                        WHERE id = @Id
                        LIMIT 1;";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", pagoId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                pago = new Pago
                                {
                                    Id = (ulong)reader.GetInt32("id"),
                                    ContratoId = (int)reader.GetInt32("contrato_id"),
                                    Estado = reader.GetString("estado"),
                                    PeriodoAnio = reader.GetInt16("periodo_anio"),
                                    PeriodoMes = reader.GetByte("periodo_mes"),
                                    FechaVencimiento = reader.GetDateTime("fecha_vencimiento"),
                                    Importe = reader.GetDecimal("importe"),
                                    Recargo = reader.GetDecimal("recargo"),
                                    Descuento = reader.GetDecimal("descuento"),
                                    ImportePagado = reader.IsDBNull(reader.GetOrdinal("importe_pagado")) ? null : reader.GetDecimal("importe_pagado"),
                                    FechaPago = reader.IsDBNull(reader.GetOrdinal("fecha_pago")) ? null : reader.GetDateTime("fecha_pago"),
                                    MedioPago = reader.IsDBNull(reader.GetOrdinal("medio_pago")) ? null : reader.GetString("medio_pago"),
                                    Observaciones = reader.IsDBNull(reader.GetOrdinal("observaciones")) ? null : reader.GetString("observaciones"),
                                    FechaCreacion = reader.GetDateTime("fecha_creacion"),
                                    FechaModificacion = reader.GetDateTime("fecha_modificacion"),
                                    CreadoPor = reader.IsDBNull(reader.GetOrdinal("creado_por")) ? null : (ulong?)reader.GetInt32("creado_por"),
                                    ModificadoPor = reader.IsDBNull(reader.GetOrdinal("modificado_por")) ? null : (ulong?)reader.GetInt32("modificado_por")
                                };
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error al obtener pago: {e.Message}");
                    throw;
                }
            }

            return pago!;
        }

        public IList<Pago> ListarPorContrato(int contratoId)
        {
            IList<Pago> pagos = new List<Pago>();

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"
                        SELECT * 
                        FROM pago
                        WHERE contrato_id = @ContratoId
                        ORDER BY periodo_anio, periodo_mes;";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ContratoId", contratoId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var pago = new Pago
                                {
                                    Id = (ulong)reader.GetInt32("id"),
                                    ContratoId = (int)reader.GetInt32("contrato_id"),
                                    Estado = reader.GetString("estado"),
                                    PeriodoAnio = reader.GetInt16("periodo_anio"),
                                    PeriodoMes = reader.GetByte("periodo_mes"),
                                    FechaVencimiento = reader.GetDateTime("fecha_vencimiento"),
                                    Importe = reader.GetDecimal("importe"),
                                    Recargo = reader.GetDecimal("recargo"),
                                    Descuento = reader.GetDecimal("descuento"),
                                    ImportePagado = reader.IsDBNull(reader.GetOrdinal("importe_pagado")) ? null : reader.GetDecimal("importe_pagado"),
                                    FechaPago = reader.IsDBNull(reader.GetOrdinal("fecha_pago")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("fecha_pago")).Date,
                                    MedioPago = reader.IsDBNull(reader.GetOrdinal("medio_pago")) ? null : reader.GetString("medio_pago"),
                                    Observaciones = reader.IsDBNull(reader.GetOrdinal("observaciones")) ? null : reader.GetString("observaciones"),
                                    FechaCreacion = reader.GetDateTime("fecha_creacion"),
                                    FechaModificacion = reader.GetDateTime("fecha_modificacion"),
                                };

                                pagos.Add(pago);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error al obtener pagos por contrato: {e.Message}");
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }

            return pagos;
        }

        public bool ActualizarEstado(ulong pagoId, string nuevoEstado)
        {
            bool exito = false;

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"
                        UPDATE pago
                        SET estado = @NuevoEstado,
                            fecha_modificacion = NOW()
                        WHERE id = @Id;";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", pagoId);
                        command.Parameters.AddWithValue("@NuevoEstado", nuevoEstado);

                        int rowsAffected = command.ExecuteNonQuery();
                        exito = rowsAffected > 0;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error al actualizar estado del pago: {e.Message}");
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }

            return exito;
        }
    }
}
