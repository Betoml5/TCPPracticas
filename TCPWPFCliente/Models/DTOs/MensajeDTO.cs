using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPWPFCliente.Models.DTOs
{
    public class MensajeDTO
    {
        public string Mensaje { get; set; } = null!;
        public string Origen { get; set; } = null!;
        public DateTime Fecha { get; set; }
    }
}
