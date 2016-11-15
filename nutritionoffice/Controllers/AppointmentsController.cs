using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using nutritionoffice.Models;
using nutritionoffice.ViewModels;
using System.Collections.Generic;

namespace nutritionoffice.Controllers
{
    [Authorize(Roles = "Administrator,Owner")]
    public class AppointmentsController : MyBaseController
    {
        private ndbContext db = new ndbContext();

        // GET: Appointments
        public ActionResult Index(DateTime? date)
        {
            try
            {
                int CompID = CompanyID();
                //Αν έρθει χωρίς ημερομηνία, βάζουμε την σημερινή.
                if (!date.HasValue) { date = DateTime.Today; }

                //Αν η επιλεγμένη ημερομηνία είναι Κυριακή, παίρνουμε την αμέσως επόμενη Δευτέρα.
                if (date.Value.DayOfWeek == DayOfWeek.Sunday)
                {
                    date = date.Value.AddDays(1);
                }
                else
                {
                    date = date.Value.AddDays(1 - (int)date.Value.DayOfWeek);
                }
                ViewBag.date = date;
                DateTime todate = date.Value.AddDays(6);

                IEnumerable<Appointment> appointments = db.Appointments.Include(a => a.Customer).Where(r => r.Customer.CompanyID == CompID).OrderByDescending(r => r.Date).ThenBy(r => r.FromTime).Where(r => r.Date >= date && r.Date <= todate);

                int totaldays = (int)(todate - date.Value).TotalDays;

                IEnumerable<DateTime> daterange = from int p in Enumerable.Range(0, totaldays) select date.Value.AddDays(p);

                int appointmentdurationinminutes = 40;
                foreach (DateTime d in daterange)
                {
                    var currentdatestart = new DateTime(d.Year, d.Month, d.Day, 08, 30, 0);
                    var currentdateend = new DateTime(d.Year, d.Month, d.Day, 21, 00, 0);
                    int totaldateminutes = (int)(currentdateend - currentdatestart).TotalMinutes;
                    for (int Index = 0; Index < totaldateminutes; Index += appointmentdurationinminutes)
                    {
                        var refdate = currentdatestart.AddMinutes(Index);
                        var existingappointment = appointments.Where(r => r.FromTime < refdate && r.ToTime > refdate).FirstOrDefault();
                        if (existingappointment == null)
                        {
                            appointments = appointments.Concat(new Appointment[] { new Appointment { CustomerID = 0, Date = refdate, FromTime = refdate, ToTime = refdate.AddMinutes(appointmentdurationinminutes), Notes = "" } }.AsEnumerable());
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }
                //ViewBag.CultureInfo= System.Globalization.CultureInfo.CurrentCulture.Clone();
                return View(appointments.ToList());
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, string.Format("{0} - {1}", "AppointmentsController", "Index"));
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // GET: Appointments/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                int CompID = CompanyID();
                Appointment appointment = await db.Appointments.FindAsync(id);
                if (appointment == null || appointment.Customer.CompanyID != CompID)
                {
                    return HttpNotFound();
                }
                return View(appointment);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, string.Format("{0} - {1}", "AppointmentsController", "Details"));
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // GET: Appointments/Create
        public ActionResult Create(int? CustomerID, DateTime? datetime)
        {
            try
            {
                int CompID = CompanyID();
                if (!datetime.HasValue) { datetime = DateTime.Today; }
                if (datetime.Value.DayOfWeek == DayOfWeek.Sunday) { datetime = datetime.Value.AddDays(1); }
                IQueryable<Customer> customerslist = db.Customers.OrderBy(r => r.LastName).ThenBy(r => r.FirstName).Where(r => r.CompanyID == CompID);
                ViewBag.CustomerID = new SelectList(customerslist, "id", "FullName", CustomerID);
                DateTime d = datetime.Value.Date;

                Appointment appointment = new Appointment
                {
                    Date = datetime.Value,
                    FromTime = new DateTime(d.Year, d.Month, d.Day, 16, 0, 0),
                    ToTime = new DateTime(d.Year, d.Month, d.Day, 16, 40, 0),
                    State = Appointment.AppointmentState.Active
                };

                ViewBag.SelectCustomer = true;
                ViewBag.CustomerName = "";
                Customer customer = null;
                if (CustomerID.HasValue)
                {
                    appointment.CustomerID = CustomerID.Value;
                    customer = db.Customers.Find(CustomerID);
                    ViewBag.CustomerName = string.Format("{0} {1}", customer.LastName, customer.FirstName);
                    ViewBag.SelectCustomer = false;
                }

                DateTime reminderdate = datetime.Value.AddDays(-2).Date;

                Reminder reminder = new Reminder()
                {
                    CustomerID = appointment.CustomerID,
                    OnDate = new DateTime(reminderdate.Year,
                    reminderdate.Month, reminderdate.Day, 11, 30, 0),
                    email = (customer == null) ? "" : customer.email,
                    Mobile = (customer == null) ? "" : customer.Mobile,
                    Message = "Σας υπενθυμίζουμε το ραντεβού μας για.....",
                    //SMSState = Reminder.ReminderState.Active,
                    MailState = Reminder.ReminderState.Active,
                    SendEmail = true,
                    SendSMS = false
                };

                return View(new appointmentreminder() { appointment = appointment, reminder = reminder });
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, string.Format("{0} - {1}", "AppointmentsController", "Create"));
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(appointmentreminder appointmentreminder)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        DateTime d = appointmentreminder.appointment.Date;
                        appointmentreminder.appointment.FromTime = new DateTime(d.Year, d.Month, d.Day, appointmentreminder.appointment.FromTime.Hour, appointmentreminder.appointment.FromTime.Minute, 0);
                        appointmentreminder.appointment.ToTime = new DateTime(d.Year, d.Month, d.Day, appointmentreminder.appointment.ToTime.Hour, appointmentreminder.appointment.ToTime.Minute, 0);
                        if ((appointmentreminder.appointment.FromTime - appointmentreminder.appointment.ToTime).TotalMinutes > 1)
                        {
                            ModelState.AddModelError(string.Empty, "Η ώρα τέλους του ραντεβού πρέπει να είναι μεταγενέστερη της ώρας έναρξης του!");
                            throw new Exception("Η ώρα αρχής δεν μπορεί να είναι μεταγενέστερη της ώρας τέλους");
                        }
                        db.Appointments.Add(appointmentreminder.appointment);
                        appointmentreminder.reminder.Appointment = appointmentreminder.appointment;
                        appointmentreminder.reminder.CustomerID = appointmentreminder.appointment.CustomerID;
                        appointmentreminder.reminder.SMSState = Reminder.ReminderState.Active;
                        db.Reminders.Add(appointmentreminder.reminder);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index", new { date = d });
                    }
                    catch (Exception ex)
                    {
                        Classes.ErrorHandler.LogException(ex, string.Format("{0} - {1}", "AppointmentsController", "Create (POST)"));
                        ViewBag.CustomerID = new SelectList(db.Customers, "id", "LastName", appointmentreminder.appointment.CustomerID);
                        return View(appointmentreminder);
                    }
                }

                ViewBag.CustomerID = new SelectList(db.Customers, "id", "LastName", appointmentreminder.appointment.CustomerID);
                return View(appointmentreminder);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, string.Format("{0} - {1}", "AppointmentsController", "Create (POST)"));
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // GET: Appointments/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                int CompID = CompanyID();
                Appointment appointment = await db.Appointments.FindAsync(id);
                if (appointment == null || appointment.Customer.CompanyID != CompID)
                {
                    return HttpNotFound();
                }
                Reminder reminder = await db.Reminders.Where(r => r.AppointmentID == appointment.id).FirstOrDefaultAsync();

                Customer customer = await db.Customers.FindAsync(appointment.CustomerID);

                ViewBag.CustomerName = string.Format($"{customer.LastName} {customer.FirstName}");

                appointmentreminder appreminder = new appointmentreminder { appointment = appointment, reminder = reminder };
                return View(appreminder);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, string.Format("{0} - {1}", "AppointmentsController", "Edit"));
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(appointmentreminder appointmentreminder)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DateTime d = appointmentreminder.appointment.Date;
                    try
                    {
                        appointmentreminder.appointment.FromTime = new DateTime(d.Year, d.Month, d.Day, appointmentreminder.appointment.FromTime.Hour, appointmentreminder.appointment.FromTime.Minute, 0);
                        appointmentreminder.appointment.ToTime = new DateTime(d.Year, d.Month, d.Day, appointmentreminder.appointment.ToTime.Hour, appointmentreminder.appointment.ToTime.Minute, 0);
                        if ((appointmentreminder.appointment.FromTime - appointmentreminder.appointment.ToTime).TotalMinutes > 1)
                        {
                            ModelState.AddModelError(string.Empty, "Η ώρα τέλους του ραντεβού πρέπει να είναι μεταγενέστερη της ώρας έναρξης του!");
                            throw new Exception("Η ώρα αρχής δεν μπορεί να είναι μεταγενέστερη της ώρας τέλους");
                        }
                    }
                    catch (Exception ex)
                    {
                        return View(appointmentreminder);
                    }
                    db.Entry(appointmentreminder.appointment).State = EntityState.Modified;
                    db.Entry(appointmentreminder.reminder).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", new { date = d });// RedirectToAction("Details", "Customers", new { id = appointment.CustomerID });
                }

                return View(appointmentreminder);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, string.Format("{0} - {1}", "AppointmentsController", "Edit (POST)"));
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // GET: Appointments/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Appointment appointment = await db.Appointments.FindAsync(id);
                if (appointment == null)
                {
                    return HttpNotFound();
                }
                return View(appointment);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, string.Format("{0} - {1}", "AppointmentsController", "Delete"));
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                Appointment appointment = await db.Appointments.FindAsync(id);
                Reminder reminder = await db.Reminders.Where(r => r.AppointmentID.HasValue && r.AppointmentID.Value == id).FirstOrDefaultAsync();
                if (reminder != null) { db.Reminders.Remove(reminder); }
                db.Appointments.Remove(appointment);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, string.Format("{0} - {1}", "AppointmentsController", "Delete (Confirmed)"));
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        public ActionResult CancelAppointments(DateTime? fromdate, DateTime? todate, IEnumerable<appointmentcancelation> appointments)
        {
            try
            {
                ViewBag.FromDate = null;
                ViewBag.ToDate = null;
                var MailMessage = Request["mailmessage"];
                var SMSMessage = Request["smsmessage"];
                if (fromdate.HasValue && todate.HasValue)
                {
                    ViewBag.FromDate = fromdate.Value.ToShortDateString();
                    ViewBag.ToDate = todate.Value.ToShortDateString();
                    if (Request["refreshdates"] != null)
                    {
                        try
                        {
                            if (todate.Value < fromdate.Value) { throw new Exception("'From Date' Should be greater than today and previous than 'To Date'"); }

                            IQueryable<Appointment> PeriodAppointments = db.Appointments.Include(r => r.Customer).Where(r => r.Date <= todate.Value && r.Date >= fromdate.Value && r.State == Appointment.AppointmentState.Active);
                            appointments = (from Appointment p in PeriodAppointments select new appointmentcancelation { appointment = p, Cancel = true, SendSMS = false, SendEmail = true }).AsEnumerable();
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("CustomError", ex.Message);
                            return View(appointments);
                        }
                    }
                    else
                    {
                        try
                        {
                            if ((MailMessage == null || string.IsNullOrEmpty(MailMessage)) && (SMSMessage == null || string.IsNullOrEmpty(SMSMessage))) { throw new Exception("Type at list one message (mail or SMS)"); }
                            foreach (var item in appointments.Where(r => r.Cancel))
                            {
                                //Βρίσκουμε το Existing Appointment
                                Appointment existingappointment = db.Appointments.Find(item.appointment.id);
                                Customer customer = db.Customers.Find(item.appointment.CustomerID);

                                //Δημιουργούμε τα νέα Reminders
                                //Αν υπάρχει το existingappointment
                                if (existingappointment != null)
                                {
                                    //Βρίσκουμε και το αντίστοιχο Reminder
                                    Reminder existingreminder = db.Reminders.Where(r => r.AppointmentID == item.appointment.id).FirstOrDefault();

                                    if (item.SendEmail && MailMessage != null)
                                    {
                                        Reminder NewEmailReminder = new Reminder
                                        {
                                            CustomerID = existingappointment.CustomerID,
                                            MailState = Reminder.ReminderState.Active,
                                            OnDate =Classes.SharedClass.Now().AddHours(2),
                                            SendEmail = true,
                                            SendSMS = false,
                                            SMSState = Reminder.ReminderState.Active,
                                            Message = MailMessage,
                                            email = (existingreminder == null) ? customer.email : existingreminder.email
                                        };
                                        if (!string.IsNullOrEmpty(NewEmailReminder.email)) { db.Reminders.Add(NewEmailReminder); }
                                    }

                                    if (item.SendSMS && SMSMessage != null)
                                    {
                                        Reminder NewSMSReminder = new Reminder
                                        {
                                            CustomerID = existingappointment.CustomerID,
                                            MailState = Reminder.ReminderState.Active,
                                            OnDate = Classes.SharedClass.Now().AddHours(2),
                                            SendEmail = false,
                                            SendSMS = true,
                                            SMSState = Reminder.ReminderState.Active,
                                            Message = SMSMessage,
                                            Mobile = (existingreminder == null) ? customer.Mobile : existingreminder.Mobile
                                        };
                                        if (!string.IsNullOrEmpty(NewSMSReminder.Mobile)) { db.Reminders.Add(NewSMSReminder); }
                                    }

                                    if (existingreminder != null)
                                    {
                                        db.Reminders.Remove(existingreminder);
                                        db.Appointments.Remove(existingappointment);
                                    }
                                }

                            }
                            db.SaveChanges();
                            //Έξοδος στην κεντρική
                            return RedirectToAction("Index", "Home");
                        }
                        catch (Exception ex)
                        {
                            if (fromdate.HasValue && todate.HasValue)
                            {
                                IQueryable<Appointment> PeriodAppointments = db.Appointments.Include(r => r.Customer).Where(r => r.Date <= todate.Value && r.Date >= fromdate.Value && r.State == Appointment.AppointmentState.Active);
                                appointments = (from Appointment p in PeriodAppointments select new appointmentcancelation { appointment = p, Cancel = true, SendSMS = false, SendEmail = true }).AsEnumerable();
                            }
                            Classes.ErrorHandler.LogException(ex, string.Format("{0} - {1}", "AppointmentsController", "CancelAppointments"));
                            ModelState.AddModelError("CustomError", ex.Message);
                            return View(appointments);
                        }

                    }

                }
                else
                {
                    appointments = new List<appointmentcancelation>();
                }
                return View(appointments.ToList());
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, string.Format("{0} - {1}", "AppointmentsController", "CancelAppointments"));
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
