using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace MvcComputer
{
    public class EmailService : IIdentityMessageService
    {

        public Task SendAsync(IdentityMessage message)
        {
            MailMessage email = new MailMessage("vinothrao89@hotmail.com", message.Destination);
 
            email.Subject = message.Subject;
 
            email.Body = message.Body;
 
            email.IsBodyHtml = true;
 
            var mailClient = new SmtpClient("smtp.live.com", 587) { Credentials = new NetworkCredential("vinothrao89@hotmail.com", "Bharathi123"), EnableSsl = true };
 
            return mailClient.SendMailAsync(email);
        }
    }
}