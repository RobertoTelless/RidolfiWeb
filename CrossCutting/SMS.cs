using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting
{
    public class SMS
    {
        public string LOGIN_SMS { get; set; }
        public string PASSWORD_SMS { get; set; }
        public string URL_PROVEDOR_SMS { get; set; }
        public string SMS_CORPO { get; set; }
        public string SMS_LINK { get; set; }
        public DateTime? DT_AGENDAMENTO { get; set; }
        public List<string> LISTA_DESTINATARIO_CEL { get; set; }
    }
}
