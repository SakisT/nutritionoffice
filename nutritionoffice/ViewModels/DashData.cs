using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nutritionoffice.ViewModels
{
    public class DashData
    {
        public ICollection<AppointmentsView> Appointments { get; set; }
        public class AppointmentsView
        {
            public Models.Customer Customer { get; set; }
            public Models.Appointment Appointment { get; set; }
            public Models.Reminder LastReminder { get; set; }
        }


        public ICollection<NameDaysView> CelebratingCustomers { get; set; }
        public class NameDaysView
        {
            public Classes.eotologioClass.NameDay NameDay { get; set; }
            public ICollection<Models.Customer> NameDayCustomers { get; set; }

            public ICollection<Models.Customer> BirthdayCustomers { get; set; }
        }

    }
}