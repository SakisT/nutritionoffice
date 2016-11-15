using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using nutritionoffice.Models;
using nutritionoffice.ViewModels;
using System.Collections.Generic;
using Microsoft.Reporting.WebForms;
using System.Data.Entity.SqlServer;
//using System.Web.Helpers;
//using System.Web.UI.DataVisualization.Charting;

namespace nutritionoffice.Controllers
{
    [Authorize(Roles = "Administrator,Owner")]
    public class ReportsController : MyBaseController
    {
        private ndbContext db = new ndbContext();

        public async Task<ActionResult> Details(DateTime? fromdate, DateTime? todate, int? targetgroupid, string filetype = "")
        {
            if (fromdate == null)
            {
                fromdate = DateTime.Today.AddMonths(-6);
                todate = DateTime.Today;
            }
            int companyid = CompanyID();
            var CompanyCustomers = db.Customers.Include(r => r.TargetGroup).Include(r => r.Measurements).Where(r => r.CompanyID == companyid);
            if (targetgroupid.HasValue)
            {
                CompanyCustomers = CompanyCustomers.Where(r => r.TargetGroupID == targetgroupid.Value);
            }

            var AllData = from p in CompanyCustomers
                          let MeasurementsInPeriod = p.Measurements.Where(r => r.Date >= fromdate.Value && r.Date <= todate.Value)
                          let MeasurementsBeforePeriod = p.Measurements.Where(r => r.Date < fromdate.Value)
                          let measurementsAfterPeriod = p.Measurements.Where(r => r.Date > todate.Value)
                          select new
                          {
                              Customer = p,
                              MeasurementsInPeriod = MeasurementsInPeriod,
                              MeasurementsBeforePeriod = MeasurementsBeforePeriod,
                              measurementsAfterPeriod = measurementsAfterPeriod
                          };
            var meninperiod = AllData.Where(r => r.MeasurementsInPeriod.Count() > 0 && r.Customer.Sex == Customer.sex.Male);
            var womeninperiod = AllData.Where(r => r.MeasurementsInPeriod.Count() > 0 && r.Customer.Sex == Customer.sex.Female);
            var meninperiodbytargetgroup = meninperiod.GroupBy(r => r.Customer.TargetGroupID);
            var womeninperiodbytargetgroup = womeninperiod.GroupBy(r => r.Customer.TargetGroupID);

            var temp = AllData.GroupBy(r => new { r.Customer.TargetGroupID, r.Customer.Sex, r.Customer.Measurements.OrderByDescending(a => a.Age).FirstOrDefault().Age });

            return View();
        }
        // GET: Reports
        public async Task<ActionResult> Visits(DateTime? fromdate, DateTime? todate, bool print = false)
        {
            DateTime FromDate = fromdate ?? DateTime.Today.AddMonths(-1);
            DateTime ToDate = todate ?? DateTime.Today;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            int CompID = CompanyID();
            List<Appointment> appointments = await db.Appointments.Include(r => r.Customer).OrderByDescending(r => r.Date).Where(r => r.Date <= ToDate && r.Date >= FromDate && r.Customer.CompanyID == CompID).ToListAsync();
            return View(appointments);
        }

        public async Task<ActionResult> NewCustomers(DateTime? fromdate, DateTime? todate, bool print = false)
        {
            DateTime FromDate = fromdate ?? DateTime.Today.AddMonths(-1);
            DateTime ToDate = todate ?? DateTime.Today;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            int CompID = CompanyID();
            List<Customer> _customers = await db.Customers.OrderByDescending(r => r.CreatedOn).Where(r => r.CreatedOn.HasValue && r.CreatedOn <= ToDate && r.CreatedOn >= FromDate && r.CompanyID == CompID).ToListAsync();
            List<ViewModels.selectablecustomer> customers = (from p in _customers select new selectablecustomer { customer = p, IsSelected = true }).ToList();
            return View(customers);
        }

        public async Task<ActionResult> Measurements(DateTime? fromdate, DateTime? todate, bool print = false, string filetype = "IMAGE")
        {
            DateTime initdate = DateTime.Today.AddMonths(-1);
            DateTime FromDate = fromdate ?? new DateTime(initdate.Year, initdate.Month, 1);
            DateTime ToDate = todate ?? FromDate.AddMonths(1).AddDays(-1);
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            int CompID = CompanyID();
            List<Measurement> measurements = await db.Measurements.Include(r => r.Customer).OrderByDescending(r => r.Date).Where(r => r.Date <= ToDate && r.Date >= FromDate && r.Customer.CompanyID == CompID).ToListAsync();
            if (!print) { return View(measurements); }


            LocalReport lr = new LocalReport();

            //#if DEBUG
            //            lr.ReportPath = Server.MapPath(Path.Combine("~/Reports", "Documents", "MonthlyMeasurements.rdlc"));
            //#else
            lr.LoadReportDefinition(await Classes.AzureStorageClass.GetRDLC("reportdocuments", "MonthlyMeasurements.rdlc"));
            //#endif




            ReportDataSource rd = new ReportDataSource("MeasurementsDS", (from Measurement p in measurements
                                                                          select new
                                                                          {
                                                                              FullName = p.Customer.FullName,
                                                                              MeasurementDate = p.Date,
                                                                              Weight = p.Weight,
                                                                              Fat = p.Fat,
                                                                              RatioWeist = p.WaistHipRatio,
                                                                              Triglycerides = p.triglycerides,
                                                                              Cholesterol = p.cholesterol,
                                                                              BloodPressure = p.BloodPressure,
                                                                              Ferrum = p.Ferrum,
                                                                              Spelter = p.spelter,
                                                                              eFAT = p.e_FAT
                                                                          }).ToList());

            lr.DataSources.Add(rd);

            lr.SetParameters(new ReportParameter("ReportHeader", String.Format("Περίοδος επισκέψεων από {0:d/M/yyyy} έως {1:d/M/yyyy}", FromDate, ToDate)));

            lr.SetParameters(new ReportParameter("ReportFooter", string.Format("Συνολικός αριθμός πελατών : {0}, Αριθμός μετρήσεων : {1}", measurements.Select(r => r.CustomerID).Distinct().Count(), measurements.Count())));

            string reportType = filetype;// (filetype != "PDF") ? "Excel" : filetype;// id;
            string extension = (filetype == "IMAGE") ? "tiff" : (filetype == "PDF") ? "pdf" : (filetype == "EXCEL") ? "xls" : "doc";// (filetype != "PDF") ? "xlsx" : "pdf";// id;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + reportType + "</OutputFormat>" +
            "  <PageWidth>11.692in</PageWidth>" +
            "  <PageHeight>8.267in</PageHeight>" +
            "  <MarginTop>0.3cm</MarginTop>" +
            "  <MarginLeft>0.4cm</MarginLeft>" +
            "  <MarginRight>0.4cm</MarginRight>" +
            "  <MarginBottom>0.3cm</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderBytes;

            renderBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            return File(renderBytes, mimeType, string.Format("Report Period {0:yyyy-MM-dd} - {1:yyyy-MM-dd}.{2}", FromDate, ToDate, extension));
        }

        public ActionResult VisitsStatistics()
        {
            int companyid = CompanyID();
            Company company = db.Companies.Find(companyid);

            Measurement companyfirstmeasurements = db.Measurements.OrderBy(r => r.Date).Where(r => r.Customer.CompanyID == companyid).FirstOrDefault();
            ViewBag.FromDate = (companyfirstmeasurements == null) ? DateTime.Today : companyfirstmeasurements.Date;
            IEnumerable<AgeRange> companyageranges = company.AgeRanges.OrderBy(r => r.FromAge).ThenBy(r => r.ToAge);
            ViewBag.TargetGroupID = new SelectList(db.TargetGroups.Where(r => r.CompanyID == companyid), "id", "Name");
            ViewBag.AgeRanges = new SelectList(companyageranges, "id", "RangeText");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> VisitsInPeriod(DateTime fromdate, DateTime todate, int? targetgroupid, int? agerangeid, int? sexid, string filetype = "")
        {
            int companyid = CompanyID();
            if (companyid == 0) { return new HttpStatusCodeResult(404); }
            Company company = await db.Companies.FindAsync(companyid);
            db.Entry(company).Reference("Picture").Load();
            IQueryable<Customer> customers = db.Customers.Include(r => r.TargetGroup).Include(r => r.Measurements).OrderBy(r => r.TargetGroup.Name).ThenBy(r => r.LastName).Where(r => r.CompanyID == companyid);
            TargetGroup targetgroup = null;
            AgeRange agerange = null;
            if (targetgroupid != null)
            {
                targetgroup = await db.TargetGroups.FindAsync(targetgroupid.Value);
                customers = customers.Where(r => r.TargetGroupID == targetgroupid.Value);
            }
            if (sexid != null)
            {
                customers = customers.Where(r => r.Sex == (Customer.sex)sexid);
            }

            List<AgeRange> CompanyAgeRanges = company.AgeRanges.ToList();

            List<StatisticsData.VisistsInPeriod> data = await (from Customer p in customers
                                                               let MeasurementsInPeriod = p.Measurements.Where(r => r.Date >= fromdate && r.Date <= todate).Count()
                                                               where MeasurementsInPeriod > 0
                                                               select new StatisticsData.VisistsInPeriod
                                                               {
                                                                   Customer = p,
                                                                   TargetGroup = p.TargetGroup.Name,
                                                                   Sex = (p.Sex == Customer.sex.Male) ? Resource.ManSex : Resource.WomanSex,
                                                                   Age = 0,
                                                                   AgeText = "",
                                                                   MesurementsBefore = 0,
                                                                   Measurements = MeasurementsInPeriod,
                                                                   MeasurementsAfter = 0
                                                               }).ToListAsync();

            AgeRange defaultagerange = new AgeRange { FromAge = 0, ToAge = 0 };
            foreach (var item in data)
            {
                item.MesurementsBefore = item.Customer.Measurements.Where(r => r.Date < fromdate).Count();
                item.MeasurementsAfter = item.Customer.Measurements.Where(r => r.Date > todate).Count();
                item.Age = Math.Round((item.Measurements > 0) ? (from k in item.Customer.Measurements.Where(r => r.Date >= fromdate && r.Date <= todate) let Age = (k.Date - k.Customer.BirthDate).TotalDays / 365d select Age).Average() : (from k in item.Customer.Measurements let Age = (k.Date - k.Customer.BirthDate).TotalDays / 365 select Age).Average(), 1);
                item.AgeText = CompanyAgeRanges.Where(a => a.FromAge <= item.Age && a.ToAge >= item.Age).DefaultIfEmpty(defaultagerange).FirstOrDefault().RangeText;
            }

            if (agerangeid != null)
            {
                agerange = await db.AgeRanges.FindAsync(agerangeid);
                data = (from StatisticsData.VisistsInPeriod p in data where p.Measurements > 0 && p.AgeText == agerange.RangeText select p).ToList();
            }

            ViewBag.FromDate = string.Format($"{fromdate:d/M/yyyy}");
            ViewBag.ToDate = string.Format($"{todate:d/M/yyyy}");
            ViewBag.TargetGroup = (targetgroup == null) ? Resource.AllTargetGroups : targetgroup.Name;
            ViewBag.Sex = (sexid == null) ? Resource.AllSexes : (sexid == (int)Customer.sex.Male) ? Resource.Men : Resource.Women;
            ViewBag.TargetGroupID = new SelectList(db.TargetGroups.Where(r => r.CompanyID == companyid), "id", "Name");

            if (filetype == "")
            {
                var PerSexValues = (from p in data.GroupBy(r => r.Customer.Sex.ToString()) select new Classes.ChartClass.KeyValue { LabelText = p.Count().ToString(), LegentText = (p.Key == "Female") ? Resource.Women : Resource.Men, Value = p.Count() }).ToArray();
                var PerSexChart = new Classes.ChartClass(System.Web.UI.DataVisualization.Charting.SeriesChartType.Pie, PerSexValues);
                ViewBag.PerSex = PerSexChart.ImageString;

                var CustomersPerTargetGroupValues = (from p in data
                                            group p by new { p.TargetGroup } into g
                                            select new Classes.ChartClass.KeyValue
                                            {
                                                LabelText = g.Count().ToString(),
                                                LegentText = g.Key.TargetGroup,
                                                Value = g.Count()
                                            }).ToArray();

                var CustomersPerTargetChart = new Classes.ChartClass(System.Web.UI.DataVisualization.Charting.SeriesChartType.Pie, CustomersPerTargetGroupValues);
                ViewBag.CustomersPerTargetGroup = CustomersPerTargetChart.ImageString;

                //if (data.GroupBy(r => r.TargetGroup).Count() > 3)
                //{

                //}

                Classes.ChartClass.KeyValue[] MeasurementsPerTargetGroupValues = (from p in data
                                                     group p by new { p.TargetGroup } into g
                                                     select new Classes.ChartClass.KeyValue
                                                     {
                                                         LabelText = g.Sum(r=>r.Measurements).ToString(),
                                                         LegentText = g.Key.TargetGroup,
                                                         Value = g.Sum(r => r.Measurements)
                                                     }).ToArray();

               
                var MeasurementsPerTargetChart = new Classes.ChartClass(System.Web.UI.DataVisualization.Charting.SeriesChartType.Pie, MeasurementsPerTargetGroupValues);
                ViewBag.MeasurementsPerTargetGroup = MeasurementsPerTargetChart.ImageString;
                return View(data);
            }

            LocalReport lr = new LocalReport();
            lr.LoadReportDefinition(await Classes.AzureStorageClass.GetRDLC("reportdocuments", "StatisticsOfMeasurements.rdlc"));
            //lr.ReportPath = Server.MapPath(System.IO.Path.Combine("~/Reports", "Documents", "StatisticsOfMeasurements.rdlc"));

            lr.DataSources.Add(new ReportDataSource("CompanyDT", new[] { company.Picture.Logo }.ToList()));
            var ReportData = new[] {
                new { ReportFieldName = "Περίδος", ReportFieldValue = string.Format($"{fromdate:d/M/yyyy} - {todate:d/M/yyyy}") },
            new { ReportFieldName = "Ομάδα", ReportFieldValue = (string)ViewBag.TargetGroup },
            new { ReportFieldName = "Φύλλο", ReportFieldValue = (string)ViewBag.Sex }
            }.ToList();
            lr.DataSources.Add(new ReportDataSource("ReportFieldDataDT", ReportData));
            lr.DataSources.Add(new ReportDataSource("StatisticsDT", data.ToList()));

            lr.Refresh();
            string reportType = filetype;
            string extension = (filetype == "IMAGE") ? "tiff" : (filetype == "PDF") ? "pdf" : (filetype == "EXCEL") ? "xls" : "doc";// (filetype != "PDF") ? "xlsx" : "pdf";// id;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>" + reportType + "</OutputFormat>" +
            "  <PageWidth>21cm</PageWidth>" +
            "  <PageHeight>29.7cm</PageHeight>" +
            "  <MarginTop>1cm</MarginTop>" +
            "  <MarginLeft>1.5cm</MarginLeft>" +
            "  <MarginRight>1.5cm</MarginRight>" +
            "  <MarginBottom>1cm</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderBytes;

            renderBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            return File(renderBytes, mimeType, string.Format($"Report.{extension}"));
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}