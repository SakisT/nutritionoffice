using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using nutritionoffice.Models;
using PagedList;
using System.Data.Entity.Infrastructure;

namespace nutritionoffice.Controllers
{
    [Authorize(Roles = "Administrator,Owner")]
    public class CustomersController : MyBaseController
    {
        private ndbContext db = new ndbContext();

        // GET: Customers
        public ActionResult Index(string currentFilter, string searchString, int? page, int? Customertodisplay, int? TargetGroupID)
        {
            try
            {
                int pageSize = 10;
                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }
                int CompID = CompanyID();
                ViewBag.CurrentFilter = searchString;
                ViewBag.TargetGroupID = TargetGroupID;

                IQueryable<Customer> customers = db.Customers.Include(c => c.TargetGroup).Include(c => c.Appointments).Include(c => c.Diets).Include(c => c.Measurements).Include(c => c.Reminders).OrderBy(r => new { r.LastName, r.FirstName }).Where(r => r.CompanyID == CompID);

                if (TargetGroupID.HasValue) { customers = customers.Where(r => r.TargetGroupID == TargetGroupID.Value); }

                if (!String.IsNullOrEmpty(searchString)) { customers = customers.Where(r => r.LastName.Contains(searchString) | r.FirstName.Contains(searchString)); }

                if (Customertodisplay.HasValue)
                {
                    Customer currentcustomer = db.Customers.Find(Customertodisplay.Value);
                    page = (Array.IndexOf(customers.ToArray(), currentcustomer) / pageSize) + 1;
                }
                int pageNumber = (page ?? 1);
                return View(customers.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, string.Format("{0} - {1}", "CustomersController", "Index"));
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public ActionResult Search(string searchString)
        {
            try
            {
                int CompID = CompanyID();
                var customers = db.Customers.Include(c => c.TargetGroup).OrderBy(r => new { r.LastName, r.FirstName }).Where(r => r.CompanyID == CompID && (r.LastName.Contains(searchString) | r.FirstName.Contains(searchString)));
                if (customers.Count() == 1)
                {
                    return RedirectToAction("Details", new { id = customers.FirstOrDefault().id });
                }
                ViewBag.CurrentFilter = searchString;
                int pageSize = 10;
                int pageNumber = 1;
                return View("Index", customers.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, string.Format("{0} - {1}", "CustomersController", "Search"));
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // GET: Customers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                int CompID = CompanyID();
                Customer customer = await db.Customers.FindAsync(id);
                if (customer == null || customer.CompanyID != CompID)
                {
                    return HttpNotFound();
                }

                IQueryable<TargetGroup> validtargetgroups = db.TargetGroups.Where(r => r.CompanyID == CompID);
                ViewBag.TargetGroupID = new SelectList(validtargetgroups, "id", "Name", customer.TargetGroupID);
                DailyRecall recall = await db.DailyRecalls.Where(r => r.CustomerID == id.Value).FirstOrDefaultAsync();
                if (recall == null)
                {
                    recall = new DailyRecall
                    {
                        CustomerID = id.Value,
                        Alcohol = new LikesDislikes { Like = false },
                        Beef = new LikesDislikes { Like = false },
                        Cereals = new LikesDislikes { Like = false },
                        Chicken = new LikesDislikes { Like = false },
                        CottageCheese = new LikesDislikes { Like = false },
                        Fruits = new LikesDislikes { Like = false },
                        Hamburger = new LikesDislikes { Like = false },
                        InOilFood = new LikesDislikes { Like = false },
                        JunkFood = new LikesDislikes { Like = false },
                        Legumes = new LikesDislikes { Like = false },
                        Milk = new LikesDislikes { Like = false },
                        Nuts = new LikesDislikes { Like = false },
                        Pork = new LikesDislikes { Like = false },
                        Salads = new LikesDislikes { Like = false },
                        Turkey = new LikesDislikes { Like = false },
                        WhiteCheese = new LikesDislikes { Like = false },
                        YellowCheese = new LikesDislikes { Like = false },
                        Yoghurt = new LikesDislikes { Like = false }
                    };
                    db.DailyRecalls.Add(recall);
                    await db.SaveChangesAsync();
                }
                ViewBag.DailyRecall = recall;

                BasicQuestionnaire quest = await db.BasicQuestionnairies.Where(r => r.CustomerID == id.Value).FirstOrDefaultAsync();
                if (quest == null)
                {
                    quest = new BasicQuestionnaire
                    {
                        CustomerID = id.Value,
                        Allergies = new ExistingProblems { Exists = false },
                        Arthritis = new ExistingProblems { Exists = false },
                        Asthma = new ExistingProblems { Exists = false },
                        BreathingProblems = new ExistingProblems { Exists = false },
                        Bulimia = false,
                        CardioVascularProblems = new ExistingProblems { Exists = false },
                        Diabetes = new ExistingProblems { Exists = false },
                        HighBloodPressure = new ExistingProblems { Exists = false },
                        HighCholesterol = new ExistingProblems { Exists = false },
                        HighTriglycerides = new ExistingProblems { Exists = false },
                        Hypoglycemia = new ExistingProblems { Exists = false },
                        LackOfAppetite = false,
                        LowBloodPressure = new ExistingProblems { Exists = false },
                        OverWeightOnEarlyYears = false,
                        QuestionnareDate = DateTime.Today,
                        Ulcer = new ExistingProblems { Exists = false },
                        WeightDecreasedOnLastPeriod = false,
                        WeightIncreasedOnLastPeriod = false
                    };
                    db.BasicQuestionnairies.Add(quest);
                    await db.SaveChangesAsync();
                }
                ViewBag.BasicQuest = quest;
                return View(customer);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, string.Format("{0} - {1}", "CustomersController", "Details"));
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // GET: Customers/Create
        public ActionResult Create(int? TargetGroupID)
        {
            try
            {
                int CompID = CompanyID();
                IQueryable<TargetGroup> validtargetgroups = db.TargetGroups.Where(r => r.CompanyID == CompID);
                ViewBag.TargetGroupID = new SelectList(validtargetgroups, "id", "Name", TargetGroupID);
                ViewBag.GroupID = TargetGroupID;
                Customer customer = new Customer() { CompanyID = CompID, Sex = Customer.sex.Female, BirthDate = new DateTime(1980, 1, 1), LastName = "", FirstName = "", CustomerGUID = Guid.NewGuid() };//

                return View(customer);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, string.Format("{0} - {1}", "CustomersController", "Create"));
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public JsonResult checkifcustomerexists(string lastname, string firstname)
        {
            int companyid = CompanyID();
            string[] firstnameapproaches = new string[]
            {
                firstname,firstname.ToLower(),firstname.ToUpper(),lastname,lastname.ToLower(), lastname.ToUpper()
            }.ToArray();
            string[] lastnameapproaches = new string[]
            {
                firstname,firstname.ToLower(),firstname.ToUpper(),lastname,lastname.ToLower(), lastname.ToUpper()
            }.ToArray();
            IQueryable<Customer> CompanyCustomers = db.Customers.Include(r => r.TargetGroup).Where(r => r.CompanyID == companyid);
            Customer existingcustomer = CompanyCustomers.Where(r => lastnameapproaches.Any(l => l.Equals(r.LastName)) && firstnameapproaches.Any(f => f.Equals(r.FirstName))).FirstOrDefault();
            var Result = new { Exists = (existingcustomer != null), id = existingcustomer?.id, customername = existingcustomer?.FullName, TargetGroupName = existingcustomer?.TargetGroup.Name };
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,TargetGroupID,LastName,FirstName,BirthDate,Sex,Phone,Mobile,email,Facebook,City,Address,PostCode,Notes,CompanyID,CustomerGUID")] Customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    customer.CreatedOn = Classes.SharedClass.Now();
                    db.Customers.Add(customer);
                    await db.SaveChangesAsync();

                    var source = Request["TargetGroup"];

                    if (source != null && !string.IsNullOrEmpty(source))
                    {
                        return RedirectToAction("Create", "Measurements", new { CustomerID = customer.id, TargetGroupID = Convert.ToInt32(source) });
                    }
                    else
                    {
                        return RedirectToAction("Details", new { id = customer.id });
                    }
                    //Αν Έχει έρθει από Creation μέλους ενός TargetGroup πηγαίνει στο Measurements αλλιώς πηγαίνει στο Details του Customer

                }
                int CompID = CompanyID();
                if (customer.CompanyID == 0) { customer.CompanyID = CompID; }
                IQueryable<TargetGroup> validtargetgroups = db.TargetGroups.Where(r => r.CompanyID == CompID);
                ViewBag.TargetGroupID = new SelectList(validtargetgroups, "id", "Name", customer.TargetGroupID);
                return View(customer);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, string.Format("{0} - {1}", "CustomersController", "Create (POST)"));
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // GET: Customers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Customer customer = await db.Customers.FindAsync(id);
                int CompID = CompanyID();
                if (customer == null || customer.CompanyID != CompID)
                {
                    return HttpNotFound();
                }

                IQueryable<TargetGroup> validtargetgroups = db.TargetGroups.Where(r => r.CompanyID == CompID);
                ViewBag.TargetGroupID = new SelectList(validtargetgroups, "id", "Name", customer.TargetGroupID);

                return View(customer);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, string.Format("{0} - {1}", "CustomersController", "Edit"));
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<PartialViewResult> CustomerPartial(int id)
        {
            int CompID = CompanyID();
            IQueryable<TargetGroup> validtargetgroups = db.TargetGroups.Where(r => r.CompanyID == CompID);
            Customer customer = await db.Customers.FindAsync(id);
            ViewBag.TargetGroupID = new SelectList(validtargetgroups, "id", "Name", customer.TargetGroupID);
            return PartialView(customer);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("CustomerPartial")]
        public async Task<ActionResult> CustomerPartialPost(int id)
        {
            var customer = await db.Customers.FindAsync(id);
            if (TryUpdateModel(customer))
            {
                try
                {
                    await db.SaveChangesAsync();
                    return RedirectToAction("Details", new { id=id});
                }
                catch(Exception ex)
                {
                    return PartialView(customer); 
                }
            }
            return null;
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,TargetGroupID,LastName,FirstName,BirthDate,Sex,Phone,Mobile,email,Facebook,City,Address,PostCode,Notes,CompanyID,CreatedOn,CustomerGUID")] Customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (customer.CustomerGUID == Guid.Empty) { customer.CustomerGUID = Guid.NewGuid(); }
                    customer.EditedOn = Classes.SharedClass.Now();
                    db.Entry(customer).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = customer.id });
                }
                int CompID = CompanyID();
                IQueryable<TargetGroup> validtargetgroups = db.TargetGroups.Where(r => r.CompanyID == CompID);
                ViewBag.TargetGroupID = new SelectList(validtargetgroups, "id", "Name", customer.TargetGroupID);
                return View(customer);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, string.Format("{0} - {1}", "CustomersController", "Edit (POST)"));
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // GET: Customers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                int CompID = CompanyID();
                Customer customer = await db.Customers.FindAsync(id);
                if (customer == null || customer.CompanyID != CompID)
                {
                    return HttpNotFound();
                }
                return View(customer);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, string.Format("{0} - {1}", "CustomersController", "Delate"));
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                Customer customer = await db.Customers.FindAsync(id);
                db.Customers.Remove(customer);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, string.Format("{0} - {1}", "CustomersController", "Delete (Confirmed)"));
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public JsonResult GetCustomerByid(int id)
        {
            var customer = db.Customers.Find(id);
            var data = new { email = customer.email, Mobile = customer.Mobile };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditDailyRecall(int? CustomerID)
        {
            if (CustomerID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            Customer customer = db.Customers.Find(CustomerID);
            if (customer == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            DailyRecall recall = db.DailyRecalls.SingleOrDefault(r => r.CustomerID == CustomerID);
            if (recall == null)
            {
                recall = new Models.DailyRecall { CustomerID = CustomerID.Value };
                db.DailyRecalls.Add(recall);
                db.SaveChanges();
            }
            return PartialView("DailyRecall", recall);
        }

        [HttpPost]
        [ActionName("EditDailyRecall")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditDailyRecallPost(int id)//[Bind(Include = "id,CustomerID,Breakfast,MorningSnack,Lunch,EveningSnack,Dinner,Milk,Yoghurt,WhiteCheese,YellowCheese,CottageCheese,Chicken,Turkey,Hamburger,Beef,Pork,InOilFood,Legumes,Cereals,Nuts,Alcohol,JunkFood,Salads,Fruits,LikeA,LikeB,LikeC,LikeD,LikeE,DislikeA,DislikeB,DislikeC,DislikeD,DislikeE,Notes")] DailyRecall dailyRecall)
        {

            var dailyRecall = await db.DailyRecalls.SingleOrDefaultAsync(r => r.CustomerID == id);
            if (TryUpdateModel(dailyRecall, "",null,new string[] { "id"}))
            {
                try
                {
                   await db.SaveChangesAsync();
                    return RedirectToAction("Details", "Customers", new { id = dailyRecall.CustomerID });
                }
                catch(Exception ex)
                {
                    return RedirectToAction("Details", new { id = id });
                }
            }

            return View(dailyRecall);
        }

        public async Task<ActionResult> EditBasicQuestionnaires(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BasicQuestionnaire basicQuestionnaire = await db.BasicQuestionnairies.FindAsync(id);
            if (basicQuestionnaire == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "id", "LastName", basicQuestionnaire.CustomerID);
            return View(basicQuestionnaire);
        }

        [HttpPost]
        [ActionName("EditBasicQuestionnaires")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditBasicQuestionnairesPost(int id)//[Bind(Include = "id,CustomerID,QuestionnareDate,JobName,JobHoursPerDay,DailyActivityDescription,CardioVascularProblems,HighBloodPressure,LowBloodPressure,Diabetes,Hypoglycemia,Asthma,BreathingProblems,Arthritis,HighCholesterol,HighTriglycerides,Allergies,Ulcer,MaxWeightEver,MaxWeightAge,MinWeightEver,MinWeightAge,WeightIncreasedOnLastPeriod,WeightDecreasedOnLastPeriod,OverWeightOnEarlyYears,DailyMeals,LackOfAppetite,Bulimia,HungryHours,BuyingFruitsFrequency,WeeklyConsumingFruits,WeeklyMealsOutOfHome,WeeklyConsumingSweetsByKind,DigestiveSystemFunctionality,FluidIntake,Notes")] BasicQuestionnaire basicQuestionnaire)
        {
            var basicQuestionnaire = await db.BasicQuestionnairies.SingleOrDefaultAsync(r => r.CustomerID == id);
            if (TryUpdateModel(basicQuestionnaire, "", null, new string[] { "id" }))
            {
                try
                {
                    await db.SaveChangesAsync();
                    return RedirectToAction("Details", "Customers", new { id = basicQuestionnaire.CustomerID });
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Details", new { id = id });
                }
            }
            return View(basicQuestionnaire);
        }

        public async Task<PartialViewResult> PaymentsPartial(int CustomerID)
        {
            var payments = db.Payments.Where(r => r.CustomerID == CustomerID);
            return PartialView(await payments.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Payments")]
        public ActionResult PaymentsPartialPost(int id,Payment[] payments)
        {

            return RedirectToAction("Details", new { id = id });
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
