using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nutritionoffice.ViewModels
{
    public class AnniversaryCustomers
    {
        public string[] SimilarNames { get; set; }
        public int id { get; set; }
        public string LastName { get; set; }

        public string FirstName { get; set; }

        public DateTime OnDay { get; set; }

        public Anniversary AnniversaryType { get; set; }

        public enum Anniversary
        {
            NameDay=2,
            BirthDay=4
        }
    }

}