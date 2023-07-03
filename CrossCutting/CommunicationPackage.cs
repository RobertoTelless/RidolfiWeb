using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;
using Twilio.TwiML.Messaging;

namespace CrossCutting
{
    public static class CommunicationPackage
    {
        #region Metodos e-mail
        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static Int32 SendEmailVelha(Email email)
        {
            try
            {
                MailMessage mensagem = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                mensagem.From = new MailAddress(email.EMAIL_EMISSOR, email.NOME_EMISSOR);

                if (!string.IsNullOrEmpty(email.EMAIL_TO_DESTINO))
                {
                    mensagem.To.Add(email.EMAIL_TO_DESTINO);
                }

                if (!string.IsNullOrEmpty(email.EMAIL_CC_DESTINO))
                {
                    mensagem.CC.Add(email.EMAIL_CC_DESTINO);
                }

                if (!string.IsNullOrEmpty(email.EMAIL_BCC_DESTINO))
                {
                    mensagem.Bcc.Add(email.EMAIL_BCC_DESTINO);
                }

                mensagem.Subject = email.ASSUNTO;
                mensagem.IsBodyHtml = true;
                mensagem.Body = email.CORPO;
                mensagem.Priority = email.PRIORIDADE;
                mensagem.IsBodyHtml = true;
                if (email.ATTACHMENT != null)
                {
                    foreach (var attachment in email.ATTACHMENT)
                    {
                        mensagem.Attachments.Add(attachment);
                    }
                }
                smtp.EnableSsl = email.ENABLE_SSL;
                smtp.Port = Convert.ToInt32(email.PORTA);
                smtp.Host = email.SMTP;
                smtp.UseDefaultCredentials = email.DEFAULT_CREDENTIALS;
                smtp.Credentials = new System.Net.NetworkCredential(email.EMAIL_EMISSOR, email.SENHA_EMISSOR);
                smtp.Send(mensagem);
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static ControlError SendEmail(Email email)
        {
            ControlError controlError = new ControlError();

            try
            {
                string fromAddress = email.EMAIL_EMISSOR;
                string SubjectName = email.ASSUNTO;
                string AzureSendGridKey = email.SENHA_EMISSOR;
                               

                var message = new MailMessage();
                message.From = new MailAddress(
                   fromAddress,
                    SubjectName
                );

                if (!string.IsNullOrEmpty(email.EMAIL_TO_DESTINO))
                {
                    message.To.Add(email.EMAIL_TO_DESTINO);
                }

                if (!string.IsNullOrEmpty(email.EMAIL_CC_DESTINO))
                {
                    message.CC.Add(email.EMAIL_CC_DESTINO);
                }

                if (!string.IsNullOrEmpty(email.EMAIL_BCC_DESTINO))
                {
                    message.Bcc.Add(email.EMAIL_BCC_DESTINO);
                }

                message.Subject = email.ASSUNTO;
                message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(
                      email.CORPO, null, MediaTypeNames.Text.Html)
                );

                message.IsBodyHtml = true;
                if (email.ATTACHMENT != null)
                {
                    foreach (var attachment in email.ATTACHMENT)
                    {
                        message.Attachments.Add(attachment);
                    }
                }

                var client = new SmtpClient(email.SMTP, Convert.ToInt32(email.PORTA));
                client.EnableSsl = true;
                client.Credentials = email.NETWORK_CREDENTIAL;
                client.Send(message);                     
            }
            catch (Exception ex)
            {
                controlError.HandleExeption(ex);
            }

            return controlError;
        }

        public static ControlError SendEmailCollectionAsync(Email email, MailAddressCollection col)
        {
            ControlError controlError = new ControlError();

            try
            {
                var message = new MailMessage();
                message.From = new MailAddress(email.EMAIL_EMISSOR, email.NOME_EMISSOR);

                foreach(var e in col)
                {
                    message.To.Add(e);
                }

                message.Subject = email.ASSUNTO;

                message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(
                      email.CORPO, null, MediaTypeNames.Text.Html)
                );

                message.IsBodyHtml = true;
                if (email.ATTACHMENT != null)
                {
                    foreach (var attachment in email.ATTACHMENT)
                    {
                        message.Attachments.Add(attachment);
                    }
                }

                var client = new SmtpClient(host: email.SMTP, port: Convert.ToInt32(email.PORTA));
                client.Credentials = email.NETWORK_CREDENTIAL;
                client.Send(message);             
            }
            catch (Exception ex)
            {
                controlError.HandleExeption(ex);
            }

            return controlError;
        }

        /*public static Int32 SendEmailCollection(Email email, MailAddressCollection col)
        {
            try
            {
                return 0;
                MailMessage mensagem = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                mensagem.From = new MailAddress(email.EMAIL_EMISSOR, email.NOME_EMISSOR);
                foreach (var item in col)
                {
                    mensagem.To.Add(item);
                }
                mensagem.Subject = email.ASSUNTO;
                mensagem.IsBodyHtml = true;
                mensagem.Body = email.CORPO;
                mensagem.Priority = email.PRIORIDADE;
                mensagem.IsBodyHtml = true;
                if (email.ATTACHMENT != null)
                {
                    foreach (var attachment in email.ATTACHMENT)
                    {
                        mensagem.Attachments.Add(attachment);
                    }
                }
                smtp.EnableSsl = email.ENABLE_SSL;
                smtp.Port = Convert.ToInt32(email.PORTA);
                smtp.Host = email.SMTP;
                smtp.UseDefaultCredentials = email.DEFAULT_CREDENTIALS;
                smtp.Credentials = new System.Net.NetworkCredential(email.EMAIL_EMISSOR, email.SENHA_EMISSOR);
                smtp.Send(mensagem);
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }*/

        /// <summary>
        /// Sends the email assync.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static async Task SendEmailAssync(Email email)
        {
            try
            {
                MailMessage mensagem = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                mensagem.From = new MailAddress(email.EMAIL_EMISSOR, email.NOME_EMISSOR);
                mensagem.To.Add(email.EMAIL_TO_DESTINO);
                mensagem.Subject = email.ASSUNTO;
                mensagem.IsBodyHtml = true;
                mensagem.Body = email.CORPO;
                mensagem.Priority = email.PRIORIDADE;
                mensagem.IsBodyHtml = true;
                if (email.ATTACHMENT != null)
                {
                    foreach (var attachment in email.ATTACHMENT)
                    {
                        mensagem.Attachments.Add(attachment);
                    }
                }
                smtp.EnableSsl = email.ENABLE_SSL;
                smtp.Port = Convert.ToInt32(email.PORTA);
                smtp.Host = email.SMTP;
                smtp.UseDefaultCredentials = email.DEFAULT_CREDENTIALS;
                smtp.Credentials = new System.Net.NetworkCredential(email.EMAIL_EMISSOR, email.SENHA_EMISSOR);
                await smtp.SendMailAsync(mensagem);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region Métodos SMS
               
        public static ControlError SendMensageSMS(SMS sms)
        {
            ControlError controlError = new ControlError();

            // Monta token            
            String text = sms.LOGIN_SMS + ":" + sms.PASSWORD_SMS;
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            String token = Convert.ToBase64String(textBytes);
            String auth = "Basic " + token;

            // Prepara texto
            String texto = sms.SMS_CORPO;
            String link = sms.SMS_LINK;            

            // Prepara corpo do SMS e trata link
            StringBuilder str = new StringBuilder();
            str.AppendLine(texto);
            if (!String.IsNullOrEmpty(link))
            {
                if (!link.Contains("www."))
                {
                    link = "www." + link;
                }
                if (!link.Contains("http://"))
                {
                    link = "http://" + link;
                }
                str.AppendLine(link + " Clique aqui para maiores informações");
                texto += "  " + link;
            }
            String body = str.ToString();
            body = body.Replace("\r\n", " ");
            String smsBody = body;

            // inicia processo
            String resposta = String.Empty;

            try
            {
                // Monta Array de envio
                String vetor = String.Empty;
                foreach (string cli in sms.LISTA_DESTINATARIO_CEL)
                {
                    String listaDest = "55" + Regex.Replace(cli, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                    String customId = Cryptography.GenerateRandomPassword(8);
                    if (vetor == String.Empty)
                    {
                        vetor = "{\"to\": \"," + listaDest + ", \", \"text\": \"," + texto + "\", \"from\": \"ERPSys\"}";
                    }
                    else
                    {
                        vetor += ",{\"to\": \"," + listaDest + ", \", \"text\": \"," + texto + "\", \"from\": \"ERPSys\"}";
                    }
                }

                // Configura                    
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                httpWebRequest.Headers["Authorization"] = auth;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                String data = String.Empty;
                String json = String.Empty;

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    if(sms.DT_AGENDAMENTO == null)
                    {
                        json = String.Concat("{\"destinations\": [", vetor, "]}");
                    }
                    else
                    {
                        data = sms.DT_AGENDAMENTO.Value.Year.ToString() + "-" + sms.DT_AGENDAMENTO.Value.Month.ToString() + "-" + sms.DT_AGENDAMENTO.Value.Day.ToString() + "T" + sms.DT_AGENDAMENTO.Value.ToShortTimeString() + ":00";                        

                        json = String.Concat("{\"scheduleTime\": \"", data, "\",\"destinations\": [", vetor, "]}");
                    }                    
                    
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    resposta = result;
                }
            }
            catch(Exception ex)
            {
                controlError.HandleExeption(ex);
            }

            return controlError;
        }

        #endregion

        //public static async Task<ControlError> SendMailAsync(Email email)
        //{
        //    ControlError controlError = new ControlError();

        //    try
        //    {
        //        string fromAddress = email.EMAIL_EMISSOR;
        //        string toAddress = email.EMAIL_TO_DESTINO;
        //        string SubjectName = email.ASSUNTO;
        //        string AzureSendGridKey = email.SENHA_EMISSOR;
        //        string corpo = email.CORPO;

        //        var message = new MailMessage();
        //        message.From = new MailAddress(
        //           fromAddress,
        //            SubjectName
        //        );

        //        if (!string.IsNullOrEmpty(email.EMAIL_TO_DESTINO))
        //        {
        //            message.To.Add(email.EMAIL_TO_DESTINO);
        //        }

        //        if (!string.IsNullOrEmpty(email.EMAIL_CC_DESTINO))
        //        {
        //            message.CC.Add(email.EMAIL_CC_DESTINO);
        //        }

        //        if (!string.IsNullOrEmpty(email.EMAIL_BCC_DESTINO))
        //        {
        //            message.Bcc.Add(email.EMAIL_BCC_DESTINO);
        //        }

        //        message.Subject = email.ASSUNTO;
        //        message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(
        //              email.CORPO, null, MediaTypeNames.Text.Html)
        //        );

        //        message.IsBodyHtml = true;
        //        if (email.ATTACHMENT != null)
        //        {
        //            foreach (var attachment in email.ATTACHMENT)
        //            {
        //                message.Attachments.Add(attachment);
        //            }
        //        }

        //        var client = new SmtpClient(email.SMTP, Convert.ToInt32(email.PORTA));
        //        client.EnableSsl = true;
        //        client.Credentials = email.NETWORK_CREDENTIAL;
        //        object token = null;
        //        client.SendAsync(message, token);
        //    }
        //    catch (Exception ex)
        //    {
        //        controlError.HandleExeption(ex);
        //    }

        //    return controlError;







        //    EmailClient emailClient = ConnectMail(mail.ConnectionString);

        //    // Envia mensagem
        //    var emailSendOperation = await emailClient.SendAsync(
        //        wait: WaitUntil.Started,
        //        senderAddress: mail.NOME_EMISSOR_AZURE,
        //        recipientAddress: mail.EMAIL_TO_DESTINO,
        //        subject: mail.ASSUNTO,
        //        htmlContent: mail.CORPO);

        //    // Verifica
        //    try
        //    {
        //        while (true)
        //        {
        //            await emailSendOperation.UpdateStatusAsync();
        //            if (emailSendOperation.HasCompleted)
        //            {
        //                break;
        //            }
        //            await Task.Delay(100);
        //        }
        //        string operationId = emailSendOperation.Id;

        //        if (emailSendOperation.HasValue)
        //        {
        //            EmailSendStatus status = emailSendOperation.Value.Status;
        //            var tupla = Tuple.Create(status, operationId);
        //            return tupla;
        //        }
        //        else
        //        {
        //            EmailSendStatus status = null;
        //            var tupla = Tuple.Create(status, operationId);
        //            return tupla;

        //        }
        //    }
        //    catch (RequestFailedException ex)
        //    {
        //        throw ex;
        //    }
        //}

    }
}
