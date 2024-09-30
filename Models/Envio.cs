using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectJG.Models
{
    public class Envio
    {
        public int Id { get; set; }
        public int MensajeId { get; set; }
        public DateTime FechaHoraEnvio { get; set; }
        public string CodigoConfirmacionTwilio { get; set; }
    }
}
