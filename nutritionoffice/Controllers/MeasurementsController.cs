using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using nutritionoffice.Models;
using System.Web.Helpers;
using System.Collections.Generic;
using Microsoft.Reporting.WebForms;
using System.Threading;
using System.Resources;
using System.Reflection;
using nutritionoffice.Reports.DataSources;

namespace nutritionoffice.Controllers
{
    [Authorize(Roles = "Administrator,Owner")]
    public class MeasurementsController : MyBaseController
    {
        private ndbContext db = new ndbContext();

        [HttpPost]
        public JsonResult GetBMIData(string customerid, string sex, string width, string height)
        {
            var BMI = new ViewModels.BMI((sex == "girl") ? ViewModels.BMI.Sex.Girl : ViewModels.BMI.Sex.Boy,
                        new System.Drawing.Size(int.Parse(width), int.Parse(height)),
                        new System.Drawing.Printing.Margins(20, 20, 20, 20));

            int[][] CustomerLine = null;
            if (customerid != null)
            {
                Customer customer = db.Customers.Find(Convert.ToInt32(customerid));
                if (customer != null)
                {
                    sex = (customer.Sex == Customer.sex.Male) ? "boy" : "girl";
                    IEnumerable<Measurement> customermeasurements = customer.Measurements.Where(r => r.Height != 0 && r.Weight != 0);
                    if (customermeasurements != null && customermeasurements.Count() > 0)
                    {
                        CustomerLine = (from p in customermeasurements let Data = new { Age = ((p.Date - p.Customer.BirthDate).TotalDays / 365), BMIValue = Convert.ToDouble(p.Weight / (p.Height * p.Height)) } select BMI.GetCoOrdinates(Data.Age, Data.BMIValue)).ToArray();
                    }
                }
                BMI.Customer = customer.FullName;
                BMI.CustomerSex = (customer.Sex == Customer.sex.Male) ? ViewModels.BMI.Sex.Boy : ViewModels.BMI.Sex.Girl;
            }
            return Json(new
            {
                Result = "Success",
                VerticalLines = BMI.VerticalLines,
                HorizontalLines = BMI.HorizontalLines,
                AgesScale = BMI.AgesScale,
                BMIScale = BMI.BMIScale,
                LineValues = BMI.LineValues,
                CustomerLine = CustomerLine
            }, JsonRequestBehavior.AllowGet);
        }
        // GET: Measurements
        public async Task<ActionResult> Index()
        {
            try
            {
                int CompID = CompanyID();
                var measurements = db.Measurements.Include(m => m.Customer).Where(r => r.Customer.CompanyID == CompID);
                return View(await measurements.ToListAsync());
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // GET: Measurements/Details/5
        public async Task<ActionResult> Details(string measurementids)
        {
            try
            {
                var ids = measurementids.Split(new[] { ',' }).Select(r => Convert.ToInt32(r)).ToArray();
                if (ids == null || ids.Count() == 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                int CompID = CompanyID();

                List<Measurement> measurements = await db.Measurements.OrderBy(r => r.Date).Where(r => ids.Any(mi => mi == r.id)).ToListAsync();
                db.Entry(measurements.FirstOrDefault()).Reference("Customer").Load();
                if (measurements == null || measurements.FirstOrDefault().Customer.CompanyID != CompID)
                {
                    return HttpNotFound();
                }
                return View(measurements);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        public ActionResult MeasurementData(int id)
        {
            var measurement = db.Measurements.Find(id);
            return PartialView("MeasurementsView", measurement);
        }

        public ActionResult CreateGraph(List<Int32> measurementsid)
        {
            Measurement[] measurements = db.Measurements.Where(r => measurementsid.Any(r1 => r1 == r.id)).ToArray();

            var chart = new Chart(width: 1000, height: 400, theme: ChartTheme.Blue);
            chart.SetXAxis("Ηλικία", min: 0, max: 19);
            chart.SetYAxis("BMI", min: 13, max: 19);
            chart.AddSeries(
                name: "L-i-n-e-1", markerStep: 10,
                chartType: "Line",
                xValue: new[] { "0", "0.5", "1", "1.5", "2", "2.5", "3", "3.5", "4", "4.5", "5", "5.5", "6", "6.5", "7", "7.5", "8", "8.5", "9", "9.5", "10", "10.5", "11", "11.5", "12", "12.5", "13", "13.5", "14", "14.5", "15", "15.5", "16", "16.5", "17", "17.5", "18" },
                yValues: new[] { "13", "14.2", "14.5", "14.45", "14.25", "14", "13.8", "13.6", "13.6", "13.6", "13.65", "13.65", "13.65", "13.7", "13.9", "14", "14.1", "14.2", "14.3", "14.45", "14.6", "14.8", "15.05", "15.35", "15.55", "15.9", "16.2", "16.5", "16.8", "17", "17.2", "17.5", "17.8", "18", "18.2", "18.3", "18.6" });
            //chart.AddSeries(
            //    name: "L-i-n-e-1", 
            //    chartType: "Line", 
            //    xValue: new[] { "0","0.5","1","1.5","2","2.5","3","3.5","4","4.5","5","5.5","6","6.5","7","7.5","8","8.5","9","9.5","10","10.5","11","11.5","12","12.5","13","13.5","14","14.5","15","15.5","16","16.5","17","17.5","18" },
            //    yValues: new[] {"13","14.2","14.5","14.45","14.25","14","13.8","13.6","13.6","13.6","13.65","13.65","13.65","13.7","13.9","14","14.1","14.2","14.3","14.45","14.6","14.8","15.05","15.35","15.55","15.9","16.2","16.5","16.8","17","17.2","17.5","17.8","18","18.2","18.3","18.6"});
            //chart.AddSeries(
            //    name: "L-i-n-e-2",
            //   chartType: "Line",
            //   xValue: new[] {"0","0.5","1","1.5","2","2.5","3","3.5","4","4.5","5","5.5","6","6.5","7","7.5","8","8.5","9","9.5","10","10.5","11","11.5","12","12.5","13","13.5","14","14.5","15","15.5","16","16.5","17","17.5","18" },
            //   yValues: new[] {"14","14.9","15.4","15.35","15.05","14.8","14.55","14.45","14.4","14.3","14.4","14.45","14.5","14.6","14.8","14.9","15.05","15.3","15.4","15.55","15.75","16","16.25","16.4","16.7","16.9","17.3","17.5","17.95","18.2","18.6","18.8","19.15","19.45","19.7","20","20.35" });
            //chart.AddSeries(
            //    name: "L-i-n-e-3",
            //   chartType: "Line",
            //   xValue: new[] { "0", "0.5", "1", "1.5", "2", "2.5", "3", "3.5", "4", "4.5", "5", "5.5", "6", "6.5", "7", "7.5", "8", "8.5", "9", "9.5", "10", "10.5", "11", "11.5", "12", "12.5", "13", "13.5", "14", "14.5", "15", "15.5", "16", "16.5", "17", "17.5", "18" },
            //   yValues: new[] { "15","16","16.4","16.3","15.95","15.6","15.4","15.2","15.1","15.1","15.2","15.25","15.5","15.6","15.8","16","16.2","16.5","16.7","17","17.2","17.4","17.6","17.8","18.1","18.4","18.6","19","19.4","19.7","20.1","20.5","20.8","21.1","21.4","21.55","21.8" });
            //chart.AddSeries(
            //    name: "L-i-n-e-4",
            //   chartType: "Line",
            //   xValue: new[] { "0", "0.5", "1", "1.5", "2", "2.5", "3", "3.5", "4", "4.5", "5", "5.5", "6", "6.5", "7", "7.5", "8", "8.5", "9", "9.5", "10", "10.5", "11", "11.5", "12", "12.5", "13", "13.5", "14", "14.5", "15", "15.5", "16", "16.5", "17", "17.5", "18" },
            //   yValues: new[] {"16","17","17.5","17.4","16.7","16.5","16.3","16.1","16.1","16.1","16.2","16.45","16.6","16.8","17","17.4","17.6","18","18.4","18.7","19","19.4","19.6","20","20.2","20.5","20.7","21","21.4","21.6","22","22.4","22.7","23","23.5","23.9","24.3"});
            //chart.AddSeries(
            //    name: "L-i-n-e-5",
            //   chartType: "Line",
            //   xValue: new[] { "0", "0.6", "1", "1.5", "2", "2.5", "3", "3.5", "4", "4.5", "5", "5.5", "6", "6.5", "7", "7.5", "8", "8.5", "9", "9.5", "10", "10.5", "11", "11.5", "12", "12.5", "13", "13.5", "14", "14.5", "15", "15.5", "16", "16.5", "17", "17.5", "18" },
            //   yValues: new[] {"17.2","17.5","17.7","17.6","17.4","17","16.6","16.5","16.45","16.5","16.8","17","17.2","17.5","17.8","18","18.4","18.7","19.2","19.6","19.9","20.3","20.6","21","21.3","21.6","21.9","22.2","22.4","22.8","23.1","23.4","23.75","24.1","24.45","24.8","25.1"});
            //chart.AddSeries(
            //    name: "L-i-n-e-6",
            //   chartType: "Line",
            //   xValue: new[] { "0", "0.7", "1", "1.5", "2", "2.5", "3", "3.5", "4", "4.5", "5", "5.5", "6", "6.5", "7", "7.5", "8", "8.5", "9", "9.5", "10", "10.5", "11", "11.5", "12", "12.5", "13", "13.5", "14", "14.5", "15", "15.5", "16", "16.5", "17", "17.5", "18" },
            //   yValues: new[] {"17","17.9","18.4","18.3","18","17.6","17.4","17.2","17.3","17.4","17.6","18","18.2","18.5","18.9","19.4","19.8","20.3","20.8","21.2","21.6","22","22.3","22.5","23","23.2","23.6","24","24.25","24.5","24.9","25.2","25.5","25.8","26.2","26.5","26.9"});
            //chart.AddSeries(
            //    name: "L-i-n-e-7",
            //   chartType: "Line",
            //   xValue: new[] { "0", "0.8", "1", "1.5", "2", "2.5", "3", "3.5", "4", "4.5", "5", "5.5", "6", "6.5", "7", "7.5", "8", "8.5", "9", "9.5", "10", "10.5", "11", "11.5", "12", "12.5", "13", "13.5", "14", "14.5", "15", "15.5", "16", "16.5", "17", "17.5", "18" },
            //   yValues: new[] {"17.7","18.7","19.2","19.3","19.1","18.8","18.6","18.65","18.7","19","19.3","19.8","20.4","20.9","21.4","22","22.4","22.9","23.4","23.85","24.2","24.55","24.9","25.4","25.9","26.4","26.9","27.1","27.4","27.6","28","28.35","28.7","29","29.4","29.7","30"});
            //chart.AddSeries(
            //    name: "L-i-n-e-8",
            //  chartType: "Line",
            //  xValue: new[] { "0", "0.8", "1", "1.5", "2", "2.5", "3", "3.5", "4", "4.5", "5", "5.5", "6", "6.5", "7", "7.5", "8", "8.5", "9", "9.5", "10", "10.5", "11", "11.5", "12", "12.5", "13", "13.5", "14", "14.5", "15", "15.5", "16", "16.5", "17", "17.5", "18" },
            //  yValues: new[] {"18.7","19.55","20.2","20.2","20","19.75","19.7","19.9","20.4","20.65","21.1","21.65","22.5","23.2","23.9","24.5","25","25.5","26","26.6","27.1","27.5","27.9","28.3","28.8","29.2","29.6","29.9","30.2","30.6","31.1","31.5","31.8","32.2","32","33","33.3"});
            var bytes = chart.GetBytes("png");
            return Json(new { base64Data = Convert.ToBase64String(bytes) }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetSkinFolds(string sex,double age,
            double abdominal,
            double axilla,
            double chest,
            double subscapular,
            double suprailiac, 
            double thigh,
            double tricepts
            )
        {
            var threesitebodyfat = 0d;
            var threesitebodydensity = 0d;
            var fourssitebodyfat = 0d;
            var foursitebodydensity = 0d;
            var sevensitebodyfat = 0d;
            var sevensitebodydensity = 0d;
            double sumofskinfolds = 0d;
            switch (sex)
            {
                case "man":
                    sumofskinfolds = abdominal + tricepts + thigh + suprailiac;
                    fourssitebodyfat = (0.29288 * sumofskinfolds) - (0.0005 * (sumofskinfolds * sumofskinfolds)) + (0.15845 * age) - 5.76377;

                    break;
                default:
                    break;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        // GET: Measurements/Create
        public ActionResult Create(int? CustomerID, int? TargetGroupID)
        {
            try
            {
                int CompID = CompanyID();
                IQueryable<Customer> companycustomers = db.Customers.Where(r => r.CompanyID == CompID);
                ViewBag.CustomerID = new SelectList(companycustomers, "id", "LastName", CustomerID);
                Measurement defaultmeasurement = new Measurement
                {
                    BloodPressure = 8,
                    cholesterol = 150,
                    Date = DateTime.Today,
                    Fat = 10.1m,
                    Ferrum = 20,
                    Height = 1.75m,
                    Notes = "",
                    spelter = 0.4m,
                    triglycerides = 180,
                    Weight = 75,
                    e_FAT = 0
                };
                Measurement measurement = new Measurement
                {
                    Date = DateTime.Today,
                };
                ViewBag.Customer = null;
                ViewBag.TargetGroupID = TargetGroupID;
                Measurement existingmeasurement = null;
                if (CustomerID.HasValue)
                {
                    Customer customer = db.Customers.Find(CustomerID.Value);
                    if (customer.CompanyID == CompID)
                    {
                        existingmeasurement = db.Measurements.Where(r => r.CustomerID == customer.id && r.Date == DateTime.Today).FirstOrDefault();
                        measurement.CustomerID = CustomerID.Value;
                        measurement.Height = customer.Measurements.OrderByDescending(r => r.Date).DefaultIfEmpty(defaultmeasurement).FirstOrDefault().Height;
                        ViewBag.Customer = customer;
                    }
                }
                if (existingmeasurement != null)
                {
                    return RedirectToAction("Edit", new { id = existingmeasurement.id });
                }
                return View(measurement);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // POST: Measurements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,CustomerID,Date,Height,Weight,Fat,WaistHipRatio,triglycerides,cholesterol,BloodPressure,Ferrum,spelter,Notes, e_FAT,Chest,Belly,Quadriceps,Triceps,Diceps,Ypoplatios,Iliac,Armpit,Shank,LowerMean")] Measurement measurement)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Measurements.Add(measurement);
                    await db.SaveChangesAsync();
                    var source = Request["TargetGroup"];
                    if (source != null && !string.IsNullOrEmpty(source))
                    {
                        return RedirectToAction("Index", "Customers", new { TargetGroupID = Convert.ToInt32(source) });
                    }
                    else
                    {
                        return RedirectToAction("Details", "Customers", new { id = measurement.CustomerID });
                    }

                }

                ViewBag.CustomerID = new SelectList(db.Customers, "id", "LastName", measurement.CustomerID);
                ViewBag.Customer = null;
                if (measurement.CustomerID > 0)
                {
                    ViewBag.Customer = db.Customers.Find(measurement.CustomerID);
                }
                return View(measurement);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // GET: Measurements/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                int CompID = CompanyID();
                Measurement measurement = await db.Measurements.FindAsync(id);
                if (measurement == null || measurement.Customer.CompanyID != CompID)
                {
                    return HttpNotFound();
                }
                ViewBag.Customer = measurement.Customer;
                IQueryable<Customer> companycustomers = db.Customers.Where(r => r.CompanyID == CompID);
                return View(measurement);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // POST: Measurements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,CustomerID,Date,Height,Weight,Fat,WaistHipRatio,triglycerides,cholesterol,BloodPressure,Ferrum,spelter,Notes, e_FAT,Chest,Belly,Quadriceps,Triceps,Diceps,Ypoplatios,Iliac,Armpit,Shank,LowerMean")] Measurement measurement)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(measurement).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Details", "Customers", new { id = measurement.CustomerID });
                }
                ViewBag.Customer = await db.Customers.FindAsync(measurement.CustomerID);
                return View(measurement);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // GET: Measurements/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                int CompID = CompanyID();
                Measurement measurement = await db.Measurements.FindAsync(id);
                if (measurement == null || measurement.Customer.CompanyID != CompID)
                {
                    return HttpNotFound();
                }
                return View(measurement);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // POST: Measurements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                Measurement measurement = await db.Measurements.FindAsync(id);
                int CustomerID = measurement.CustomerID;
                db.Measurements.Remove(measurement);

                await db.SaveChangesAsync();


                return RedirectToAction("Details", "Customers", new { id = CustomerID });
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [ValidateInput(false)]
        public async Task<ActionResult> PrintOut(string ids, string filetype = "IMAGE")
        {

            var measurementids = ids.Split(new[] { ',' }).Select(r => Convert.ToInt32(r)).ToArray();
            IEnumerable<Measurement> AllCustomerMeasurements = db.Measurements.Where(r => measurementids.Any(mi => mi == r.id));

            Customer customer = await db.Customers.FindAsync(AllCustomerMeasurements.FirstOrDefault().CustomerID);

            Company company = await db.Companies.FindAsync(customer.CompanyID);

            Picture picture = await db.Pictures.FindAsync(customer.CompanyID);

            LocalReport lr = new LocalReport();

            //#if DEBUG
            //            lr.ReportPath = Server.MapPath(System.IO.Path.Combine("~/Reports", "Documents", "CustomerMeasurements.rdlc"));
            //#else
            lr.LoadReportDefinition(await Classes.AzureStorageClass.GetRDLC("reportdocuments", "CustomerMeasurements.rdlc"));
            //#endif

            ReportParameterInfoCollection allparams = lr.GetParameters();

            ReportDataSource MeasurementSource = new ReportDataSource("Mesaurements", AllCustomerMeasurements.ToList());

            lr.DataSources.Add(MeasurementSource);

            ReportDataSource CustomerSource = new ReportDataSource("Customer", new Customer[] { customer }.ToList());

            lr.DataSources.Add(CustomerSource);

            ReportDataSource CompanySource = new ReportDataSource("Company", new Company[] { company }.ToList());

            lr.DataSources.Add(CompanySource);

            ReportDataSource BackgroundPicture = new ReportDataSource("BackgroundPicture", new Picture[] { picture }.ToList());

            lr.DataSources.Add(BackgroundPicture);

            ReportDataSource Calculations = new ReportDataSource("Calculations", (from p in AllCustomerMeasurements
                                                                                  select new
                                                                                  {
                                                                                      Date = p.Date,
                                                                                      Age = p.Age,
                                                                                      Height = (double)p.Height,
                                                                                      Weight = (double)p.Weight,
                                                                                      BMI = p.BMI,
                                                                                      BD7 = p.JP7.BodyDensity.GetValueOrDefault(0),
                                                                                      BF7 = p.JP7.BodyFat.GetValueOrDefault(0),
                                                                                      BD3 = p.JP3.BodyDensity.GetValueOrDefault(0),
                                                                                      BF3 = p.JP3.BodyFat.GetValueOrDefault(0)
                                                                                  }).ToList());

            lr.DataSources.Add(Calculations);
            var BMIValuesTable = new CustomerMeasurementsDS.BMIDataTable();
            //if (AllCustomerMeasurements.Where(r => r.Age <= 18d).Count() > 0)
            //{
            ViewModels.BMI bmi = new ViewModels.BMI((customer.Sex == Customer.sex.Male) ? ViewModels.BMI.Sex.Boy : ViewModels.BMI.Sex.Girl, new System.Drawing.Size(), new System.Drawing.Printing.Margins());
            var MaxAge = AllCustomerMeasurements.Max(r => r.Age);
            var MinAge = AllCustomerMeasurements.Min(r => r.Age);
            double AdultStartAge = (MinAge < 18d) ? 18d : MinAge - 1;
            double AdultEndAge = MaxAge + 1;
            double adultvalue;
            foreach (var item in bmi.Values[bmi.CustomerSex])
            {
                if (MinAge < 18d)
                {
                    for (int ageindex = 0; ageindex < bmi.Ages.Count(); ageindex += 1)
                    {
                        BMIValuesTable.AddBMIRow(LineIndex: item.Key,
                            LineName: " ",
                            Color: item.Value.StrokeData.Color,
                            DashName: item.Value.StrokeData.DashName,
                            Width: item.Value.StrokeData.WidthValue,
                            Age: bmi.Ages[ageindex], Value: item.Value.Values[ageindex]);
                    }
                }

                if (MaxAge > 18d)
                {

                    if (customer.Sex == Customer.sex.Male)
                    {
                        adultvalue = (item.Key == 1) ? 17.5 : (item.Key == 2) ? 19.1 : (item.Key == 3) ? 25.8 : (item.Key == 4) ? 27.3 : (item.Key == 5) ? 32.3 : (item.Key == 6) ? 35 : (item.Key == 7) ? 40 : (item.Key == 8) ? 50 : 70;

                    }
                    else
                    {
                        adultvalue = (item.Key == 1) ? 17.5 : (item.Key == 2) ? 20.7 : (item.Key == 3) ? 26.4 : (item.Key == 4) ? 27.8 : (item.Key == 5) ? 31.1 : (item.Key == 6) ? 35 : (item.Key == 7) ? 40 : (item.Key == 8) ? 50 : 70;
                    }

                    for (double i = AdultStartAge; i <= AdultEndAge; i += 0.5d)
                    {
                        BMIValuesTable.AddBMIRow(LineIndex: item.Key, LineName: " ", Color: item.Value.StrokeData.Color, DashName: item.Value.StrokeData.DashName, Width: item.Value.StrokeData.WidthValue, Age: i, Value: adultvalue);
                    }
                }
            }
            //}


            foreach (var item in AllCustomerMeasurements)
            {
                //double age = Math.Round((item.Date - customer.BirthDate).TotalDays / 365, 1);

                //double rest = age % 1;
                //if (rest != 0)
                //{
                //    age = (rest > 0.75) ? age = (int)age + 1 : (rest < 0.25) ? age = (int)age : age = (int)age + 0.5;
                //}

                BMIValuesTable.AddBMIRow(LineIndex: -1,
                    LineName: customer.FirstName,
                    Color: "red",
                    Age: item.Age,
                    Value: Math.Round((double)item.Weight / ((double)item.Height * (double)item.Height), 2),
                    Width: "5pt",
                    DashName: "Solid");
            }

            ReportDataSource BMIValues = new ReportDataSource("BMIValues", BMIValuesTable.AsEnumerable());

            lr.DataSources.Add(BMIValues);

            System.Globalization.CultureInfo ci = Thread.CurrentThread.CurrentCulture;

            ResourceManager RM = new ResourceManager(typeof(Resource));

            foreach (var param in allparams.Where(r => r.Name.StartsWith("Resource_")))
            {
                string ResourceName = param.Name.Replace("Resource_", "");
                if (RM.GetString(ResourceName) != null)
                {
                    lr.SetParameters(new ReportParameter(param.Name, RM.GetString(ResourceName, ci)));
                }

            }



            lr.SetParameters(new ReportParameter("pReportCulture", ci.Name));

            lr.Refresh();

            //System.Globalization.CultureInfo ci = Thread.CurrentThread.CurrentUICulture;

            //lr.SetParameters(new ReportParameter("DietStartDate", string.Format(Thread.CurrentThread.CurrentUICulture, $"{measurement.Date:ddd d/M/yyyy}")));

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
            "  <MarginTop>0.5cm</MarginTop>" +
            "  <MarginLeft>1.5cm</MarginLeft>" +
            "  <MarginRight>1.5cm</MarginRight>" +
            "  <MarginBottom>1cm</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderBytes;

            lr.ReleaseSandboxAppDomain();

            renderBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            return File(renderBytes, mimeType, string.Format($"{customer.FullName} - Measurements from {AllCustomerMeasurements.FirstOrDefault().Date:d-M-yyyy}.{extension}"));
        }

        public PartialViewResult MeasurementsDisplay(IEnumerable<int> measurementids)
        {
            IEnumerable<Measurement> measurements = db.Measurements.Where(r => measurementids.Any(f => r.id == f));
            return PartialView("MeasurementsDisplay", measurements);
        }

        // [ValidateInput(false)]
        public async Task<ActionResult> CreateReport(string ids, string filetype = "IMAGE")
        {
            var measurementids = ids.Split(new[] { ',' }).Select(r => Convert.ToInt32(r)).ToArray();
            IEnumerable<Measurement> AllCustomerMeasurements = db.Measurements.OrderBy(r => r.Date).Where(r => measurementids.Any(m => m == r.id));

            Customer customer = await db.Customers.FindAsync(AllCustomerMeasurements.FirstOrDefault().CustomerID);

            Company company = await db.Companies.FindAsync(customer.CompanyID);

            Picture picture = await db.Pictures.FindAsync(customer.CompanyID);

            LocalReport lr = new LocalReport();

            //#if DEBUG
            //            lr.ReportPath = Server.MapPath(System.IO.Path.Combine("~/Reports", "Documents", "CustomerMeasurementsReport.rdlc"));
            //#else
            lr.LoadReportDefinition(await Classes.AzureStorageClass.GetRDLC("reportdocuments", "CustomerMeasurementsReport.rdlc"));
            //#endif

            ReportParameterInfoCollection allparams = lr.GetParameters();

            ReportDataSource MeasurementSource = new ReportDataSource("Mesaurements", AllCustomerMeasurements.ToList());

            lr.DataSources.Add(MeasurementSource);

            ReportDataSource CustomerSource = new ReportDataSource("Customer", new Customer[] { customer }.ToList());

            lr.DataSources.Add(CustomerSource);

            ReportDataSource CompanySource = new ReportDataSource("Company", new Company[] { company }.ToList());

            lr.DataSources.Add(CompanySource);

            ReportDataSource BackgroundPicture = new ReportDataSource("BackgroundPicture", new Picture[] { picture }.ToList());

            lr.DataSources.Add(BackgroundPicture);

            ViewModels.BMI bmi = new ViewModels.BMI((customer.Sex == Customer.sex.Male) ? ViewModels.BMI.Sex.Boy : ViewModels.BMI.Sex.Girl, new System.Drawing.Size(), new System.Drawing.Printing.Margins());

            var BMIValuesTable = new CustomerMeasurementsDS.BMIDataTable();
            foreach (var item in bmi.Values[bmi.CustomerSex])
            {
                for (int ageindex = 0; ageindex < bmi.Ages.Count(); ageindex += 1)
                {
                    BMIValuesTable.AddBMIRow(LineIndex: item.Key,
                        LineName: " ",
                        Color: item.Value.StrokeData.Color,
                        DashName: item.Value.StrokeData.DashName,
                        Width: item.Value.StrokeData.WidthValue,
                        Age: bmi.Ages[ageindex], Value: item.Value.Values[ageindex]);
                }
            }

            foreach (var item in AllCustomerMeasurements)
            {
                BMIValuesTable.AddBMIRow(LineIndex: -1,
                    LineName: customer.FirstName,
                    Color: "red",
                    Age: item.Age,
                    Value: Math.Round((double)item.Weight / ((double)item.Height * (double)item.Height), 2),
                    Width: "5pt",
                    DashName: "Solid");
            }

            ReportDataSource BMIValues = new ReportDataSource("BMI", BMIValuesTable.AsEnumerable());

            lr.DataSources.Add(BMIValues);

            System.Globalization.CultureInfo ci = Thread.CurrentThread.CurrentCulture;

            ResourceManager RM = new ResourceManager(typeof(Resource));

            foreach (var param in allparams.Where(r => r.Name.StartsWith("Resource_")))
            {
                string ResourceName = param.Name.Replace("Resource_", "");
                if (RM.GetString(ResourceName) != null)
                {
                    lr.SetParameters(new ReportParameter(param.Name, RM.GetString(ResourceName, ci)));
                }

            }

            lr.SetParameters(new ReportParameter("pReportCulture", ci.Name));

            lr.Refresh();

            string reportType = filetype;// (filetype != "PDF") ? "Excel" : filetype;// id;
            string extension = (filetype == "IMAGE") ? "tiff" : (filetype == "PDF") ? "pdf" : (filetype == "EXCEL") ? "xls" : "doc";// (filetype != "PDF") ? "xlsx" : "pdf";// id;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + reportType + "</OutputFormat>" +
            "  <PageWidth>21cm</PageWidth>" +
            "  <PageHeight>29.7cm</PageHeight>" +
            "  <MarginTop>0.5cm</MarginTop>" +
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


            return File(renderBytes, mimeType, string.Format($"{customer.FullName} - Measurements from {AllCustomerMeasurements.FirstOrDefault().Date:d-M-yyyy}.{extension}"));
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
