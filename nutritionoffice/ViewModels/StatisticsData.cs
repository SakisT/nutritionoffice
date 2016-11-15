using nutritionoffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nutritionoffice.ViewModels
{
    public class StatisticsData
    {
        public class VisistsInPeriod
        {
            public string TargetGroup { get; set; }
            public Customer Customer { get; set; }

            public double Age { get; set; }
            public string AgeText { get; set; }
            public string Sex { get; set; }
            public int MesurementsBefore{ get; set; }
            public int Measurements { get; set; }

            public int MeasurementsAfter { get; set; }
        }
    }
}