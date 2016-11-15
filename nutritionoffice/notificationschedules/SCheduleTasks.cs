using nutritionoffice.Models;
using Quartz;
using Quartz.Impl;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Data;
using nutritionoffice.Classes;

namespace nutritionoffice.notificationschedules
{
    public class EmailJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                using (ndbContext db = new ndbContext())
                {
                    //var greektimezone = TimeZoneInfo.FindSystemTimeZoneById("GTB Standard Time");// TimeZoneInfo.GetSystemTimeZones()[55];
                    //var greektimeoffset= greektimezone.GetUtcOffset(DateTime.UtcNow);
                    DateTime dt = SharedClass.Now();// DateTime.UtcNow.Add(greektimeoffset);
                    IQueryable<Reminder> PenddingEmailTasks = db.Reminders.Include("Customer").Include("Customer.Company").Where(r => r.MailState == Reminder.ReminderState.Active && 
                                            r.SendEmail && r.Message != null && r.email != null && r.OnDate < dt);
                    foreach (Reminder reminder in PenddingEmailTasks)
                    {

                        var company = reminder.Customer.Company;

                        using (var message = new MailMessage(company.email, reminder.email))
                        {
                            message.Subject = string.Format($"Υπενθύμιση ({company.CompanyName})");
                            message.Body = reminder.Message ?? "" + SharedClass.Now();
                            using (SmtpClient client = new SmtpClient
                            {
                                EnableSsl = company.SMTPEnableSSL,
                                Host = company.SMTPHost,
                                Port = company.SMTPPort,
                                Credentials = new NetworkCredential(company.SMTPUserName, company.SMTPPassword)
                            })
                            {
                                client.Send(message);
                            }
                        }
                        reminder.MailState = Reminder.ReminderState.Completed;
                    }

                    IQueryable<Reminder> PenddingSMSTasks = db.Reminders.Include("Customer").Include("Customer.Company").Where(r => r.SMSState == Reminder.ReminderState.Active && r.SendSMS && r.Message != null && r.Mobile != null && r.OnDate < dt);
                    foreach (Reminder reminder in PenddingSMSTasks)
                    {
                        var company = reminder.Customer.Company;

                        if (company.SMSSign != null && company.SMSUserName!=null && company.SMSPassword!=null)
                        {
                            SendSMSClass cls = new SendSMSClass();
                            cls.Send( SenderCompanySign: company.SMSSign, Username:company.SMSUserName, Password:company.SMSPassword, Recipients: new string[] { reminder.Mobile }.ToArray(), MessageText: reminder.Message, ReminderID: reminder.id);
                        }

                    }
                    db.SaveChanges();
                }
            }
        }
    }



    public class JobScheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<EmailJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInMinutes(20)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))
                  )
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }



    public class GSM
    {
        private string _Message;
        private string _InvalidCharacters;
        private byte[] _Bytes;
        private int _BytesLength;
        public event EventHandler InvalidCharTyped;

        public GSM(string Message)
        {
            CalcBytes(Message);
        }
        public GSM()
        {
        }
        private string _GSMMessage;
        public string GSMMessage
        {
            get { return _GSMMessage; }
        }

        public string InvalidCharacters
        {
            get { return _InvalidCharacters; }
        }

        public string Message
        {
            get { return _Message; }
            set
            {
                _Message = value;
                if (!string.IsNullOrEmpty(_Message))
                    CalcBytes(_Message);
            }
        }

        public int GSM_Chars_Length
        {
            get { return _BytesLength; }
        }

        public byte[] Bytes
        {
            get { return _Bytes; }
        }

        private void CalcBytes(string Text)
        {
            _InvalidCharacters = "";
            if (string.IsNullOrEmpty(Text))
            {
                _BytesLength = 0;
                _Bytes = null;
            }
            _GSMMessage = "";
            byte[] ReturnByte = new byte[Text.Length];
            int ByteIndex = 0;
            for (int i = 0; i <= Text.Length - 1; i++)
            {
                char Character = Text.Skip(i).FirstOrDefault();
                switch (Character)
                {
                    case '@':
                        ReturnByte[ByteIndex] = 0;
                        _GSMMessage += '@';
                        break;
                    case '£':
                        ReturnByte[ByteIndex] = 1;
                        _GSMMessage += '£';
                        break;
                    case '$':
                        ReturnByte[ByteIndex] = 2;
                        _GSMMessage += '$';
                        break;
                    case '\n':
                        ReturnByte[ByteIndex] = 10;
                        _GSMMessage += "\r\n";
                        break;
                    case '\r':
                        ReturnByte[ByteIndex] = 13;
                        _GSMMessage += "\r";
                        break;
                    case 'Δ':
                    case 'δ':
                        ReturnByte[ByteIndex] = 16;
                        _GSMMessage += 'Δ';
                        break;
                    case '_':
                        ReturnByte[ByteIndex] = 17;
                        _GSMMessage += '_';
                        break;
                    case 'Φ':
                    case 'φ':
                        ReturnByte[ByteIndex] = 18;
                        _GSMMessage += 'Φ';
                        break;
                    case 'Γ':
                    case 'γ':
                        ReturnByte[ByteIndex] = 19;
                        _GSMMessage += 'Γ';
                        break;
                    case 'Λ':
                    case 'λ':
                        ReturnByte[ByteIndex] = 20;
                        _GSMMessage += 'Λ';
                        break;
                    case 'Ω':
                    case 'ω':
                    case 'ώ':
                        ReturnByte[ByteIndex] = 21;
                        _GSMMessage += 'Ω';
                        break;
                    case 'Π':
                    case 'π':
                        ReturnByte[ByteIndex] = 22;
                        _GSMMessage += 'Π';
                        break;
                    case 'Ψ':
                    case 'ψ':
                        ReturnByte[ByteIndex] = 23;
                        _GSMMessage += 'Ψ';
                        break;
                    case 'Σ':
                    case 'σ':
                    case 'ς':
                        ReturnByte[ByteIndex] = 24;
                        _GSMMessage += 'Σ';
                        break;
                    case 'Θ':
                    case 'θ':
                        ReturnByte[ByteIndex] = 25;
                        _GSMMessage += 'Θ';
                        break;
                    case 'Ξ':
                    case 'ξ':
                        ReturnByte[ByteIndex] = 26;
                        _GSMMessage += 'Ξ';
                        break;
                    case ' ':
                        ReturnByte[ByteIndex] = 32;
                        _GSMMessage += ' ';
                        break;
                    case '!':
                        ReturnByte[ByteIndex] = 33;
                        _GSMMessage += '!';
                        break;
                    case '\\':
                        ReturnByte[ByteIndex] = 34;
                        _GSMMessage += '\'';
                        break;
                    case '#':
                        ReturnByte[ByteIndex] = 35;
                        _GSMMessage += '#';
                        break;
                    case '%':
                        ReturnByte[ByteIndex] = 37;
                        _GSMMessage += '%';
                        break;
                    case '&':
                        ReturnByte[ByteIndex] = 38;
                        _GSMMessage += '&';
                        break;
                    case '\'':
                        ReturnByte[ByteIndex] = 39;
                        _GSMMessage += '\'';
                        break;
                    case '(':
                        ReturnByte[ByteIndex] = 40;
                        _GSMMessage += '(';
                        break;
                    case ')':
                        ReturnByte[ByteIndex] = 41;
                        _GSMMessage += ')';
                        break;
                    case '*':
                        ReturnByte[ByteIndex] = 42;
                        _GSMMessage += '*';
                        break;
                    case '+':
                        ReturnByte[ByteIndex] = 43;
                        _GSMMessage += '+';
                        break;
                    case ',':
                        ReturnByte[ByteIndex] = 44;
                        _GSMMessage += ',';
                        break;
                    case '-':
                        ReturnByte[ByteIndex] = 45;
                        _GSMMessage += '-';
                        break;
                    case '.':
                        ReturnByte[ByteIndex] = 46;
                        _GSMMessage += '.';
                        break;
                    case '/':
                        ReturnByte[ByteIndex] = 47;
                        _GSMMessage += '/';
                        break;
                    case '0':
                        ReturnByte[ByteIndex] = 48;
                        _GSMMessage += '0';
                        break;
                    case '1':
                        ReturnByte[ByteIndex] = 49;
                        _GSMMessage += '1';
                        break;
                    case '2':
                        ReturnByte[ByteIndex] = 50;
                        _GSMMessage += '2';
                        break;
                    case '3':
                        ReturnByte[ByteIndex] = 51;
                        _GSMMessage += '3';
                        break;
                    case '4':
                        ReturnByte[ByteIndex] = 52;
                        _GSMMessage += '4';
                        break;
                    case '5':
                        ReturnByte[ByteIndex] = 53;
                        _GSMMessage += '5';
                        break;
                    case '6':
                        ReturnByte[ByteIndex] = 54;
                        _GSMMessage += '6';
                        break;
                    case '7':
                        ReturnByte[ByteIndex] = 55;
                        _GSMMessage += '7';
                        break;
                    case '8':
                        ReturnByte[ByteIndex] = 56;
                        _GSMMessage += '8';
                        break;
                    case '9':
                        ReturnByte[ByteIndex] = 57;
                        _GSMMessage += '9';
                        break;
                    case ':':
                        ReturnByte[ByteIndex] = 58;
                        _GSMMessage += ':';
                        break;
                    case ';':
                        ReturnByte[ByteIndex] = 59;
                        _GSMMessage += ';';
                        break;
                    case '<':
                        ReturnByte[ByteIndex] = 60;
                        _GSMMessage += '<';
                        break;
                    case '=':
                        ReturnByte[ByteIndex] = 61;
                        _GSMMessage += '=';
                        break;
                    case '>':
                        ReturnByte[ByteIndex] = 62;
                        _GSMMessage += '>';
                        break;
                    case '?':
                        ReturnByte[ByteIndex] = 63;
                        _GSMMessage += '?';
                        break;
                    case 'A':
                    case 'Α':
                    case 'α':
                    case 'ά':
                    case 'Ά':
                        ReturnByte[ByteIndex] = 65;
                        _GSMMessage += 'Α';
                        break;
                    case 'Β':
                    case 'β':
                    case 'B':
                        ReturnByte[ByteIndex] = 66;
                        _GSMMessage += 'Β';
                        break;
                    case 'C':
                        ReturnByte[ByteIndex] = 67;
                        _GSMMessage += 'C';
                        break;
                    case 'D':
                        ReturnByte[ByteIndex] = 68;
                        _GSMMessage += 'D';
                        break;
                    case 'E':
                    case 'Ε':
                    case 'ε':
                    case 'έ':
                        ReturnByte[ByteIndex] = 69;
                        _GSMMessage += 'E';
                        break;
                    case 'F':
                        ReturnByte[ByteIndex] = 70;
                        _GSMMessage += 'F';
                        break;
                    case 'G':
                        ReturnByte[ByteIndex] = 71;
                        _GSMMessage += 'G';
                        break;
                    case 'H':
                    case 'Η':
                    case 'η':
                    case 'ή':
                    case 'Ή':
                        ReturnByte[ByteIndex] = 72;
                        _GSMMessage += 'H';
                        break;
                    case 'I':
                    case 'Ι':
                    case 'ι':
                    case 'ί':
                    case 'ϊ':
                    case 'ΐ':
                    case 'Ί':
                        ReturnByte[ByteIndex] = 73;
                        _GSMMessage += 'I';
                        break;
                    case 'J':
                        ReturnByte[ByteIndex] = 74;
                        _GSMMessage += 'J';
                        break;
                    case 'K':
                    case 'Κ':
                    case 'κ':
                        ReturnByte[ByteIndex] = 75;
                        _GSMMessage += 'K';
                        break;
                    case 'L':
                        ReturnByte[ByteIndex] = 76;
                        _GSMMessage += 'L';
                        break;
                    case 'M':
                    case 'Μ':
                    case 'μ':
                        ReturnByte[ByteIndex] = 77;
                        _GSMMessage += 'M';
                        break;
                    case 'N':
                    case 'Ν':
                    case 'ν':
                        ReturnByte[ByteIndex] = 78;
                        _GSMMessage += 'N';
                        break;
                    case 'O':
                    case 'Ο':
                    case 'ο':
                    case 'ό':
                    case 'Ό':
                        ReturnByte[ByteIndex] = 79;
                        _GSMMessage += 'O';
                        break;
                    case 'P':
                    case 'Ρ':
                    case 'ρ':
                        ReturnByte[ByteIndex] = 80;
                        _GSMMessage += 'P';
                        break;
                    case 'Q':
                        ReturnByte[ByteIndex] = 81;
                        _GSMMessage += 'Q';
                        break;
                    case 'R':
                        ReturnByte[ByteIndex] = 82;
                        _GSMMessage += 'R';
                        break;
                    case 'S':
                        ReturnByte[ByteIndex] = 83;
                        _GSMMessage += 'S';
                        break;
                    case 'T':
                    case 'Τ':
                    case 'τ':
                        ReturnByte[ByteIndex] = 84;
                        _GSMMessage += 'T';
                        break;
                    case 'U':
                        ReturnByte[ByteIndex] = 85;
                        _GSMMessage += 'U';
                        break;
                    case 'V':
                        ReturnByte[ByteIndex] = 86;
                        _GSMMessage += 'V';
                        break;
                    case 'W':
                        ReturnByte[ByteIndex] = 87;
                        _GSMMessage += 'W';
                        break;
                    case 'X':
                    case 'Χ':
                    case 'χ':
                        ReturnByte[ByteIndex] = 88;
                        _GSMMessage += 'X';
                        break;
                    case 'Y':
                    case 'Υ':
                    case 'υ':
                    case 'ύ':
                    case 'ϋ':
                    case 'ΰ':
                        ReturnByte[ByteIndex] = 89;
                        _GSMMessage += 'Y';
                        break;
                    case 'Z':
                    case 'Ζ':
                    case 'ζ':
                        ReturnByte[ByteIndex] = 90;
                        _GSMMessage += 'Z';
                        break;
                    case 'a':
                        ReturnByte[ByteIndex] = 97;
                        _GSMMessage += 'a';
                        break;
                    case 'b':
                        ReturnByte[ByteIndex] = 98;
                        _GSMMessage += 'b';
                        break;
                    case 'c':
                        ReturnByte[ByteIndex] = 99;
                        _GSMMessage += 'c';
                        break;
                    case 'd':
                        ReturnByte[ByteIndex] = 100;
                        _GSMMessage += 'd';
                        break;
                    case 'e':
                        ReturnByte[ByteIndex] = 101;
                        _GSMMessage += 'e';
                        break;
                    case 'f':
                        ReturnByte[ByteIndex] = 102;
                        _GSMMessage += 'f';
                        break;
                    case 'g':
                        ReturnByte[ByteIndex] = 103;
                        _GSMMessage += 'g';
                        break;
                    case 'h':
                        ReturnByte[ByteIndex] = 104;
                        _GSMMessage += 'h';
                        break;
                    case 'i':
                        ReturnByte[ByteIndex] = 105;
                        _GSMMessage += 'i';
                        break;
                    case 'j':
                        ReturnByte[ByteIndex] = 106;
                        _GSMMessage += 'j';
                        break;
                    case 'k':
                        ReturnByte[ByteIndex] = 107;
                        _GSMMessage += 'k';
                        break;
                    case 'l':
                        ReturnByte[ByteIndex] = 108;
                        _GSMMessage += 'l';
                        break;
                    case 'm':
                        ReturnByte[ByteIndex] = 109;
                        _GSMMessage += 'm';
                        break;
                    case 'n':
                        ReturnByte[ByteIndex] = 110;
                        _GSMMessage += 'n';
                        break;
                    case 'o':
                        ReturnByte[ByteIndex] = 111;
                        _GSMMessage += 'o';
                        break;
                    case 'p':
                        ReturnByte[ByteIndex] = 112;
                        _GSMMessage += 'p';
                        break;
                    case 'q':
                        ReturnByte[ByteIndex] = 113;
                        _GSMMessage += 'q';
                        break;
                    case 'r':
                        ReturnByte[ByteIndex] = 114;
                        _GSMMessage += 'r';
                        break;
                    case 's':
                        ReturnByte[ByteIndex] = 115;
                        _GSMMessage += 's';
                        break;
                    case 't':
                        ReturnByte[ByteIndex] = 116;
                        _GSMMessage += 't';
                        break;
                    case 'u':
                        ReturnByte[ByteIndex] = 117;
                        _GSMMessage += 'u';
                        break;
                    case 'v':
                        ReturnByte[ByteIndex] = 118;
                        _GSMMessage += 'v';
                        break;
                    case 'w':
                        ReturnByte[ByteIndex] = 119;
                        _GSMMessage += 'w';
                        break;
                    case 'x':
                        ReturnByte[ByteIndex] = 120;
                        _GSMMessage += 'x';
                        break;
                    case 'y':
                        ReturnByte[ByteIndex] = 121;
                        _GSMMessage += 'y';
                        break;
                    case 'z':
                        ReturnByte[ByteIndex] = 122;
                        _GSMMessage += 'z';
                        break;
                    default:
                        this._InvalidCharacters += Character;
                        if (InvalidCharTyped != null)
                        {
                            InvalidCharTyped(this, EventArgs.Empty);
                        }

                        break;
                }
                ByteIndex += 1;
                this._BytesLength = Math.Max(ByteIndex, 0);
            }

            this._Bytes = ReturnByte;
        }

    }

    //=======================================================
    //Service provided by Telerik (www.telerik.com)
    //Conversion powered by NRefactory.
    //Twitter: @telerik
    //Facebook: facebook.com/telerik
    //=======================================================

}