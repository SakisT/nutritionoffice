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
            ViewBag.StartDate = DateTime.Today.ToString("yyyy-MM-dd");
            if (date.HasValue) { ViewBag.StartDate = date.Value.ToString("yyyy-MM-dd"); }
            return View();
        }

        //try
        //{
        //    int CompID = CompanyID();
        //    //Αν έρθει χωρίς ημερομηνία, βάζουμε την σημερινή.
        //    if (!date.HasValue) { date = DateTime.Today; }

        //    //Αν η επιλεγμένη ημερομηνία είναι Κυριακή, παίρνουμε την αμέσως επόμενη Δευτέρα.
        //    if (date.Value.DayOfWeek == DayOfWeek.Sunday)
        //    {
        //        date = date.Value.AddDays(1);
        //    }
        //    else
        //    {
        //        date = date.Value.AddDays(1 - (int)date.Value.DayOfWeek);
        //    }
        //    ViewBag.date = date;
        //    DateTime todate = date.Value.AddDays(6);

        //    IEnumerable<Appointment> appointments = db.Appointments.Include(a => a.Customer).Where(r => r.Customer.CompanyID == CompID).OrderByDescending(r => r.Date).ThenBy(r => r.FromTime).Where(r => r.Date >= date && r.Date <= todate);

        //    int totaldays = (int)(todate - date.Value).TotalDays;

        //    IEnumerable<DateTime> daterange = from int p in Enumerable.Range(0, totaldays) select date.Value.AddDays(p);

        //    int appointmentdurationinminutes = 40;
        //    foreach (DateTime d in daterange)
        //    {
        //        var currentdatestart = new DateTime(d.Year, d.Month, d.Day, 08, 30, 0);
        //        var currentdateend = new DateTime(d.Year, d.Month, d.Day, 21, 00, 0);
        //        int totaldateminutes = (int)(currentdateend - currentdatestart).TotalMinutes;
        //        for (int Index = 0; Index < totaldateminutes; Index += appointmentdurationinminutes)
        //        {
        //            var refdate = currentdatestart.AddMinutes(Index);
        //            var existingappointment = appointments.Where(r => r.FromTime < refdate && r.ToTime > refdate).FirstOrDefault();
        //            if (existingappointment == null)
        //            {
        //                appointments = appointments.Concat(new Appointment[] { new Appointment { CustomerID = 0, Date = refdate, FromTime = refdate, ToTime = refdate.AddMinutes(appointmentdurationinminutes), Notes = "" } }.AsEnumerable());
        //            }
        //            Console.WriteLine();
        //        }
        //        Console.WriteLine();
        //    }
        //    //ViewBag.CultureInfo= System.Globalization.CultureInfo.CurrentCulture.Clone();
        //    return View(appointments.ToList());
        //}
        //catch (Exception ex)
        //{
        //    Classes.ErrorHandler.LogException(ex, string.Format("{0} - {1}", "AppointmentsController", "Index"));
        //    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
        //}

        [HttpPost]
        public async Task<ActionResult> UpdateAppointment(int id, int years, int months, int days, int starthours, int startminutes, int endhours, int endminutes)
        {
            Appointment appointment = await db.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return new HttpNotFoundResult();
            }

            var newdate = new DateTime(appointment.Date.Year,
                appointment.Date.Month, appointment.Date.Day,
                appointment.FromTime_Hour.GetValueOrDefault(16),
                appointment.FromTime_Minutes.GetValueOrDefault(0), 0)
                .AddYears(years).AddMonths(months).AddDays(days).AddHours(starthours).AddMinutes(startminutes);

            appointment.Date = newdate.Date;
            DateTime fromdt = newdate;
            DateTime todt = new DateTime(newdate.Year,
                newdate.Month,
                newdate.Day,
                appointment.ToTime_Hour.GetValueOrDefault(16),
                appointment.ToTime_Minutes.GetValueOrDefault(0),
                0);
            if (Math.Abs(years)+Math.Abs(months)+Math.Abs(days)+ Math.Abs(starthours) + Math.Abs(startminutes) > 0)
            {
                todt = todt.Add(new TimeSpan(starthours, startminutes, 0));
            }
            else
            {
                todt = todt.Add(new TimeSpan(endhours, endminutes, 0));
            }
            //    .Add(new TimeSpan(endhours, endminutes, 0));
            appointment.FromTime_Hour = fromdt.Hour;
            appointment.FromTime_Minutes = fromdt.Minute;
            appointment.ToTime_Hour = todt.Hour;
            appointment.ToTime_Minutes = todt.Minute;

            Reminder reminder = await db.Reminders.Where(r => r.AppointmentID == appointment.id).FirstOrDefaultAsync();
            reminder.Message = string.Format($"Σας υπενθυμίζουμε το ραντεβού μας για {appointment.Date:d/M/yyyy} {appointment.FromTime_Hour}:{appointment.FromTime_Minutes:00}");
            db.Entry(reminder).State = EntityState.Modified;

            db.Entry(appointment).State = EntityState.Modified;

            await db.SaveChangesAsync();

            return Json(new { result = "Success" }, behavior: JsonRequestBehavior.AllowGet);
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

        [HttpGet]
        public async Task<JsonResult> GetAppointmentsByDate(string datetext, DateTime? date)
        {
            try
            {
                int CompID = CompanyID();
                //Αν έρθει χωρίς ημερομηνία, βάζουμε την σημερινή.
                if (!date.HasValue) { date = DateTime.Today; }

                DateTime fromdate = new DateTime(date.Value.Year, date.Value.Month, 1);

                DateTime todate = fromdate.AddMonths(1).AddDays(-1);

                List<Appointment> appointments = await db.Appointments.Include(a => a.Customer).Where(r => r.Customer.CompanyID == CompID).OrderByDescending(r => r.Date).ThenBy(r => r.FromTime_Hour).ThenBy(r => r.FromTime_Minutes).Where(r => r.Date >= fromdate && r.Date <= todate).ToListAsync();

                //var returnvalue = (from p in appointments select new {title=p.Customer?.FullName??"", start=p.Date.ToString("O") });
                var returnvalue = (from p in appointments
                                   let startdt = new DateTime(p.Date.Year, p.Date.Month, p.Date.Day, p.FromTime_Hour.GetValueOrDefault(0), p.FromTime_Minutes.GetValueOrDefault(0), 0)
                                   let enddt = new DateTime(p.Date.Year, p.Date.Month, p.Date.Day, p.ToTime_Hour.GetValueOrDefault(0), p.ToTime_Minutes.GetValueOrDefault(0), 0)
                                   select new
                                   {
                                       id = p.id,
                                       title = p.Customer?.FullName ?? "",
                                       start = startdt.ToString("O"),
                                       end = enddt.ToString("O"),
                                       color = p.Color
                                   });
                //,color = "red"
                // Event default color #6BA5C2
                return Json(returnvalue, behavior: JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, string.Format("{0} - {1}", "AppointmentsController", "Index"));
                return Json(new { error = ex.Message }, behavior: JsonRequestBehavior.AllowGet);
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
                    FromTime_Hour = 16,
                    FromTime_Minutes = 0,
                    ToTime_Hour = 16,
                    ToTime_Minutes = 40,
                    State = Appointment.AppointmentState.Active,
                    Color = "#6ba5c2"
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
                    Message = string.Format($"Σας υπενθυμίζουμε το ραντεβού μας για {new DateTime(appointment.Date.Year, appointment.Date.Month, appointment.Date.Day, 16, 00, 0):d/M/yyyy HH:mm}"),
                    //SMSState = Reminder.ReminderState.Active,
                    MailState = Reminder.ReminderState.Active,
                    SendEmail = true,
                    SendSMS = false
                };

                ViewBag.FromHours = new SelectList(Enumerable.Range(0, 24).Select(r => r.ToString("00")).AsEnumerable(), "16");
                ViewBag.FromMinutes = new SelectList(Enumerable.Range(0, 59).Where(r => (r % 5) == 0).Select(r => r.ToString("00")).AsEnumerable(), "00");

                ViewBag.ToHours = new SelectList(Enumerable.Range(0, 24).Select(r => r.ToString("00")).AsEnumerable(), "16");
                ViewBag.ToMinutes = new SelectList(Enumerable.Range(0, 59).Where(r => (r % 5) == 0).Select(r => r.ToString("00")).AsEnumerable(), "20");

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
                        appointmentreminder.appointment.FromTime_Hour = appointmentreminder.appointment.FromTime_Hour.GetValueOrDefault(0);
                        appointmentreminder.appointment.FromTime_Minutes = appointmentreminder.appointment.FromTime_Minutes.GetValueOrDefault(0);
                        appointmentreminder.appointment.ToTime_Hour = appointmentreminder.appointment.ToTime_Hour.GetValueOrDefault(0);
                        appointmentreminder.appointment.ToTime_Minutes = appointmentreminder.appointment.ToTime_Minutes.GetValueOrDefault(0);

                        var fromdt = new DateTime(d.Year, d.Month, d.Day, appointmentreminder.appointment.FromTime_Hour.GetValueOrDefault(0), appointmentreminder.appointment.FromTime_Minutes.GetValueOrDefault(0), 0);
                        var todt = new DateTime(d.Year, d.Month, d.Day, appointmentreminder.appointment.ToTime_Hour.GetValueOrDefault(0), appointmentreminder.appointment.ToTime_Minutes.GetValueOrDefault(0), 0);
                        if ((fromdt - todt).TotalMinutes > 1)
                        {
                            ModelState.AddModelError(string.Empty, "Η ώρα τέλους του ραντεβού πρέπει να είναι μεταγενέστερη της ώρας έναρξης του!");
                            throw new Exception("Η ώρα αρχής δεν μπορεί να είναι μεταγενέστερη της ώρας τέλους");
                        }
                        appointmentreminder.appointment.FromTime = new DateTime(d.Year, d.Month, d.Day, appointmentreminder.appointment.FromTime_Hour.Value, appointmentreminder.appointment.FromTime_Minutes.Value, 0);
                        appointmentreminder.appointment.ToTime = new DateTime(d.Year, d.Month, d.Day, appointmentreminder.appointment.ToTime_Hour.Value, appointmentreminder.appointment.ToTime_Minutes.Value, 0);
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

                        ViewBag.FromHours = new SelectList(Enumerable.Range(0, 24).Select(r => r.ToString("00")).AsEnumerable(), appointmentreminder.appointment.FromTime_Hour.GetValueOrDefault(0).ToString("00"));
                        ViewBag.FromMinutes = new SelectList(Enumerable.Range(0, 59).Where(r => (r % 5) == 0).Select(r => r.ToString("00")).AsEnumerable(), appointmentreminder.appointment.FromTime_Minutes.GetValueOrDefault(0).ToString("00"));

                        ViewBag.ToHours = new SelectList(Enumerable.Range(0, 24).Select(r => r.ToString("00")).AsEnumerable(), appointmentreminder.appointment.ToTime_Hour.GetValueOrDefault(0).ToString("00"));
                        ViewBag.ToMinutes = new SelectList(Enumerable.Range(0, 59).Where(r => (r % 5) == 0).Select(r => r.ToString("00")).AsEnumerable(), appointmentreminder.appointment.ToTime_Minutes.GetValueOrDefault(0).ToString("00"));

                        return View(appointmentreminder);
                    }
                }

                ViewBag.CustomerID = new SelectList(db.Customers, "id", "LastName", appointmentreminder.appointment.CustomerID);

                var Hours = (from p in Enumerable.Range(0, 24) select new { Value = p, Text = p.ToString("00") });
                var Minutes = (from p in Enumerable.Range(0, 59).Where(r => (r % 5) == 0) select new { Value = p, Text = p.ToString("00") });


                ViewBag.FromHours = new SelectList(Hours, dataValueField: "Value", dataTextField: "Text", selectedValue: appointmentreminder.appointment.FromTime_Hour.GetValueOrDefault(0));
                ViewBag.FromMinutes = new SelectList(Minutes, dataValueField: "Value", dataTextField: "Text", selectedValue: appointmentreminder.appointment.FromTime_Minutes.GetValueOrDefault(0));

                ViewBag.ToHours = new SelectList(Hours, dataValueField: "Value", dataTextField: "Text", selectedValue: appointmentreminder.appointment.ToTime_Hour.GetValueOrDefault(0));
                ViewBag.ToMinutes = new SelectList(Minutes, dataValueField: "Value", dataTextField: "Text", selectedValue: appointmentreminder.appointment.ToTime_Minutes.GetValueOrDefault(0));
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

                var Hours = (from p in Enumerable.Range(0, 24) select new {Value=p, Text=p.ToString("00") });
                var Minutes = (from p in Enumerable.Range(0, 59).Where(r => (r % 5) == 0) select new { Value = p, Text = p.ToString("00") });

                ViewBag.FromHours = new SelectList(Hours, dataValueField:"Value", dataTextField:"Text", selectedValue: appointment.FromTime_Hour.GetValueOrDefault(16));
                ViewBag.FromMinutes = new SelectList(Minutes,dataValueField:"Value",dataTextField: "Text", selectedValue: appointment.FromTime_Minutes.GetValueOrDefault(0));

                ViewBag.ToHours = new SelectList(Hours, dataValueField: "Value", dataTextField: "Text", selectedValue: appointment.ToTime_Hour.GetValueOrDefault(16));
                ViewBag.ToMinutes = new SelectList(Minutes, dataValueField: "Value", dataTextField: "Text", selectedValue: appointment.ToTime_Minutes.GetValueOrDefault(20));

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
                        appointmentreminder.appointment.FromTime_Hour = appointmentreminder.appointment.FromTime_Hour.GetValueOrDefault(0);
                        appointmentreminder.appointment.FromTime_Minutes = appointmentreminder.appointment.FromTime_Minutes.GetValueOrDefault(0);
                        appointmentreminder.appointment.ToTime_Hour = appointmentreminder.appointment.ToTime_Hour.GetValueOrDefault(0);
                        appointmentreminder.appointment.ToTime_Minutes = appointmentreminder.appointment.ToTime_Minutes.GetValueOrDefault(0);
                        var fromdt = new DateTime(d.Year, d.Month, d.Day, appointmentreminder.appointment.FromTime_Hour.GetValueOrDefault(0), appointmentreminder.appointment.FromTime_Minutes.GetValueOrDefault(0), 0);
                        var todt = new DateTime(d.Year, d.Month, d.Day, appointmentreminder.appointment.ToTime_Hour.GetValueOrDefault(0), appointmentreminder.appointment.ToTime_Minutes.GetValueOrDefault(0), 0);
                        if ((fromdt - todt).TotalMinutes > 1)
                        {
                            ModelState.AddModelError(string.Empty, "Η ώρα τέλους του ραντεβού πρέπει να είναι μεταγενέστερη της ώρας έναρξης του!");
                            throw new Exception("Η ώρα αρχής δεν μπορεί να είναι μεταγενέστερη της ώρας τέλους");
                        }
                        appointmentreminder.appointment.FromTime = fromdt;
                        appointmentreminder.appointment.ToTime = todt;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.CustomerID = new SelectList(db.Customers, "id", "LastName", appointmentreminder.appointment.CustomerID);

                        ViewBag.FromHours = new SelectList(Enumerable.Range(0, 24).Select(r => r.ToString("00")).AsEnumerable(), appointmentreminder.appointment.FromTime_Hour.GetValueOrDefault(0).ToString("00"));
                        ViewBag.FromMinutes = new SelectList(Enumerable.Range(0, 59).Where(r => (r % 5) == 0).Select(r => r.ToString("00")).AsEnumerable(), appointmentreminder.appointment.FromTime_Minutes.GetValueOrDefault(0).ToString("00"));

                        ViewBag.ToHours = new SelectList(Enumerable.Range(0, 24).Select(r => r.ToString("00")).AsEnumerable(), appointmentreminder.appointment.ToTime_Hour.GetValueOrDefault(0).ToString("00"));
                        ViewBag.ToMinutes = new SelectList(Enumerable.Range(0, 59).Where(r => (r % 5) == 0).Select(r => r.ToString("00")).AsEnumerable(), appointmentreminder.appointment.ToTime_Minutes.GetValueOrDefault(0).ToString("00"));
                        return View(appointmentreminder);
                    }
                    db.Entry(appointmentreminder.appointment).State = EntityState.Modified;
                    db.Entry(appointmentreminder.reminder).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", new { date = d });// RedirectToAction("Details", "Customers", new { id = appointment.CustomerID });
                }
                ViewBag.FromHours = new SelectList(Enumerable.Range(0, 24).Select(r => r.ToString("00")).AsEnumerable(), appointmentreminder.appointment.FromTime_Hour.GetValueOrDefault(16).ToString("00"));
                ViewBag.FromMinutes = new SelectList(Enumerable.Range(0, 59).Where(r => (r % 5) == 0).Select(r => r.ToString("00")).AsEnumerable(), appointmentreminder.appointment.FromTime_Minutes.GetValueOrDefault(0).ToString("00"));

                ViewBag.ToHours = new SelectList(Enumerable.Range(0, 24).Select(r => r.ToString("00")).AsEnumerable(), appointmentreminder.appointment.ToTime_Hour.GetValueOrDefault(16).ToString("00"));
                ViewBag.ToMinutes = new SelectList(Enumerable.Range(0, 59).Where(r => (r % 5) == 0).Select(r => r.ToString("00")).AsEnumerable(), appointmentreminder.appointment.ToTime_Minutes.GetValueOrDefault(20).ToString("00"));

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
                                            OnDate = Classes.SharedClass.Now().AddHours(2),
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
