using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesServices.WorkClasses
{
    public class Enumerador
    {
        public enum StatusEnvioEmail
        {
            AGUARDANDO_ENVIO = 1
            ,ENVIADO = 2
            ,ERRO = 3
        }

        public enum MensagemTipo
        {
            EMAIL = 1
            , SMS = 2
            , WHATSAPP = 3
        }
    }
}
