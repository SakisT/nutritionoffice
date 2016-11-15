using nutritionoffice.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace nutritionoffice.Classes
{
    public class Communicator
    {
        public static async Task<bool> SendMailAsync(SmtpMailClient mailClient, nMessage message, Attachment[] Attachments = null)
        {
            try
            {
                Company company;
                using (var db = new ndbContext())
                {
                    company = await db.Companies.FindAsync(message.CompanyID);
                }

                string[] allrecipients = message.Recipient.Split(new[] { ';' }).ToArray();


                MailMessage mailmessage = new MailMessage(from: new MailAddress(company.email, company.CompanyName), to: new MailAddress(allrecipients.FirstOrDefault()))
                {
                    Subject = message.Subject,
                    Body = message.MessageBody,
                    IsBodyHtml=true
                };


                foreach (var item in allrecipients.Skip(1))
                {
                    mailmessage.CC.Add(item);
                }

                if (Attachments != null) { Array.ForEach(Attachments, r => mailmessage.Attachments.Add(r)); }

                var smtp = new SmtpClient()
                {
                    Credentials = mailClient.Credentials,
                    Host = mailClient.Host,
                    Port = mailClient.Port,
                    EnableSsl = mailClient.EnableSSL
                };

                smtp.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);

                Task t = Task.Run(() => { smtp.SendAsync(message: mailmessage, userToken: message); });
                //await smtp.SendMailAsync(mailmessage);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            nMessage token = (nMessage)e.UserState;
            using (var db = new ndbContext())
            {
                nMessage message = db.Messages.Find(token.id);
                if (e.Cancelled)
                {
                    db.Messages.Remove(message);
                    db.Entry(message).State = System.Data.Entity.EntityState.Deleted;
                }
                if (e.Error != null)
                {
                    message.Status = nMessage.MessageStatus.Failed;
                    db.Entry(message).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {

                    message.Status = nMessage.MessageStatus.Send;
                    db.Entry(message).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
            }
        }

        //public bool SendSMS()
        //{
        //    try
        //    {

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        public class SmtpMailClient
        {
            public bool EnableSSL { get; set; }
            public string Host { get; set; }
            public int Port { get; set; }
            public System.Net.NetworkCredential Credentials { get; set; }

        }
    }
}