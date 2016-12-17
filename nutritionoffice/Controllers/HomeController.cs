using nutritionoffice.Classes;
using nutritionoffice.Models;
using nutritionoffice.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web.Helpers;

namespace nutritionoffice.Controllers
{
    public class HomeController : MyBaseController
    {
        public ActionResult ChangeLanguage(string lang)
        {
            new SiteLanguages().SetLanguage(lang);
            return RedirectToAction("Index", "Home");
        }


        public async Task<ActionResult> Index(int appointmentsdateoffset = 0)
        {
            DateTime AppointmentsDate = DateTime.Today.AddDays(appointmentsdateoffset);
            DashData Dash = new DashData();
            ViewData["appointmentsdateoffset"] = appointmentsdateoffset;
            byte[] logo = null;
            if (CompanyID() > 0)
            {
                using (ndbContext db = new ndbContext())
                {
                    int companyid = CompanyID();
                    Company company = db.Companies.Find(companyid);
                    if (company != null && company.logo != null && company.logo.Length > 10)
                    {
                        logo = company.logo;
                    }

                    List<Customer> CompanyCustomers = db.Customers.Include("Reminders").Where(r => r.CompanyID == companyid).ToList();
                    Dash.Appointments = (from p in db.Appointments.OrderBy(r => r.Date).ThenBy(r=>r.FromTime_Hour).ThenBy(r=>r.FromTime_Minutes).Where(r => r.Customer.CompanyID == companyid && r.Date == AppointmentsDate)
                                         select
new DashData.AppointmentsView
{
    Customer = p.Customer,
    Appointment = p,
    LastReminder = p.Customer.Reminders.OrderByDescending(r => r.OnDate).FirstOrDefault()
}).ToList();

                    Dash.CelebratingCustomers = new List<DashData.NameDaysView>();
                    foreach (var namegroup in eotologioClass.NameDays())
                    {
                        Dash.CelebratingCustomers.Add(new DashData.NameDaysView
                        {
                            NameDay = namegroup,
                            NameDayCustomers = ((from p in namegroup.Names
                                                 join k in CompanyCustomers on p equals k.FirstName
                                                 select k).ToList()),
                            BirthdayCustomers = (CompanyCustomers.Where(r => r.BirthDate.Month == namegroup.Date.Month && r.BirthDate.Day == namegroup.Date.Day)).ToList()
                        });
                    }
                }
            }
            ViewBag.CompanyID = this.CompanyID();
            ViewBag.Logo = logo;

            return View(Dash);
        }

        public JsonResult GetNameDays()
        {
            List<AnniversaryCustomers> NameCustomers = new List<AnniversaryCustomers>();
            List<AnniversaryCustomers> BirthCustomers = new List<AnniversaryCustomers>();
            var CelebratingNames = eotologioClass.NameDays();

            int companyid = CompanyID();
            if (companyid > 0)
            {

                using (ndbContext db = new ndbContext())
                {
                    List<Customer> CompanyCustomers = db.Customers.Where(r => r.CompanyID == companyid).ToList();
                    foreach (var namegroup in CelebratingNames)
                    {
                        var NameDayAnniversaries = from p in namegroup.Names
                                                   join k in CompanyCustomers on p equals k.FirstName
                                                   select new AnniversaryCustomers
                                                   {
                                                       SimilarNames = namegroup.HeadNames,
                                                       AnniversaryType = AnniversaryCustomers.Anniversary.NameDay,
                                                       OnDay = namegroup.Date,
                                                       id = k.id,
                                                       LastName = k.LastName,
                                                       FirstName = k.FirstName
                                                   };
                        NameCustomers.AddRange(NameDayAnniversaries.ToArray());
                    }

                    System.DateTime[] dates = (from p in Enumerable.Range(0, 3) let d = System.DateTime.Today.AddDays(p) select d).ToArray();
                    var BirtDayAnniversaries = (from p in CompanyCustomers where dates.Any(d => p.BirthDate.Month == d.Month && p.BirthDate.Day == d.Day) select p);
                    foreach (var d in dates)
                    {
                        BirthCustomers.AddRange((from p in BirtDayAnniversaries
                                                 where p.BirthDate.Month == d.Month && p.BirthDate.Day == d.Day
                                                 select new ViewModels.AnniversaryCustomers
                                                 {
                                                     AnniversaryType = ViewModels.AnniversaryCustomers.Anniversary.BirthDay,
                                                     FirstName = p.FirstName,
                                                     id = p.id,
                                                     LastName = p.LastName,
                                                     OnDay = d,
                                                     SimilarNames = new string[] { }
                                                 }).ToArray());

                    }

                }
            }
            return Json(new
            {
                CelebratingNames = CelebratingNames.ToArray(),
                NameDayCustomers = NameCustomers.ToArray(),
                BirthDayCustomers = BirthCustomers.ToArray(),
                NameDayTooltip = "Γιορτάζουν:\r\n" + string.Join(", ", NameCustomers.Select(r => string.Format($"{r.LastName} {r.FirstName}")).ToArray()),
                BirthDayTooltip = "Έχουν γεννέθλια:\r\n" + string.Join(", ", BirthCustomers.Select(r => string.Format($"{r.LastName} {r.FirstName} στις {r.OnDay:d MMMM}")).ToArray()),
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}