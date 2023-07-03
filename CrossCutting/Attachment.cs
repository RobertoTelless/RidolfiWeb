using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting
{
    public class AttachmentModel
    {
        public string PATH { get; set; }
        public string ATTACHMENT_NAME { get; set; }
        public string CONTENT_TYPE { get; set; }
        public List<AttachmentModel> Attachments { get; set; }
    }
}
