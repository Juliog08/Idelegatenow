using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ProjectJG.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio.Exceptions;
using ProjectJG.Services;
using System.Threading.Tasks;

namespace ProjectJG
{
    public sealed partial class MainPage : Page
    {
        private readonly TwilioService _twilioService;
        private readonly DatabaseService _databaseService;

        public MainPage()
        {
            this.InitializeComponent();

            string connectionString = "Server=LAPTOP-LUF8IA2P\\SQLEXPRESS;Initial Catalog=IDELEGATENOW;User Id=sa;Password=12345;Encrypt=False;TrustServerCertificate=True;";
            _twilioService = new TwilioService("+12087827926");
            _databaseService = new DatabaseService(connectionString);

            CargarMensajesEnviados();
        }

        private async void EnviarMensaje_Click(object sender, RoutedEventArgs e)
        {
            string numero = txtPara.Text;
            string mensaje = txtMensaje.Text;

            if (string.IsNullOrWhiteSpace(numero) || string.IsNullOrWhiteSpace(mensaje))
            {
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Por favor, complete todos los campos.",
                    CloseButtonText = "Ok"
                };
                await dialog.ShowAsync();
                return;
            }

            try
            {
                // Obtener las credenciales de Twilio desde la base de datos
                var (accountSid, authToken, PhoneNumber) = await _databaseService.ObtenerCredencialesTwilioAsync();

                // Guardar el mensaje en la base de datos
                var nuevoMensaje = new Mensaje
                {
                    FechaHoraCreacion = DateTime.Now,
                    Al = numero,
                    MensajeTexto = mensaje
                };

                int mensajeId = await _databaseService.GuardarMensajeAsync(nuevoMensaje);

                // Enviar el mensaje a través de Twilio
                string codigoConfirmacion = await _twilioService.EnviarMensajeAsync(numero, mensaje, accountSid, authToken);

                // Guardar la confirmación en la base de datos
                var nuevoEnvio = new Envio
                {
                    MensajeId = mensajeId,
                    FechaHoraEnvio = DateTime.Now,
                    CodigoConfirmacionTwilio = codigoConfirmacion
                };

                await _databaseService.GuardarEnvioAsync(nuevoEnvio);

                // Recargar la lista de mensajes
                await CargarMensajesEnviados();
            }
            catch (Exception ex)
            {
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"Ocurrió un error: {ex.Message}",
                    CloseButtonText = "Ok"
                };
                await dialog.ShowAsync();
            }
        }

        private async Task CargarMensajesEnviados()
        {
            try
            {
                var mensajes = await _databaseService.CargarMensajesAsync();
                listaMensajes.ItemsSource = mensajes;
            }
            catch (Exception ex)
            {
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"Ocurrió un error al cargar los mensajes: {ex.Message}",
                    CloseButtonText = "Ok"
                };
                await dialog.ShowAsync();
            }
        }
    }
}





