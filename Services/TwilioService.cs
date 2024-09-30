using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace ProjectJG.Services
{
    public class TwilioService
    {
        private readonly string _fromPhoneNumber;
        private readonly string _accountSid;
        private readonly string _authToken;

        public TwilioService(string fromPhoneNumber)
        {
            _fromPhoneNumber = fromPhoneNumber;
            //_accountSid = accountSid;
            //_authToken = authToken;
        }

        public async Task<string> EnviarMensajeAsync(string numero, string mensaje, string accountSid, string authToken)
        {
            if (!numero.StartsWith("+503"))
            {
                numero = "+503" + numero;
            }

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                    body: mensaje,
                    from: new PhoneNumber(_fromPhoneNumber),
                    to: new PhoneNumber(numero)
            );

            return message.Sid;
        }
    }

}
