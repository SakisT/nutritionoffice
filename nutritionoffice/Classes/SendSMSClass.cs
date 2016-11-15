using nutritionoffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nutritionoffice.Classes
{
    public class SendSMSClass
    {

        public void Send(string SenderCompanySign,string Username, string Password, string[] Recipients, string MessageText, int ReminderID)
        {
            SMService.smsService client = new SMService.smsService();
            try
            {
                client.sendCompleted += SendCompletedCallback;
                client.sendAsync(username:Username, password:Password,from:SenderCompanySign, to:Recipients,text:MessageText,coding:"GSM",flash:false,userState:new ReminderData { id=ReminderID });
            }
            catch (Exception ex)
            {
                Console.WriteLine();
            }
        }
        private void SendCompletedCallback(object sender, SMService.sendCompletedEventArgs e)
        {
            using (ndbContext db = new ndbContext())
            {
                ReminderData sendreminder =(ReminderData) e.UserState;
                int reminderid = sendreminder.id;
                Reminder reminder = db.Reminders.Find(reminderid);
                reminder.SMSState=Reminder.ReminderState.Completed;
                db.SaveChanges();

            }
        }

        private class ReminderData
        {
            public int id { get; set; }
        }
    }
}