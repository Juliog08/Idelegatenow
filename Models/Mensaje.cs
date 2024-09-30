using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectJG.Models
{

    public class Mensaje
    {
        public int Id { get; set; }
        public DateTime FechaHoraCreacion { get; set; }
        public string Al { get; set; }
        public string MensajeTexto { get; set; }
    }
}
