using nutritionoffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nutritionoffice.ViewModels
{
    public class dietfromprototype
    {
        public int? DietID { get; set; }

        public Diet Diet { get; set; }
    }

    public class dietdetails
    {
        public int DayIndex { get; set; }
        public List<DietDetail> DietDetails { get; set; }

    }
}