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
//using SendGrid;
//using SendGrid.Helpers.Mail;
//using Twilio.TwiML.Messaging;
using Azure.Communication.Email;
using Azure.Core;
using Azure.Identity;
using Azure;
using System.Threading;

namespace CrossCutting
{
    public static class CommunicationAzurePackage
    {
        public static EmailClient ConnectMail(String connString)
        {          
            EmailClient emailClient = new EmailClient(connString);
            return emailClient;
        }

        public static EmailClient ConnectMailEndPoint(String endPoint)
        {
            TokenCredential tokenCredential = new DefaultAzureCredential();
            tokenCredential = new DefaultAzureCredential();
            EmailClient emailClient = new EmailClient(new Uri(endPoint), tokenCredential);
            return emailClient;
        }

        public static Tuple < EmailSendStatus, String, Boolean > SendMail(EmailAzure mail, List<AttachmentModel> anexos)
        {
            // Conecta serviço
            EmailClient emailClient = ConnectMail(mail.ConnectionString);
            
            try
            {
                // Monta mensagem
                var emailContent = new EmailContent(mail.ASSUNTO)
                {
                    PlainText = mail.CORPO,
                    Html = mail.CORPO
                };

                // Checa destinos
                List<EmailAddress> toRecipients = new List<EmailAddress>();
                EmailAddress add = new EmailAddress(address: mail.EMAIL_TO_DESTINO, displayName: mail.DISPLAY_NAME);
                toRecipients.Add(add);

                var emailRecipients = new EmailRecipients(toRecipients);

                var emailMessage = new EmailMessage(
                    senderAddress: mail.NOME_EMISSOR_AZURE,
                    emailRecipients,
                    emailContent);

                // Checa anexos
                if (anexos != null)
                {
                    if (anexos.Count > 0)
                    {
                        foreach (AttachmentModel anexo in anexos)
                        {
                            var contentType = anexo.CONTENT_TYPE;
                            var content = new BinaryData(System.IO.File.ReadAllBytes(anexo.PATH));
                            var emailAttachment = new EmailAttachment(anexo.ATTACHMENT_NAME, contentType, content);
                            emailMessage.Attachments.Add(emailAttachment);
                        }
                    }
                }

                //Envia mensagem
                var emailSendOperation = emailClient.Send(
                    wait: WaitUntil.Completed,
                    message: emailMessage);

                EmailSendStatus status = emailSendOperation.Value.Status;
                String operationId = emailSendOperation.Id;

                // Monta retorno
                var tupla = Tuple.Create(status, operationId, true);
                return tupla;
            }
            catch (RequestFailedException ex)
            {
                throw ex;
            }
        }

        public static Tuple<EmailSendStatus, String, Boolean> SendMailList(EmailAzure mail, List<AttachmentModel> anexos, List<EmailAddress> nomes)
        {
            // Conecta serviço
            EmailClient emailClient = ConnectMail(mail.ConnectionString);

            try
            {
                // Monta mensagem
                var emailContent = new EmailContent(mail.ASSUNTO)
                {
                    PlainText = mail.CORPO,
                    Html = mail.CORPO
                };

                // Checa destinos
                List<EmailAddress> toRecipients = new List<EmailAddress>();
                if (nomes != null)
                {
                    if (nomes.Count > 0)
                    {
                        foreach (EmailAddress nome in nomes)
                        {
                            toRecipients.Add(nome);
                        }
                    }
                }
                else
                {
                    EmailAddress add = new EmailAddress(address: mail.EMAIL_TO_DESTINO, displayName: mail.DISPLAY_NAME);
                    toRecipients.Add(add);
                }

                var emailRecipients = new EmailRecipients(toRecipients);

                var emailMessage = new EmailMessage(
                    senderAddress: mail.NOME_EMISSOR_AZURE,
                    emailRecipients,
                    emailContent);

                // Checa anexos
                if (anexos != null)
                {
                    if (anexos.Count > 0)
                    {
                        foreach (AttachmentModel anexo in anexos)
                        {
                            var contentType = anexo.CONTENT_TYPE;
                            var content = new BinaryData(System.IO.File.ReadAllBytes(anexo.PATH));
                            var emailAttachment = new EmailAttachment(anexo.ATTACHMENT_NAME, contentType, content);
                            emailMessage.Attachments.Add(emailAttachment);
                        }
                    }
                }

                //Envia mensagem
                var emailSendOperation = emailClient.Send(
                    wait: WaitUntil.Completed,
                    message: emailMessage);

                EmailSendStatus status = emailSendOperation.Value.Status;
                String operationId = emailSendOperation.Id;

                // Monta retorno
                var tupla = Tuple.Create(status, operationId, true);
                return tupla;
            }
            catch (RequestFailedException ex)
            {
                throw ex;
            }
        }

        public static async Task<Tuple<EmailSendStatus, String>> SendMailAsync(EmailAzure mail)
        {
            // Conecta serviço
            EmailClient emailClient = ConnectMail(mail.ConnectionString);

            // Monta mensagem
            var emailContent = new EmailContent(mail.ASSUNTO)
            {
                PlainText = mail.CORPO,
                Html = mail.CORPO
            };

            List<EmailAddress> toRecipients = new List<EmailAddress>();
            EmailAddress add = new EmailAddress(address: mail.EMAIL_TO_DESTINO, displayName: mail.DISPLAY_NAME);
            toRecipients.Add(add);
            var emailRecipients = new EmailRecipients(toRecipients);

            var emailMessage = new EmailMessage(
                senderAddress: mail.NOME_EMISSOR_AZURE,
                emailRecipients,
                emailContent);

            // Checa anexos
            if (mail.Attachments != null)
            {
                if (mail.Attachments.Count > 0)
                {
                    foreach (AttachmentModel anexo in mail.Attachments)
                    {
                        var contentType = anexo.CONTENT_TYPE;
                        var content = new BinaryData(System.IO.File.ReadAllBytes(anexo.PATH));
                        var emailAttachment = new EmailAttachment(anexo.ATTACHMENT_NAME, contentType, content);
                        emailMessage.Attachments.Add(emailAttachment);
                    }
                }
            }

            // Envia Mensagem
            var emailSendOperation = await emailClient.SendAsync(
                wait: WaitUntil.Started,
                message: emailMessage);
            EmailSendStatus status = null;
            var tupla = Tuple.Create(status, "xxx");
            return tupla;

            // Verifica
            //try
            //{
            //    while (true)
            //    {
            //        await emailSendOperation.UpdateStatusAsync();
            //        if (emailSendOperation.HasCompleted)
            //        {
            //            break;
            //        }
            //        await Task.Delay(100);
            //    }
            //    string operationId = emailSendOperation.Id;

            //    if (emailSendOperation.HasValue)
            //    {
            //        EmailSendStatus status = emailSendOperation.Value.Status;
            //        var tupla = Tuple.Create(status, operationId);
            //        return tupla;
            //    }
            //    else
            //    {
            //        EmailSendStatus status = null;
            //        var tupla = Tuple.Create(status, operationId);
            //        return tupla;

            //    }
            //}
            //catch (RequestFailedException ex)
            //{
            //    throw ex;
            //}
        }

        public static async Task<Dictionary<String, EmailSendStatus>> SendMailAsyncList(List<EmailAzure> lista)
        {
            // Conecta serviço
            Dictionary<String, EmailSendStatus> dict = new Dictionary<String, EmailSendStatus>();
            EmailClient emailClient = null;
            if (lista.Count > 0)
            {
                EmailAzure mail = lista.FirstOrDefault();   
                emailClient = ConnectMail(mail.ConnectionString);
            }
            else
            {
                EmailSendStatus status = null;
                dict.Add(String.Empty, status);
                return dict;
            }

            // Processa lista
            foreach (EmailAzure mail in lista)
            {
                // Envia mensagem
                var emailSendOperation = await emailClient.SendAsync(
                    wait: WaitUntil.Started,
                    senderAddress: mail.EMAIL_EMISSOR,
                    recipientAddress: mail.EMAIL_TO_DESTINO,
                    subject: mail.ASSUNTO,
                    htmlContent: mail.CORPO);

                // Verifica
                try
                {
                    while (true)
                    {
                        await emailSendOperation.UpdateStatusAsync();
                        if (emailSendOperation.HasCompleted)
                        {
                            break;
                        }
                        await Task.Delay(100);
                    }
                    string operationId = emailSendOperation.Id;

                    if (emailSendOperation.HasValue)
                    {
                        EmailSendStatus status = emailSendOperation.Value.Status;
                        dict.Add(operationId, status);
                    }
                }
                catch (RequestFailedException ex)
                {
                    throw ex;
                }
            }
            return dict;
        }

        public static async Task<Dictionary<String, EmailSendStatus>> SendMailAsyncListAvancado(List<EmailAzure> lista)
        {
            // Conecta serviço
            Dictionary<String, EmailSendStatus> dict = new Dictionary<String, EmailSendStatus>();
            EmailClient emailClient = null;
            String assunto = null;
            String corpo = null;
            String sender = null;
            List<AttachmentModel> anexos = new List<AttachmentModel>();
            if (lista.Count > 0)
            {
                EmailAzure mail = lista.FirstOrDefault();
                emailClient = ConnectMail(mail.ConnectionString);
                assunto = mail.ASSUNTO;
                corpo = mail.CORPO;
                sender = mail.NOME_EMISSOR_AZURE;
                anexos = mail.Attachments;
            }
            else
            {
                EmailSendStatus status = null;
                dict.Add(String.Empty, status);
                return dict;
            }

            // Processa lista e monta mensagem
            var emailContent = new EmailContent(assunto)
            {
                PlainText = corpo,
                Html = corpo
            };

            List<EmailAddress> toRecipients = new List<EmailAddress>();
            foreach (EmailAzure item in lista)
            {
                EmailAddress add = new EmailAddress(address: item.EMAIL_TO_DESTINO, displayName: item.DISPLAY_NAME);
                toRecipients.Add(add);
            }
            var emailRecipients = new EmailRecipients(toRecipients);
            var emailMessage = new EmailMessage(
                senderAddress: sender,
                emailRecipients,
                emailContent);

            // Checa anexos
            if (anexos.Count > 0)
            {
                foreach (AttachmentModel anexo in anexos)
                {
                    var contentType = anexo.CONTENT_TYPE;
                    var content = new BinaryData(System.IO.File.ReadAllBytes(anexo.PATH));
                    var emailAttachment = new EmailAttachment(anexo.ATTACHMENT_NAME, contentType, content);
                    emailMessage.Attachments.Add(emailAttachment);
                }
            }

            // Envia mensagem
            var emailSendOperation = await emailClient.SendAsync(
                wait: WaitUntil.Started,
                message: emailMessage);

            // Verifica
            try
            {
                while (true)
                {
                    await emailSendOperation.UpdateStatusAsync();
                    if (emailSendOperation.HasCompleted)
                    {
                        break;
                    }
                    await Task.Delay(100);
                }
                string operationId = emailSendOperation.Id;
                if (emailSendOperation.HasValue)
                {
                    EmailSendStatus status = emailSendOperation.Value.Status;
                    dict.Add(operationId, status);
                }
                else
                {
                    EmailSendStatus status = null;
                    dict.Add(String.Empty, status);
                    return dict;
                }
            }
            catch (RequestFailedException ex)
            {
                throw ex;
            }
            return dict;
        }

        public static Tuple < EmailSendStatus, String, Boolean > SendMailListAvancado(List<EmailAzure> lista)
        {
            // Preparacao
            Dictionary<String, EmailSendStatus> dict = new Dictionary<String, EmailSendStatus>();
            EmailClient emailClient = null;
            String assunto = null;
            String corpo = null;
            String sender = null;
            List<AttachmentModel> anexos = new List<AttachmentModel>();
            EmailSendStatus status = null;

            // Verifica lista e conecta serviço
            if (lista.Count > 0)
            {
                EmailAzure mail = lista.FirstOrDefault();
                emailClient = ConnectMail(mail.ConnectionString);
                assunto = mail.ASSUNTO;
                corpo = mail.CORPO;
                sender = mail.NOME_EMISSOR_AZURE;
                anexos = mail.Attachments;
            }
            else
            {
                var tuplaVolta = Tuple.Create(status, String.Empty, true);
                return tuplaVolta;
            }

            // Processa lista e monta mensagem
            var emailContent = new EmailContent(assunto)
            {
                PlainText = corpo,
                Html = corpo
            };

            List<EmailAddress> toRecipients = new List<EmailAddress>();
            foreach (EmailAzure item in lista)
            {
                EmailAddress add = new EmailAddress(address: item.EMAIL_TO_DESTINO, displayName: item.DISPLAY_NAME);
                toRecipients.Add(add);
            }
            var emailRecipients = new EmailRecipients(toRecipients);
            var emailMessage = new EmailMessage(
                senderAddress: sender,
                emailRecipients,
                emailContent);

            // Checa anexos
            if (anexos.Count > 0)
            {
                foreach (AttachmentModel anexo in anexos)
                {
                    var contentType = anexo.CONTENT_TYPE;
                    var content = new BinaryData(System.IO.File.ReadAllBytes(anexo.PATH));
                    var emailAttachment = new EmailAttachment(anexo.ATTACHMENT_NAME, contentType, content);
                    emailMessage.Attachments.Add(emailAttachment);
                }
            }

            // Envia mensagem
            var emailSendOperation = emailClient.Send(
                wait: WaitUntil.Started,
                message: emailMessage);

            status = emailSendOperation.Value.Status;
            String operationId = emailSendOperation.Id;

            // Monta retorno
            var tupla = Tuple.Create(status, operationId, true);
            return tupla;
        }

    }
}
