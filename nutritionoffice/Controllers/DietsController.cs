using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using nutritionoffice.Models;
using Microsoft.Reporting.WebForms;
using System.Threading;
using static nutritionoffice.Classes.Communicator;
using System.Net.Mail;
using System.Data.Entity.Validation;
using System.Data.Entity;
using PagedList;

namespace nutritionoffice.Controllers
{
    [Authorize(Roles = "Administrator,Owner")]
    public class DietsController : MyBaseController
    {
        private ndbContext db = new ndbContext();

        [HttpPost]
        public async Task<JsonResult> SaveNotes(int id, string Text)
        {
            try
            {
                DietDailyMeal meal = await db.DietDailyMeals.FindAsync(id);
                meal.Notes = Text;
                db.Entry(meal).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { Result = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return Json(new { Result = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<ActionResult> updatefooddata(int id)
        {
            Food food = await db.Foods.FindAsync(id);
            return PartialView("updatefooddata", food);
        }

        [HttpPost]
        public async Task<JsonResult> updatefooddata(int id, string greekname, float equivalent, bool isbreakfast, bool issnack, bool islunch, bool isdinner)
        {
            try
            {
                Food initialfood = await db.Foods.FindAsync(id);
                initialfood.GreekName = greekname;
                initialfood.Equivalent = Convert.ToDecimal(equivalent);
                initialfood.IsBreakfast = isbreakfast;
                initialfood.IsSnack = issnack;
                initialfood.IsLunch = islunch;
                initialfood.IsDinner = isdinner;
                db.Entry(initialfood).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { Result = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return Json(new { Result = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteMeal(int id)
        {
            try
            {
                DietDetail detail = await db.DietDetails.FindAsync(id);
                db.DietDetails.Remove(detail);
                await db.SaveChangesAsync();
                return Json(new { Result = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return Json(new { Result = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<JsonResult> GetFoods(string term, string diettype, string mealtype)
        {
            try
            {
                int companyid = CompanyID();
                IQueryable<Food> foodquery = db.Foods.Include(r=>r.FoodCategory);
                if (!User.IsInRole("Administrator"))
                {
                        foodquery = foodquery.Where(r => r.FoodCategory.CompanyID == null || r.FoodCategory.CompanyID == companyid);
                }
                var foods = await foodquery.OrderBy(r => r.FoodCategory.GreekName).Where(r => r.GreekName.ToLower().Contains(term.ToLower())).ToListAsync();
                switch (mealtype)
                {
                    case "1":
                        foods = foods.Where(r => r.IsBreakfast).ToList();
                        break;
                    case "2":
                    case "4":
                        foods = foods.Where(r => r.IsSnack).ToList();
                        break;
                    case "3":
                        foods = foods.Where(r => r.IsLunch).ToList();
                        break;
                    default:
                        foods = foods.Where(r => r.IsDinner).ToList();
                        break;
                }
                switch (diettype)
                {
                    case "CollagentSynthesis":
                        foods = foods.Where(r => r.IsCollagene).ToList();
                        break;
                    case "Detox":
                        foods = foods.Where(r => r.IsDetox).ToList();
                        break;
                    case "Antioxidant":
                        foods = foods.Where(r => r.IsAntioxidant).ToList();
                        break;
                    case "Diatrofogenomiki":
                        foods = foods.Where(r => r.IsDiatrofogenomiki).ToList();
                        break;
                    case "Menopause":
                        foods = foods.Where(r => r.IsMenopause).ToList();
                        break;
                    default:
                        break;
                }

                //var result = (from p in foods select new { GreekName = p.GreekName, id = p.id, FoodCategoryName = p.FoodCategory.GreekName }).Take(200).Distinct();
                string[] ValidRoles = new string[] { "Administrator","CanEdit"};
                var result = (from p in foods select new { GreekName = p.GreekName, id = p.id, FoodCategoryName = p.FoodCategory.GreekName, CanEdit= ValidRoles.Any(r=>User.IsInRole(r)) || p.FoodCategory.CompanyID==companyid }).Take(200).Distinct();

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return Json(HttpStatusCode.InternalServerError, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> CopyMeal(int id, int today)
        {
            try
            {
                DietDailyMeal meal = await db.DietDailyMeals.FindAsync(id);
                db.Entry(meal).Collection(r => r.DietDetails).Load();
                List<DietDetail> copies = meal.DietDetails.AsParallel().ToList();
                string mealnotes = meal.Notes;

                List<DietDailyMeal> destinationmeals = db.DietDailyMeals.Where(r => r.DietID == meal.DietID && r.MealIndex == meal.MealIndex && r.DayIndex != meal.DayIndex).ToList();
                if (today != -1) { destinationmeals = destinationmeals.Where(r => r.DayIndex == today).ToList(); }
                foreach (var destinationmeal in destinationmeals)
                {
                    db.Entry(destinationmeal).Collection(r => r.DietDetails).Load();
                    Array.ForEach(destinationmeal.DietDetails.ToArray(), r => db.DietDetails.Remove(r));
                    foreach (var item in copies)
                    {
                        db.Entry(item).State = EntityState.Detached;
                        item.DietDailyMealID = destinationmeal.id;
                        db.DietDetails.Add(item);
                        db.Entry(item).State = EntityState.Added;
                    }
                    destinationmeal.Notes = mealnotes;
                    db.Entry(destinationmeal).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return Json(new { Result = "Success", UpdateDay = today, Notes = meal.Notes }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return Json(new { Result = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Index(int? page)
        {
            try
            {
                int pageSize = 10;
                int CompID = CompanyID();
                var diets = db.Diets.Include(d => d.Customer).OrderByDescending(r => r.StartDate).Where(r => r.Customer.CompanyID == CompID);
                int pageNumber = (page ?? 1);
                //return View(await diets.ToListAsync());
                return View(diets.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ActionResult> SingleDateMeal(int id)
        {
            DietDailyMeal dailymeal = await db.DietDailyMeals.FindAsync(id);
            db.Entry(dailymeal).Collection(r => r.DietDetails).Load();
            return PartialView("SingleDateMeal", dailymeal);
        }

        public async Task<ActionResult> SingleDietDetail(int id)
        {
            DietDetail detail = await db.DietDetails.FindAsync(id);
            return PartialView("SingleDietDetail", detail);
        }

        public async Task<ActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                int CompID = CompanyID();
                Diet diet = await db.Diets.FindAsync(id);
                if (diet == null || diet.Customer.CompanyID != CompID)
                {
                    return HttpNotFound();
                }
                db.Entry(diet).Collection(r => r.DietDailyMeals).Load();
                Array.ForEach(diet.DietDailyMeals.ToArray(), r => db.Entry(r).Collection(k => k.DietDetails).Load());
                ViewBag.DietTypeID = new SelectList(Enum.GetNames(typeof(Diet.DietType)), diet.Type);
                //IQueryable<Food> foods = db.Foods.OrderBy(r => r.GreekName);
                //switch (diet.Type)
                //{
                //    case Diet.DietType.CollagentSynthesis:
                //        foods = foods.Where(r => r.IsCollagene);
                //        break;
                //    case Diet.DietType.Detox:
                //        foods = foods.Where(r => r.IsDetox);
                //        break;
                //    case Diet.DietType.Diatrofogenomiki:
                //        foods = foods.Where(r => r.IsDiatrofogenomiki);
                //        break;
                //    case Diet.DietType.Menopause:
                //        foods = foods.Where(r => r.IsMenopause);
                //        break;
                //    case Diet.DietType.Αntioxidant:
                //        foods = foods.Where(r => r.IsAntioxidant);
                //        break;
                //    default:
                //        foods = foods.Take(500);
                //        break;
                //}
                var GroupList = new[] {
                    new { Display=Resource.BasicChoice, Value=0},
                    new { Display = Resource.FirstAlternative, Value = 1},
                    new { Display = Resource.SecondAlternative, Value = 2 }
                };
                ViewBag.GroupID = new SelectList(GroupList, "Value", "Display", 0);
                //ViewBag.FoodID = new SelectList(foods, "id", "GreekName");
                //ViewBag.InitialFood = foods.FirstOrDefault();
                Session["diet"] = diet;
                return View(diet);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        public JsonResult SaveDetailToMeal(int mealid, decimal quantity, string quantitydescription, int foodid, int groupid = 0, int dietdetailid = 0)
        {
            DietDailyMeal meal = db.DietDailyMeals.Find(mealid);
            try
            {
                if (meal == null) { throw new Exception("Το meal δεν υπάρχει"); }
                DietDetail detail = db.DietDetails.Find(dietdetailid);
                if (detail == null)
                {
                    detail = new DietDetail { DietDailyMealID = mealid, FoodID = foodid, Group = groupid, Quantity = quantity, QuantityType = quantitydescription };
                    db.DietDetails.Add(detail);
                }
                else
                {
                    detail.FoodID = foodid;
                    detail.Group = groupid;
                    detail.Quantity = quantity;
                    detail.QuantityType = quantitydescription;
                    db.Entry(detail).State = EntityState.Modified;
                }
                db.SaveChanges();
                return Json(new { Result = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return Json(HttpStatusCode.InternalServerError, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult updatedaytotalslink(int dietid, int dayindex)
        {
            ViewModels.dailytotals totals = new ViewModels.dailytotals(dietid: dietid, dayindex: dayindex);
            return PartialView("updatedaytotalslink", totals);
        }
        public ActionResult Create(int? id)
        {
            try
            {
                int CompID = CompanyID();
                var customers = db.Customers.OrderBy(r => r.LastName).ThenBy(r => r.FirstName).Where(r => r.CompanyID == CompID);
                ViewBag.CustomerID = new SelectList(customers, "id", "FullName", id);
                var prototypes = db.Diets.Include("Customer").Where(r => r.Customer.CompanyID == CompID && !string.IsNullOrEmpty(r.DietName)).GroupBy(r => new { r.DietName }).Select(r => new { ID = r.FirstOrDefault().ID, DietName = r.FirstOrDefault().DietName });
                ViewBag.PrototypeID = new SelectList(prototypes, "ID", "DietName");
                ViewModels.dietfromprototype diet = new ViewModels.dietfromprototype { Diet = new Diet { Type = Diet.DietType.GeneralUse, StartDate = DateTime.Today, DietDailyMeals = new List<DietDailyMeal>(), CustomerID = id.GetValueOrDefault(0), DietName = "" } };

                return View(diet);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ViewModels.dietfromprototype diet)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int newdietid = 0;
                    if (diet.DietID != null && diet.DietID.Value > 0)//Διαβάζουμε τα στοιχεία από το πρωτότυπο
                    {
                        Diet pdiet = await db.Diets.FindAsync(diet.DietID.Value);
                        db.Entry(pdiet).Collection(r => r.DietDailyMeals).Load();
                        List<DietDailyMeal> dietdailymeals = pdiet.DietDailyMeals.ToList();
                        db.Entry(pdiet).State = EntityState.Detached;
                        pdiet.ID = 0;
                        pdiet.CustomerID = diet.Diet.CustomerID;
                        pdiet.DietName = diet.Diet.DietName;
                        pdiet.StartDate = diet.Diet.StartDate;
                        db.Diets.Add(pdiet);
                        foreach (var item in dietdailymeals)
                        {
                            db.Entry(item).State = EntityState.Detached;
                            item.id = 0;
                            item.Diet = pdiet;
                            db.DietDailyMeals.Add(item);
                        }
                        await db.SaveChangesAsync();
                        newdietid = pdiet.ID;
                    }
                    else
                    {
                        db.Diets.Add(diet.Diet);
                        diet.Diet.DietDailyMeals = new List<DietDailyMeal>();
                        for (int d = 1; d < 8; ++d)
                        {
                            for (int m = 0; m < 5; ++m)
                            {
                                diet.Diet.DietDailyMeals.Add(new DietDailyMeal { DayIndex = d, MealIndex = m, Diet = diet.Diet, DietDetails = new List<DietDetail>() });
                            }
                        }
                        try
                        {
                        await db.SaveChangesAsync();
                        newdietid = diet.Diet.ID;
                        }
                        catch(Exception ex1)
                        {
                            Console.WriteLine();
                        }

                    }

                    
                    return RedirectToAction("Details", new { id = newdietid });// RedirectToAction("Index");
                }
                int CompID = CompanyID();
                var customers = db.Customers.OrderBy(r => r.LastName).ThenBy(r => r.FirstName).Where(r => r.CompanyID == CompID);
                ViewBag.CustomerID = new SelectList(customers, "id", "FullName", diet.Diet.CustomerID);
                var prototypes = db.Diets.Include("Customer").Where(r => r.Customer.CompanyID == CompID && !string.IsNullOrEmpty(r.DietName)).GroupBy(r => new { r.DietName }).Select(r => new { ID = r.FirstOrDefault().ID, DietName = r.FirstOrDefault().DietName });
                ViewBag.PrototypeID = new SelectList(prototypes, "ID", "DietName", diet.DietID);
                return  View(diet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                int CompID = CompanyID();
                Diet diet = await db.Diets.FindAsync(id);
                if (diet == null || diet.Customer.CompanyID != CompID)
                {
                    return HttpNotFound();
                }
                IQueryable<Customer> companycustomers = db.Customers.Where(r => r.CompanyID == CompID);
                ViewBag.CustomerID = new SelectList(companycustomers, "id", "LastName", diet.CustomerID);
                return View(diet);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,CustomerID,StartDate,DietName,Type")] Diet diet)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(diet).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                int CompID = CompanyID();
                IQueryable<Customer> companycustomers = db.Customers.Where(r => r.CompanyID == CompID);
                ViewBag.CustomerID = new SelectList(companycustomers, "id", "LastName", diet.CustomerID);
                return View(diet);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                int CompID = CompanyID();
                Diet diet = await db.Diets.FindAsync(id);
                if (diet == null || diet.Customer.CompanyID != CompID)
                {
                    return HttpNotFound();
                }
                return View(diet);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                Diet diet = await db.Diets.FindAsync(id);
                db.Diets.Remove(diet);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [ValidateInput(false)]
        public async Task<ActionResult> PrintOut(int id, bool email = false, string filetype = "PDF", nMessage message = null)
        {

            Diet diet = await db.Diets.FindAsync(id);

            Customer customer = diet.Customer;

            Company company = await db.Companies.FindAsync(customer.CompanyID);

            LocalReport lr = new LocalReport();

//#if DEBUG
//            lr.ReportPath = Server.MapPath(System.IO.Path.Combine("~/Reports", "Documents", "DietPrintOut.rdlc"));
//#else
            lr.LoadReportDefinition(await Classes.AzureStorageClass.GetRDLC("reportdocuments", "DietPrintOut.rdlc"));
//#endif

            Reports.DataSources.DietDS ds = new Reports.DataSources.DietDS();

            string[] mealnames = new string[] { "Πρωϊνό", "Snack", "Γεύμα", "Snack", "Δείπνο" };

            List<DietDailyMeal> dietmeals = db.DietDailyMeals.Include(r => r.DietDetails).OrderBy(r => r.MealIndex).ThenBy(r => r.DayIndex).Where(r => r.DietID == id).ToList();
            foreach (var daymeal in dietmeals.GroupBy(r => r.MealIndex))
            {

                Reports.DataSources.DietDS.DietDetailsTableRow dtmealrow = ds.DietDetailsTable.NewDietDetailsTableRow();

                dtmealrow.MealName = mealnames[daymeal.FirstOrDefault().MealIndex];

                for (int dayindex = 1; dayindex < 8; ++dayindex)
                {
                    var thisDayMeal = daymeal.First(r => r.DayIndex == dayindex);
                    List<DietDetail> dayandmealdetails = thisDayMeal.DietDetails.OrderBy(r => r.Group).ToList();
                    if (dayandmealdetails.Count() != 0)
                    {
                        dtmealrow[dayindex] = string.Join("\r\n      - ή -      \r\n", dayandmealdetails.GroupBy(r => r.Group).Select(k =>
                        string.Join(",\r\n", k.Select(f => string.Format($"{f.Quantity:#,##} {f.QuantityType} {f.Food.GreekName}")))));
                    }
                    if (!string.IsNullOrEmpty(thisDayMeal.Notes)) { dtmealrow[dayindex] += "\r\n___________\r\n" + thisDayMeal.Notes.Trim(); }
                }
                ds.DietDetailsTable.Rows.Add(dtmealrow);
            }

            ReportDataSource rd = new ReportDataSource("DetailsDS", (from Reports.DataSources.DietDS.DietDetailsTableRow p in ds.DietDetailsTable.Rows
                                                                     select new
                                                                     {
                                                                         MealName = p.MealName,
                                                                         Day1 = p.Day1,
                                                                         Day2 = p.Day2,
                                                                         Day3 = p.Day3,
                                                                         Day4 = p.Day3,
                                                                         Day5 = p.Day3,
                                                                         Day6 = p.Day3,
                                                                         Day7 = p.Day3
                                                                     }).ToList());

            lr.DataSources.Add(rd);

            //System.Globalization.CultureInfo ci = Thread.CurrentThread.CurrentUICulture;

            lr.SetParameters(new ReportParameter("CustomerName", String.Format($"{diet.Customer.FullName}")));

            lr.SetParameters(new ReportParameter("DietStartDate", string.Format(Thread.CurrentThread.CurrentUICulture, $"{diet.StartDate:ddd d/M/yyyy}")));

            string reportType = filetype;// (filetype != "PDF") ? "Excel" : filetype;// id;
            string extension = (filetype == "IMAGE") ? "tiff" : (filetype=="PDF")?"pdf":(filetype == "EXCEL") ?"xls":"doc";// (filetype != "PDF") ? "xlsx" : "pdf";// id;
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

            if (email)
            {
                int companyid = CompanyID();
                string DestinationDirectory = Server.MapPath("~/files/documents/");
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(DestinationDirectory);
                if (!di.Exists)
                {
                    di.Create();
                    di.Refresh();
                }
                string filename = string.Format($"Diet - {Guid.NewGuid().ToString()}.{extension}");

                string DestinationPath = System.IO.Path.Combine(di.FullName, filename);
                using (System.IO.Stream stream = new System.IO.StreamWriter(DestinationPath, false).BaseStream)
                {
                    stream.Write(renderBytes, 0, renderBytes.Length);
                    stream.Flush();
                    stream.Close();
                }

                message.Type = nMessage.DeliveryType.email;
                message.Status = nMessage.MessageStatus.Pending;

                db.Messages.Add(message);
                try
                {
                    db.SaveChanges();
                    db.Entry(message).Reload();
                    List<Attachment> Attachments = new List<Attachment>();

                    System.IO.StreamReader fs = new System.IO.StreamReader(DestinationPath);
                    Attachments.Add(new Attachment(fs.BaseStream, string.Format($"{customer.FullName} - Diet {diet.StartDate:d-M-yyyy}.{extension}")));
                    if (!string.IsNullOrEmpty(message.Attatchments))
                    {
                        string[] attachmentnames = message.Attatchments.Split(new[] { ';' }).Select(r => r.Trim()).Distinct().ToArray();
                        foreach (string attachmentname in attachmentnames)
                        {
                            string attachmentfullpath = System.IO.Path.Combine(Server.MapPath("~/files/documents"), attachmentname);
                            System.IO.StreamReader attachmentstream = new System.IO.StreamReader(attachmentfullpath);
                            Recipe attachment = db.Recipes.Where(r => r.FileGuid == attachmentname).FirstOrDefault();
                            if (attachment != null && attachmentstream.BaseStream.CanRead)
                            {
                                System.IO.FileInfo afi = new System.IO.FileInfo(attachmentfullpath);
                                Attachments.Add(new Attachment(attachmentstream.BaseStream, string.Format($"{attachment.Name}{afi.Extension}")));
                            }
                        }
                        message.Attatchments = string.Join(";", Attachments.Select(r => r.Name).ToArray());
                        db.Entry(message).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    bool sent = await SendMailAsync(new SmtpMailClient
                    {
                        Credentials = new NetworkCredential { UserName = company.SMTPUserName, Password = company.SMTPPassword },
                        EnableSSL = company.SMTPEnableSSL,
                        Host = company.SMTPHost,
                        Port = company.SMTPPort
                    }, message, Attachments.ToArray());


                    return RedirectToAction("Details", "Customers", new { id = diet.CustomerID });
                }
                catch (DbEntityValidationException ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.EntityValidationErrors.FirstOrDefault().ValidationErrors.FirstOrDefault().ErrorMessage);
                }

            }
            else
            {
                return File(renderBytes, mimeType, string.Format($"{diet.Customer.FullName} - Diet from {diet.StartDate:d-M-yyyy}.{extension}"));
            }
        }

        public ActionResult AppendRecipies(string current, string addrecipe = "")
        {
            List<Recipe> currentrecipies = new List<Recipe>();
            if (!string.IsNullOrEmpty(current))
            {
                var ids = current.Split(new[] { ';' });
                foreach (var item in ids)
                {
                    currentrecipies.Add(db.Recipes.First(r => r.FileGuid == item));
                }
            }
            if (!string.IsNullOrEmpty(addrecipe)) { currentrecipies.Add(db.Recipes.First(r => r.FileGuid == addrecipe)); }
            int companyid = this.CompanyID();
            ViewBag.RecipeGroupID = new SelectList(db.RecipeCategories.Where(r => r.CompanyID == companyid), "id", "Name");
            ViewBag.RecipeID = new SelectList(db.Recipes.Where(r => r.RecipeCategory.CompanyID == companyid), "FileGuid", "Name");
            ViewBag.CurrentRecipes = string.Join(";", currentrecipies.Select(r => r.FileGuid.ToString()).ToArray());
            return PartialView(currentrecipies);
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
