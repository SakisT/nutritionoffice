using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using nutritionoffice.Models;
using PagedList;

namespace nutritionoffice.Controllers
{
    [Authorize(Roles = "Administrator,Owner")]
    public class RemindersController : MyBaseController
    {
        private ndbContext db = new ndbContext();

        // GET: Reminders
        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            try
            {
int pageSize = 20;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            int CompID = CompanyID();
            IQueryable<Reminder> reminders = db.Reminders.Include(c => c.Customer).OrderByDescending(r => new { r.OnDate }).Where(r=>r.Customer.CompanyID==CompID);
            if (!String.IsNullOrEmpty(searchString))
            {
                reminders = reminders.Where(r => r.Customer!=null && r.Customer.CompanyID==CompID && (r.Customer.LastName.Contains(searchString) | r.Customer.FirstName.Contains(searchString)));
            }

            int pageNumber = (page ?? 1);
            return View(reminders.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // GET: Reminders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
            int CompID = CompanyID();
            Reminder reminder = await db.Reminders.FindAsync(id);
            if (reminder == null || reminder.Customer.CompanyID!=CompID)
            {
                return HttpNotFound();
            }
            return View(reminder);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // GET: Reminders/Create
        public ActionResult Create(int CustomerID = 0,string Message="")
        {
            try
            {
            int CompID = CompanyID();
            IQueryable<Customer> customers = db.Customers.OrderBy(r => r.LastName).ThenBy(r => r.FirstName).Where(r=>r.CompanyID==CompID);
            ViewBag.CustomerID = new SelectList(customers.ToList(), "id", "FullName");
            Reminder reminder = new Reminder { MailState=Reminder.ReminderState.Active,
                SendSMS =false,
                SendEmail =true,
                OnDate =DateTime.Today.AddHours(11),
                SMSState =Reminder.ReminderState.Active,
                Message =Message };
                if (CustomerID != 0){
                    Customer Customer = db.Customers.Find(CustomerID);
                    reminder.CustomerID = CustomerID;
                    reminder.Customer = Customer;
                    reminder.email = Customer.email;
                    reminder.Mobile = Customer.Mobile;
                }
            return View(reminder);
            //return View();
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // POST: Reminders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,CustomerID,AppointmentID,OnDate,Mobile,SendSMS,email,SendEmail,Message,MailState,SMSState")] Reminder reminder)
        {
            try
            {
          if (ModelState.IsValid)
            {
                db.Reminders.Add(reminder);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AppointmentID = new SelectList(db.Appointments, "id", "Notes", reminder.AppointmentID);
            ViewBag.CustomerID = new SelectList(db.Customers, "id", "LastName", reminder.CustomerID);
            return View(reminder);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // GET: Reminders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
            int CompID = CompanyID();
            Reminder reminder = await db.Reminders.FindAsync(id);
            if (reminder == null || reminder.Customer.CompanyID!=CompID)
            {
                return HttpNotFound();
            }
            ViewBag.AppointmentID = new SelectList(db.Appointments, "id", "Notes", reminder.AppointmentID);
            IQueryable<Customer> customers = db.Customers.OrderBy(r => r.LastName).ThenBy(r => r.FirstName).Where(r=>r.CompanyID==CompID);
            ViewBag.CustomerID = new SelectList(customers.ToList(), "id", "LastName", reminder.CustomerID);
            return View(reminder);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // POST: Reminders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,CustomerID,AppointmentID,OnDate,Mobile,SendSMS,email,SendEmail,Message,MailState,SMSState")] Reminder reminder)
        {
            try
            {
         if (ModelState.IsValid)
            {
                db.Entry(reminder).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AppointmentID = new SelectList(db.Appointments, "id", "Notes", reminder.AppointmentID);
            ViewBag.CustomerID = new SelectList(db.Customers, "id", "LastName", reminder.CustomerID);
            return View(reminder);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // GET: Reminders/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
          int CompID = CompanyID();
            Reminder reminder = await db.Reminders.FindAsync(id);
            if (reminder == null || reminder.Customer.CompanyID!=CompID)
            {
                return HttpNotFound();
            }
            return View(reminder);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // POST: Reminders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
            Reminder reminder = await db.Reminders.FindAsync(id);
            db.Reminders.Remove(reminder);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
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
