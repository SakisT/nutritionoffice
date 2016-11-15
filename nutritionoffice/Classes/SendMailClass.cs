using System;
using System.ComponentModel;
using System.Net.Mail;
using System.Threading.Tasks;

namespace nutritionoffice.Classes
{

    public class SendMailClass
    {
        public string[] Recipients { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public async Task SendEmail(string toEmailAddress, string emailSubject, string emailMessage)
        {
            var message = new MailMessage();
            message.To.Add(toEmailAddress);

            message.Subject = emailSubject;
            message.Body = emailMessage;
            using (var smtpClient = new SmtpClient())
            {
                await smtpClient.SendMailAsync(message);
            }
        }

        private void SendCompletedCallback(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            MailMessage userToken = (MailMessage)e.UserState;
            if (e.Cancelled)
            {

            }
            if (e.Error!=null)
            {

            }
        }
    }
}