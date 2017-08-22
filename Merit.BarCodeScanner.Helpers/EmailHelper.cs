using System;
using System.Configuration;
using System.Net.Mail;
using System.Text;
using Merit.BarCodeScanner.Models;

namespace Merit.BarCodeScanner.Helpers
{
    public class EmailHelper
    {
        public static ResultRespose SendMail(AppSettingsSection _appSettings, EmailContent emailContent,DateTime? startDate)
        {
            var host = _appSettings.Settings["HostMail"]?.Value;
            var from = _appSettings.Settings["SendFrom"]?.Value;
            var to = _appSettings.Settings["SendTo"]?.Value;
            var port = _appSettings.Settings["PortMail"]?.Value;

            try
            {
                MailContent(emailContent.Subject, from, to, startDate, null, emailContent.Body);
                return new ResultRespose
                {
                    Status = true,
                    Message=""                    
                };
            }
            catch (Exception exception)
            {
                return new ResultRespose
                {
                    Status = true,
                    Message = exception.Message
                };
            }
        }

        private static void MailContent(string subject, string from, string to,DateTime? startDate,DateTime? endDate, string reason)
        {

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Hi, ");
            builder.Append("<br/>");
            builder.Append("Barcode Scanner Batch run got Error and Stop");
            builder.Append("<br/>");

            builder.AppendFormat("Batch Start : {0:MM/dd/yyyy HH:mm:ss} ", startDate);
            builder.Append("<br/>");
            builder.Append("Batch Status: Unsuccessfully");
            builder.Append("<br/>");
            builder.Append("Reason : "+reason);
            builder.Append("<br/>");
            builder.Append("Sincerely,");
            builder.Append("<br/>");
            builder.Append("Merit Logistics."); 
            string body = builder.ToString();
            var mailMessage = new MailMessage
            {
                From = new MailAddress(from),
                IsBodyHtml = true,
                Subject = subject,
                Body = body
            };
            string[] sendTo = to.Split(',');
            foreach (var t in sendTo)
            {
                mailMessage.To.Add(t);
            }
            var smtpClient = new SmtpClient();
            smtpClient.Send(mailMessage);
        }
    }
}
