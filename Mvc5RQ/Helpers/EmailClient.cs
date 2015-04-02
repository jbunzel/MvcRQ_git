using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;


namespace Mvc5RQ.Helpers
{
    public class EmailClient
    {
        public static Task SendAsync(string destination, string subject, string body)
        {
            var credentialUserName = "renate.bunzel@riquest.de";
            var sentFrom = "support@riquest.de";
            var pwd = "quiko";

            // Configure the client:
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("smtp.riquest.de");
            client.Port = 25;
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;

            // Create the credentials:
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(credentialUserName, pwd);

            client.EnableSsl = false;
            client.Credentials = credentials;

            // Create the message:
            var mail = new System.Net.Mail.MailMessage(sentFrom, destination);

            mail.Subject = subject;
            mail.Body = body;

            // Send:
            return client.SendMailAsync(mail);
        }

        public static void Send(string destination, string subject, string body)
        {
            var credentialUserName = "renate.bunzel@riquest.de";
            var sentFrom = "support@riquest.de";
            var pwd = "quiko";

            // Configure the client:
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("smtp.riquest.de");
            client.Port = 25;
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;

            // Create the credentials:
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(credentialUserName, pwd);

            client.EnableSsl = false;
            client.Credentials = credentials;

            // Create the message:
            var mail = new System.Net.Mail.MailMessage(sentFrom, destination);

            mail.Subject = subject;
            mail.Body = body;

            // Send:
            client.Send(mail);
        }

    }
}