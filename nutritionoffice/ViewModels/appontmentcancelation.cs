using nutritionoffice.Models;
using System.ComponentModel.DataAnnotations;

namespace nutritionoffice.ViewModels
{
    public class appointmentcancelation
    {
        public Appointment appointment { get; set; }

        [Display(Name="Cancel")]
        public bool Cancel { get; set; }

        [Display(Name = "Send SMS")]
        public bool SendSMS { get; set; }

        [Display(Name = "Send Email")]
        public bool SendEmail { get; set; }

    }
}