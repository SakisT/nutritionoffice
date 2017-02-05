using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using nutritionoffice.Models;

namespace nutritionoffice.Controllers
{
    [Authorize(Roles = "Administrator,Owner")]
    public class TargetGroupsController : MyBaseController
    {
        private ndbContext db = new ndbContext();

        // GET: TargetGroups
        public async Task<ActionResult> Index()
        {
            try
            {
                int CompID = CompanyID();
                List<TargetGroup> companytargetgroups = await db.TargetGroups.Where(r => r.CompanyID == CompID).ToListAsync();
                return View(companytargetgroups);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "TargetGroupsController - Index");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // GET: TargetGroups/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                int CompID = CompanyID();
                TargetGroup targetGroup = await db.TargetGroups.FindAsync(id);
                if (targetGroup == null || targetGroup.CompanyID != CompID)
                {
                    return HttpNotFound();
                }

                return View(targetGroup);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "TargetGroupsController - Details");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // GET: TargetGroups/Create
        public ActionResult Create()
        {
            try
            {
                int CompID = CompanyID();
                TargetGroup targetgroup = new TargetGroup { IsActive = true, CompanyID = CompID };
                return View(targetgroup);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "TargetGroupsController - Create");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // POST: TargetGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,Name,IsActive,Notes,CompanyID")] TargetGroup targetGroup)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.TargetGroups.Add(targetGroup);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

                return View(targetGroup);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "TargetGroupsController - Create (Post)");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // GET: TargetGroups/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                int CompID = CompanyID();
                TargetGroup targetGroup = await db.TargetGroups.FindAsync(id);
                if (targetGroup == null || targetGroup.CompanyID != CompID)
                {
                    return HttpNotFound();
                }
                return View(targetGroup);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "TargetGroupsController - Edit");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // POST: TargetGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,Name,IsActive,Notes,CompanyID")] TargetGroup targetGroup)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(targetGroup).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                return View(targetGroup);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "TargetGroupsController - Edit (POST)");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // GET: TargetGroups/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                int CompID = CompanyID();
                TargetGroup targetGroup = await db.TargetGroups.FindAsync(id);
                if (targetGroup == null || targetGroup.CompanyID != CompID)
                {
                    return HttpNotFound();
                }
                return View(targetGroup);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "TargetGroupsController - Delete");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // POST: TargetGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                TargetGroup targetGroup = await db.TargetGroups.FindAsync(id);
                db.TargetGroups.Remove(targetGroup);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "TargetGroupsController - DeleteConfirmed");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ActionResult> ListGroupCustomerToSendReminder(int id)
        {
            try
            {
                int CompID = CompanyID();
                TargetGroup targetGroup = await db.TargetGroups.FindAsync(id);

                if (targetGroup == null || targetGroup.CompanyID != CompID)
                {
                    return HttpNotFound();
                }
                ViewBag.GroupName = string.Format($"Create Bulk Reminder for group '{targetGroup.Name}'");

                List<ViewModels.selectablecustomer> groupcustomers = await (from Customer p in db.Customers.Where(r => r.TargetGroupID == id) select new ViewModels.selectablecustomer { customer = p, IsSelected = true }).ToListAsync();

                return View(groupcustomers);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, string.Format("{0} - {1}", "TargetGroupsController", "ListGroupCustomerToSendReminder"));
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }
        [HttpPost]
        public async Task<ActionResult> ListGroupCustomerToSendReminder(IEnumerable<ViewModels.selectablecustomer> groupcustomers)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        if (Request["message"] == null) { throw new Exception("Fill out message text!"); }
                        string Message = Request["message"].ToString();
                        if (string.IsNullOrEmpty(Message)) { throw new Exception(string.Format("Message is Empty !!!", "")); }

                        string ondatestring = Request["ondate"].ToString();
                        DateTime OnDate;
                        if (!DateTime.TryParse(ondatestring, out OnDate)) { throw new Exception(String.Format("{0} is not a valid Date", ondatestring)); }

                        string OnTime = Request["ontime"].ToString();
                        System.Text.RegularExpressions.Regex Rule = new System.Text.RegularExpressions.Regex("^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$");
                        if (!Rule.IsMatch(OnTime)) { throw new Exception(string.Format("{0} is not a valid time", OnTime)); }
                        string[] TimeParts = OnTime.Split(':').ToArray();
                        int CompID = CompanyID();
                        DateTime SentDateTime = new DateTime(OnDate.Year, OnDate.Month, OnDate.Day, Convert.ToInt32(TimeParts[0]), Convert.ToInt32(TimeParts[1]), 0);
                        foreach (ViewModels.selectablecustomer item in groupcustomers.Where(r => r.IsSelected == true))
                        {
                            Customer customer = db.Customers.Find(item.customer.id);
                            if (customer != null && customer.CompanyID == CompID)
                            {
                                if ((item.SendEmail && !string.IsNullOrEmpty(item.customer.email)) | (item.SendSMS && !string.IsNullOrEmpty(item.customer.Mobile)))
                                {
                                    Reminder newreminder = new Reminder { CustomerID = customer.id, Message = Message, OnDate = SentDateTime, SMSState = Reminder.ReminderState.Active, SendSMS = item.SendSMS, SendEmail = item.SendEmail, MailState = Reminder.ReminderState.Active, Mobile = customer.Mobile ?? "", email = customer.email ?? "" };
                                    db.Reminders.Add(newreminder);
                                    Console.WriteLine();
                                }
                            }
                        }
                        db.SaveChanges();
                        return RedirectToAction("Index", "Reminders", null);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("CustomError", string.Format($"Error : {ex.Message}"));
                    }

                }
                TargetGroup targetgroup = await db.TargetGroups.FindAsync(groupcustomers.FirstOrDefault().customer.TargetGroupID);
                ViewBag.GroupName = targetgroup.Name;
                return View(groupcustomers);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, string.Format("{0} - {1}", "TargetGroupsController", "ListGroupCustomerToSendReminder (Post)"));
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
