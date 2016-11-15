using nutritionoffice.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace nutritionoffice.ViewModels
{
    public class selectablecustomer
    {
        public Customer customer{ get; set; }

        public bool SendSMS { get; set; }

        private bool _SendEmail = true;
        public bool SendEmail {
            get
            {
                return _SendEmail;
            }
            set {
                _SendEmail = value;
            }
        }

        [Display(Name="Selected")]
        public bool IsSelected{ get; set; }

    }
}