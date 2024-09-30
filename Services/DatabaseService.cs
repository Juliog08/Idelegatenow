using ProjectJG.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;

namespace ProjectJG.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> GuardarMensajeAsync(Mensaje mensaje)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "INSERT INTO Mensajes (FechaHoraCreacion, Al, Mensaje) OUTPUT INSERTED.Id VALUES (@FechaHoraCreacion, @Al, @Mensaje)";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@FechaHoraCreacion", mensaje.FechaHoraCreacion);
                    cmd.Parameters.AddWithValue("@Al", mensaje.Al);
                    cmd.Parameters.AddWithValue("@Mensaje", mensaje.MensajeTexto);
                    return (int)await cmd.ExecuteScalarAsync();
                }
            }
        }

        public async Task GuardarEnvioAsync(Envio envio)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "INSERT INTO Envios (MensajeId, FechaHoraEnvio, CodigoConfirmacionTwilio) VALUES (@MensajeId, @FechaHoraEnvio, @CodigoConfirmacionTwilio)";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@MensajeId", envio.MensajeId);
                    cmd.Parameters.AddWithValue("@FechaHoraEnvio", envio.FechaHoraEnvio);
                    cmd.Parameters.AddWithValue("@CodigoConfirmacionTwilio", envio.CodigoConfirmacionTwilio);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<Mensaje>> CargarMensajesAsync()
        {
            var mensajes = new List<Mensaje>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Al, Mensaje, FechaHoraCreacion FROM Mensajes ORDER BY FechaHoraCreacion DESC";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            mensajes.Add(new Mensaje
                            {
                                Al = reader["Al"].ToString(),
                                MensajeTexto = reader["Mensaje"].ToString(),
                                FechaHoraCreacion = (DateTime)reader["FechaHoraCreacion"]
                            });
                        }
                    }
                }
            }

            return mensajes;
        }

        // Método para obtener las credenciales de Twilio desde la base de datos
        public async Task<(string AccountSid, string AuthToken, string PhoneNumber)> ObtenerCredencialesTwilioAsync()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT TOP 1 AccountSid, AuthToken, fromPhoneNumber FROM TwilioCredentials";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string accountSid = reader["AccountSid"].ToString();
                            string authToken = reader["AuthToken"].ToString();
                            string phoneNumber = reader["fromPhoneNumber"].ToString();
                            return (accountSid, authToken, phoneNumber);
                        }
                    }
                }
            }
            throw new Exception("No se encontraron credenciales de Twilio en la base de datos.");
        }
    }
}
