using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace nutritionoffice.ViewModels
{
    public class partialusers
    {
        public string UserName { get; set; }
        public string UserID { get; set; }
        public string UserEmail { get; set; }
        [StringLength(15, MinimumLength = 8)]
        [RegularExpression(@"^.*(?=.*[!@#$%^&*\(\)_\-+=]).*$")]
        public string UserPassword { get; set; }
        public string[] CurrentRoles { get; set; }
        public string CompanyID { get; set; }
    }
    public class partialcompanies
    {
        public int id { get; set; }
        public string Name { get; set; }
    }
}