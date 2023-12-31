using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting
{
    public class EmailAzure
    {
        public string SMTP { get; set; }
        public string PORTA { get; set; }
        public string EMAIL_EMISSOR { get; set; }
        public string SENHA_EMISSOR { get; set; }
        public string NOME_EMISSOR { get; set; }
        public string EMAIL_TO_DESTINO { get; set; }
        public string EMAIL_CC_DESTINO { get; set; }
        public string EMAIL_BCC_DESTINO { get; set; }
        public string ASSUNTO { get; set; }
        public string CORPO { get; set; }
        public MailPriority PRIORIDADE { get; set; }
        public Boolean ENABLE_SSL { get; set; }
        public Boolean DEFAULT_CREDENTIALS { get; set; }
        public NetworkCredential NETWORK_CREDENTIAL { get; set; }
        public List<Attachment> ATTACHMENT { get; set; }
        public Boolean IS_HTML { get; set; }
        public String ConnectionString { get; set; }
        public String EndPoint { get; set; }
        public string NOME_EMISSOR_AZURE { get; set; }
        public string DISPLAY_NAME { get; set; }

        public List<AttachmentModel> Attachments { get; set; }


    }
}
