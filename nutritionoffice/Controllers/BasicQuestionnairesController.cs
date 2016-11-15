using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using nutritionoffice.Models;

namespace nutritionoffice.Controllers
{
    public class BasicQuestionnairesController : MyBaseController
    {
        private ndbContext db = new ndbContext();

        public  ActionResult Index()
        {
                        return View();
        }

        //-------------------------------------- C u s t o m e r   A c t i o n s ---------------------------------------
        [AllowAnonymous]
        public async Task<ActionResult> CreateOrEditByCustomer(string CustomerGuid)
        {
            Guid g = Guid.Parse(CustomerGuid);
            Customer customer = await db.Customers.Where(r => r.CustomerGUID == g).FirstOrDefaultAsync();
            if (customer == null) { return RedirectToAction("Index"); }
            BasicQuestionnaire questionnaire = await db.BasicQuestionnairies.Where(r => r.CustomerID == customer.id).FirstOrDefaultAsync();
            if (questionnaire == null)
            {
                return RedirectToAction("CreateByCustomer", new { CustomerGuid = g });
            }
            return RedirectToAction("EditByCustomer", new { CustomerGuid = g });
        }

        [AllowAnonymous]
        public async Task<ActionResult> EditByCustomer(Guid CustomerGuid)
        {
            if (CustomerGuid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.Customers.FirstAsync(r => r.CustomerGUID == CustomerGuid);

            BasicQuestionnaire questionnaire = await db.BasicQuestionnairies.FirstAsync(r => r.CustomerID == customer.id);
            ViewBag.HideMenu = true;
            return View(questionnaire);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditByCustomer([Bind(Include = "id,CustomerID,QuestionnareDate,JobName,JobHoursPerDay,DailyActivityDescription,CardioVascularProblems,HighBloodPressure,LowBloodPressure,Diabetes,Hypoglycemia,Asthma,BreathingProblems,Arthritis,HighCholesterol,HighTriglycerides,Allergies,Ulcer,MaxWeightEver,MaxWeightAge,MinWeightEver,MinWeightAge,WeightIncreasedOnLastPeriod,WeightDecreasedOnLastPeriod,OverWeightOnEarlyYears,DailyMeals,LackOfAppetite,Bulimia,HungryHours,BuyingFruitsFrequency,WeeklyConsumingFruits,WeeklyMealsOutOfHome,WeeklyConsumingSweetsByKind,DigestiveSystemFunctionality,FluidIntake,Notes")] BasicQuestionnaire basicQuestionnaire)
        {
            if (ModelState.IsValid)
            {
                db.Entry(basicQuestionnaire).State = EntityState.Modified;
                await db.SaveChangesAsync();
                db.Entry(basicQuestionnaire).Reference(r => r.Customer).Load();
                return RedirectToAction("SuccessfulRegistration", "DailyRecalls", new { id = basicQuestionnaire.Customer.CompanyID });
            }
            return View(basicQuestionnaire);
        }

        public async Task<ActionResult> CreateOrEdit(int id)
        {
            BasicQuestionnaire questionnare = await db.BasicQuestionnairies.Where(r => r.CustomerID == id).FirstOrDefaultAsync();
            if (questionnare == null)
            {
                return RedirectToAction("Create", new { id = id });
            }
            return RedirectToAction("Edit", new { id = questionnare.id });
        }

        public async Task<ActionResult> Details(int? id)
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
            return View(basicQuestionnaire);
        }

        public async Task<ActionResult> Create(int id)
        {
            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null) { return new HttpNotFoundResult(); }
            BasicQuestionnaire questionnary = new BasicQuestionnaire
            {
                QuestionnareDate=DateTime.Today,
                Customer = customer,
                CustomerID = id
            };
            return View(questionnary);
        }

        // POST: BasicQuestionnaires/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,CustomerID,QuestionnareDate,JobName,JobHoursPerDay,DailyActivityDescription,CardioVascularProblems,HighBloodPressure,LowBloodPressure,Diabetes,Hypoglycemia,Asthma,BreathingProblems,Arthritis,HighCholesterol,HighTriglycerides,Allergies,Ulcer,MaxWeightEver,MaxWeightAge,MinWeightEver,MinWeightAge,WeightIncreasedOnLastPeriod,WeightDecreasedOnLastPeriod,OverWeightOnEarlyYears,DailyMeals,LackOfAppetite,Bulimia,HungryHours,BuyingFruitsFrequency,WeeklyConsumingFruits,WeeklyMealsOutOfHome,WeeklyConsumingSweetsByKind,DigestiveSystemFunctionality,FluidIntake,Notes")] BasicQuestionnaire basicQuestionnaire)
        {
            if (ModelState.IsValid)
            {
                db.BasicQuestionnairies.Add(basicQuestionnaire);
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Customers", new { id = basicQuestionnaire.CustomerID });
            }
            return View(basicQuestionnaire);
        }

        public async Task<ActionResult> Edit(int? id)
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

        // POST: BasicQuestionnaires/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,CustomerID,QuestionnareDate,JobName,JobHoursPerDay,DailyActivityDescription,CardioVascularProblems,HighBloodPressure,LowBloodPressure,Diabetes,Hypoglycemia,Asthma,BreathingProblems,Arthritis,HighCholesterol,HighTriglycerides,Allergies,Ulcer,MaxWeightEver,MaxWeightAge,MinWeightEver,MinWeightAge,WeightIncreasedOnLastPeriod,WeightDecreasedOnLastPeriod,OverWeightOnEarlyYears,DailyMeals,LackOfAppetite,Bulimia,HungryHours,BuyingFruitsFrequency,WeeklyConsumingFruits,WeeklyMealsOutOfHome,WeeklyConsumingSweetsByKind,DigestiveSystemFunctionality,FluidIntake,Notes")] BasicQuestionnaire basicQuestionnaire)
        {
            if (ModelState.IsValid)
            {
                db.Entry(basicQuestionnaire).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Customers", new { id = basicQuestionnaire.CustomerID });
            }
            return View(basicQuestionnaire);
        }

        public async Task<ActionResult> Delete(int? id)
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
            return View(basicQuestionnaire);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            BasicQuestionnaire basicQuestionnaire = await db.BasicQuestionnairies.FindAsync(id);
            db.BasicQuestionnairies.Remove(basicQuestionnaire);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<JsonResult> SendBasicQuestsByEmail(int id, string MessageText)
        {
            int companyid = CompanyID();

            Company company = await db.Companies.FindAsync(companyid);
            Customer customer = await db.Customers.FindAsync(id);
            try
            {
                if (customer.CustomerGUID == Guid.Empty)
                {
                    customer.CustomerGUID = Guid.NewGuid();
                    db.Entry(customer).State = EntityState.Modified;
                    db.SaveChanges();
                    throw new Exception("Μη αποδεκτό Customer Guid.  Κάντε Refresh τη σελίδα και προσπαθήστε ξανά.");
                }
                if (string.IsNullOrEmpty(customer.email)) { throw new Exception("Μη αποδεκτό email"); }

                nMessage message = new nMessage();

                message.Type = nMessage.DeliveryType.email;
                message.Status = nMessage.MessageStatus.Pending;
                message.Subject = Resource.BasicQuestionnaire;
                message.MessageBody = MessageText;
                message.Recipient = customer.email;
                message.CompanyID = companyid;
                db.Messages.Add(message);
                await db.SaveChangesAsync();

                bool sent = await Classes.Communicator.SendMailAsync(new Classes.Communicator.SmtpMailClient
                {
                    Credentials = new NetworkCredential { UserName = company.SMTPUserName, Password = company.SMTPPassword },
                    EnableSSL = company.SMTPEnableSSL,
                    Host = company.SMTPHost,
                    Port = company.SMTPPort
                }, message);

                return Json(new { Result = (sent) ? "Success" : "Failed" }, behavior: JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = ex.Message }, behavior: JsonRequestBehavior.AllowGet);
            }

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
