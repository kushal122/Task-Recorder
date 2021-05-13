using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace TaskRecorder.Helper.Mail
{
    public class MailHelper
    {
        public void Send(string To, string confirmationLink)
        {            
            string From = "rraut202022@gmail.com";

            MailMessage mail = new MailMessage(From, To);

            mail.To.Add(From);
            mail.Subject = "Test";
            mail.Body = "Please click this link for confirmation :" + confirmationLink;
            mail.IsBodyHtml = false;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl = true;
            NetworkCredential networkCredential = new NetworkCredential(From, MailPassword.MailpasswordHelper());
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = networkCredential;
            smtp.Port = 587;
            try
            {
                smtp.Send(mail);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
