using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using nutritionoffice.Models;

namespace nutritionoffice.Controllers
{
    [Authorize(Roles = "Administrator,Owner")]
    public class DailyRecallsController : MyBaseController
    {
        private ndbContext db = new ndbContext();

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> CreateOrEdit(int id)
        {
            DailyRecall recall = await db.DailyRecalls.Where(r => r.CustomerID == id).FirstOrDefaultAsync();
            if (recall == null)
            {
                return RedirectToAction("Create", new { id = id });
            }
            return RedirectToAction("Edit", new { id = recall.id });
        }

        public async Task<ActionResult> Create(int id)
        {
            Customer customer = await db.Customers.FindAsync(id);
            DailyRecall dailyrecall = new DailyRecall { CustomerID = id, Customer = customer };
            return View(dailyrecall);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,CustomerID,Breakfast,MorningSnack,Lunch,EveningSnack,Dinner,Milk,Yoghurt,WhiteCheese,YellowCheese,CottageCheese,Chicken,Turkey,Hamburger,Beef,Pork,InOilFood,Legumes,Cereals,Nuts,Alcohol,JunkFood,Salads,Fruits,LikeA,LikeB,LikeC,LikeD,LikeE,DislikeA,DislikeB,DislikeC,DislikeD,DislikeE,Notes")] DailyRecall dailyRecall)
        {
            if (ModelState.IsValid)
            {
                db.DailyRecalls.Add(dailyRecall);
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Customers", new { id = dailyRecall.CustomerID });
            }
            return View(dailyRecall);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DailyRecall dailyRecall = await db.DailyRecalls.FindAsync(id);
            if (dailyRecall == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "id", "LastName", dailyRecall.CustomerID);
            return View(dailyRecall);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<JsonResult> SendDailyRecallByEmail(int id, string MessageText)
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
                message.Subject = Resource.DailyRecall;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,CustomerID,Breakfast,MorningSnack,Lunch,EveningSnack,Dinner,Milk,Yoghurt,WhiteCheese,YellowCheese,CottageCheese,Chicken,Turkey,Hamburger,Beef,Pork,InOilFood,Legumes,Cereals,Nuts,Alcohol,JunkFood,Salads,Fruits,LikeA,LikeB,LikeC,LikeD,LikeE,DislikeA,DislikeB,DislikeC,DislikeD,DislikeE,Notes")] DailyRecall dailyRecall)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dailyRecall).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Customers", new { id = dailyRecall.CustomerID });
            }
            return View(dailyRecall);
        }

        //-------------------------------------- C u s t o m e r   A c t i o n s ---------------------------------------
        [AllowAnonymous]
        public async Task<ActionResult> CreateOrEditByCustomer(string CustomerGuid)
        {
            Guid g = Guid.Parse(CustomerGuid);
            Customer customer = await db.Customers.Where(r => r.CustomerGUID == g).FirstOrDefaultAsync();
            if (customer == null) { return RedirectToAction("Index"); }
            DailyRecall recall = await db.DailyRecalls.Where(r => r.CustomerID == customer.id).FirstOrDefaultAsync();
            if (recall == null)
            {
                return RedirectToAction("CreateByCustomer", new { CustomerGuid = g });
            }
            return RedirectToAction("EditByCustomer", new { CustomerGuid = g });
        }

        [AllowAnonymous]
        public async Task<ActionResult> CreateByCustomer(Guid CustomerGuid)
        {
            Customer customer = await db.Customers.FirstAsync(r => r.CustomerGUID == CustomerGuid);
            DailyRecall dailyrecall = new DailyRecall { CustomerID = customer.id, Customer = customer };
            ViewBag.HideMenu = true;
            return View(dailyrecall);
        }

        [HttpPost]
        [ValidateAntiForgeryToken, AllowAnonymous]
        public async Task<ActionResult> CreateByCustomer([Bind(Include = "id,CustomerID,Breakfast,MorningSnack,Lunch,EveningSnack,Dinner,Milk,Yoghurt,WhiteCheese,YellowCheese,CottageCheese,Chicken,Turkey,Hamburger,Beef,Pork,InOilFood,Legumes,Cereals,Nuts,Alcohol,JunkFood,Salads,Fruits,LikeA,LikeB,LikeC,LikeD,LikeE,DislikeA,DislikeB,DislikeC,DislikeD,DislikeE,Notes")] DailyRecall dailyRecall)
        {
            if (ModelState.IsValid)
            {
                db.DailyRecalls.Add(dailyRecall);
                await db.SaveChangesAsync();
                db.Entry(dailyRecall).Reference(r => r.Customer).Load();
                return RedirectToAction("SuccessfulRegistration", new { id = dailyRecall.Customer.CompanyID });
            }
            return View(dailyRecall);
        }

        [AllowAnonymous]
        public async Task<ActionResult> EditByCustomer(Guid CustomerGuid)
        {
            if (CustomerGuid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.Customers.FirstAsync(r => r.CustomerGUID == CustomerGuid);

            DailyRecall dailyRecall = await db.DailyRecalls.FirstAsync(r => r.CustomerID == customer.id);
            ViewBag.HideMenu = true;
            return View(dailyRecall);
        }

        [HttpPost]
        [ValidateAntiForgeryToken, AllowAnonymous]
        public async Task<ActionResult> EditByCustomer([Bind(Include = "CustomerID,Breakfast,MorningSnack,Lunch,EveningSnack,Dinner,Milk,Yoghurt,WhiteCheese,YellowCheese,CottageCheese,Chicken,Turkey,Hamburger,Beef,Pork,InOilFood,Legumes,Cereals,Nuts,Alcohol,JunkFood,Salads,Fruits,LikeA,LikeB,LikeC,LikeD,LikeE,DislikeA,DislikeB,DislikeC,DislikeD,DislikeE,Notes,id")] DailyRecall dailyRecall)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dailyRecall).State = EntityState.Modified;
                await db.SaveChangesAsync();
                db.Entry(dailyRecall).Reference(r => r.Customer).Load();
                return RedirectToAction("SuccessfulRegistration", new { id = dailyRecall.Customer.CompanyID });
            }
            return View(dailyRecall);
        }

        [AllowAnonymous]
        public ActionResult SuccessfulRegistration(int id)
        {
            byte[] logo = null;
            using (ndbContext db = new ndbContext())
            {
                Company company = db.Companies.Find(id);
                if (company != null && company.logo != null && company.logo.Length > 10)
                {
                    logo = company.logo;
                }
            }
            ViewBag.HideMenu = true;
            ViewBag.Logo = logo;
            return View();
        }

        [AllowAnonymous]
        public ActionResult ErrorFindingData(string ErrorMessage)
        {
            return View();
        }
        //-------------------------------------- C u s t o m e r   A c t i o n s ---------------------------------------

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DailyRecall dailyRecall = await db.DailyRecalls.FindAsync(id);
            if (dailyRecall == null)
            {
                return HttpNotFound();
            }
            return View(dailyRecall);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            DailyRecall dailyRecall = await db.DailyRecalls.FindAsync(id);
            db.DailyRecalls.Remove(dailyRecall);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
