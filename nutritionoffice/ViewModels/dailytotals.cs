using System.Collections.Generic;
using System.Linq;

namespace nutritionoffice.ViewModels
{
    public class dailytotals
    {
        public decimal MinKalories { get; set; } = 0;
        public decimal MaxKalories { get; set; } = 0;
        public decimal MinCalcium { get; set; } = 0;
        public decimal MaxCalcium { get; set; } = 0;

        public decimal MinCarbohydrates { get; set; }
        public decimal MaxCarbohydrates { get; set; }

        public decimal MinProtein { get; set; }
        public decimal MaxProtein { get; set; }

        public decimal MinIron { get; set; }

        public decimal MaxIron { get; set; }

        public dailytotals() { }
        public dailytotals(int dietid, int dayindex)
        {
            using (Models.ndbContext db = new Models.ndbContext())
            {
                //db.Configuration.LazyLoadingEnabled = true;
                List<Models.DietDailyMeal> dailymeals = db.DietDailyMeals.Include("DietDetails").Include("DietDetails.Food").Where(r => r.DietID == dietid && r.DayIndex == dayindex).ToList();
                foreach (var item in dailymeals)
                {
                    if (item.DietDetails != null && item.DietDetails.Count() != 0)
                    {
                        var groups = item.DietDetails.GroupBy(r => r.Group);
                        MinKalories += groups.Min(r => r.Sum(f => f.Quantity * (f.Food.Equivalent.GetValueOrDefault(100) / 100) * f.Food.Energy.GetValueOrDefault(0)));
                        MaxKalories += groups.Max(r => r.Sum(f => f.Quantity * (f.Food.Equivalent.GetValueOrDefault(100) / 100) * f.Food.Energy.GetValueOrDefault(0)));
                        MinCalcium += groups.Min(r => r.Sum(f => f.Quantity * (f.Food.Equivalent.GetValueOrDefault(100) / 100) * f.Food.Calcium.GetValueOrDefault(0)));
                        MaxCalcium += groups.Max(r => r.Sum(f => f.Quantity * (f.Food.Equivalent.GetValueOrDefault(100) / 100) * f.Food.Calcium.GetValueOrDefault(0)));
                        MinCarbohydrates += groups.Min(r => r.Sum(f => f.Quantity * (f.Food.Equivalent.GetValueOrDefault(100) / 100) * f.Food.Carbohydrates.GetValueOrDefault(0)));
                        MaxCarbohydrates += groups.Max(r => r.Sum(f => f.Quantity * (f.Food.Equivalent.GetValueOrDefault(100) / 100) * f.Food.Carbohydrates.GetValueOrDefault(0)));
                        MinProtein += groups.Min(r => r.Sum(f => f.Quantity * (f.Food.Equivalent.GetValueOrDefault(100) / 100) * f.Food.Protein.GetValueOrDefault(0)));
                        MaxProtein += groups.Max(r => r.Sum(f => f.Quantity * (f.Food.Equivalent.GetValueOrDefault(100) / 100) * f.Food.Protein.GetValueOrDefault(0)));
                        MinIron += groups.Min(r => r.Sum(f => f.Quantity * (f.Food.Equivalent.GetValueOrDefault(100) / 100) * f.Food.Iron.GetValueOrDefault(0)));
                        MaxIron += groups.Max(r => r.Sum(f => f.Quantity * (f.Food.Equivalent.GetValueOrDefault(100) / 100) * f.Food.Iron.GetValueOrDefault(0)));
                    }
                }
            }
        }

        public string GetSingleFoodData(int foodid)
        {
            using (Models.ndbContext db = new Models.ndbContext())
            {
                Models.Food food = db.Foods.Find(foodid);
                MinKalories = food.Energy.GetValueOrDefault(0);
                MinCalcium = food.Calcium.GetValueOrDefault(0);
                MinCarbohydrates =food.Carbohydrates.GetValueOrDefault(0);
                MinProtein = food.Protein.GetValueOrDefault(0);
                MinIron =food.Iron.GetValueOrDefault(0);
            }
            string Text = "<table class='table table-condensed'>" +
                "<thead>" +
                    "<th>" +
                        "<tr>" +
                            "<td  colspan='2' class='text-center text-primary'><h4>Ανάλυση</h4></td>" +
                        "</tr>" +
                    "</th>" +
                    "<th>" +
                        "<tr style='font-style:italics'>" +
                            "<td>Συστατικό</td>" +
                            "<td>Τιμή</td>" +
                        "</tr>" +
                    "</th>" +
                "</thead>" +
                "<tbody class='text-muted'>" +
                    "<tr>" +
                        "<td>" + Resource.Energy + "</td>" +
                        "<td align='right'>" + string.Format($"{MinKalories:N1}") + "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>" + Resource.Calcium + "</td>" +
                        "<td align='right'>" + string.Format($"{MinCalcium:N1}") + "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>" + Resource.Carbohydrates + "</td>" +
                        "<td align='right'>" + string.Format($"{MinCarbohydrates:N1}") + "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>" + Resource.Protein + "</td>" +
                        "<td align='right'>" + string.Format($"{MinProtein:N1}") + "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>" + Resource.Iron + "</td>" +
                        "<td align='right'>" + string.Format($"{MinIron:N1}") + "</td>" +
                    "</tr>" +
                "</tbody>" +
            "</table>";
            return Text;
        }
        public string GetSingleMealData(int detailid)
        {
            using (Models.ndbContext db = new Models.ndbContext())
            {
                Models.DietDetail detail = db.DietDetails.Find(detailid);
                MinKalories = (detail.Quantity * detail.Food.Equivalent.GetValueOrDefault(100) / 100) * detail.Food.Energy.GetValueOrDefault(0);
                MinCalcium = (detail.Quantity * detail.Food.Equivalent.GetValueOrDefault(100) / 100) * detail.Food.Calcium.GetValueOrDefault(0);
                MinCarbohydrates = (detail.Quantity * detail.Food.Equivalent.GetValueOrDefault(100) / 100) * detail.Food.Carbohydrates.GetValueOrDefault(0);
                MinProtein = (detail.Quantity * detail.Food.Equivalent.GetValueOrDefault(100) / 100) * detail.Food.Protein.GetValueOrDefault(0);
                MinIron = (detail.Quantity * detail.Food.Equivalent.GetValueOrDefault(100) / 100) * detail.Food.Iron.GetValueOrDefault(0);
            }
            string Text = "<table class='table table-condensed'>" +
                "<thead>" +
                    "<th>" +
                        "<tr>" +
                            "<td colspan='2' class='text-center text-primary'><h4>Ανάλυση</h4></td>" +
                        "</tr>" +
                    "</th>" +
                    "<th>" +
                        "<tr style='font-style:italics'>" +
                            "<td>Συστατικό</td>" +
                            "<td>Τιμή</td>" +
                        "</tr>" +
                    "</th>" +
                "</thead>" +
                "<tbody class='text-muted'>" +
                    "<tr>" +
                        "<td>" + Resource.Energy + "</td>" +
                        "<td align='right'>" + string.Format($"{MinKalories:N1}") + "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>" + Resource.Calcium + "</td>" +
                        "<td align='right'>" + string.Format($"{MinCalcium:N1}") + "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>" + Resource.Carbohydrates + "</td>" +
                        "<td align='right'>" + string.Format($"{MinCarbohydrates:N1}") + "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>" + Resource.Protein + "</td>" +
                        "<td align='right'>" + string.Format($"{MinProtein:N1}") + "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>" + Resource.Iron + "</td>" +
                        "<td align='right'>" + string.Format($"{MinIron:N1}") + "</td>" +
                    "</tr>" +
                "</tbody>" +
            "</table>";
            return Text;
        }
    }
}