using nutritionoffice.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace nutritionoffice.Controllers
{
    public class NutritionOfficeSettingsController : MyBaseController
    {
        private ndbContext db = new ndbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AgeRangesIndex()
        {
            int companyid = CompanyID();
            Company company = db.Companies.Find(companyid);
            db.Entry(company).Collection(r => r.AgeRanges).Load();
            if (company.AgeRanges.Count() == 0)
            {
                db.AgeRanges.Add(new AgeRange { CompanyID = companyid, FromAge = 0d, ToAge = 5d });
                db.AgeRanges.Add(new AgeRange { CompanyID = companyid, FromAge = 5d, ToAge = 12d });
                db.AgeRanges.Add(new AgeRange { CompanyID = companyid, FromAge = 12d, ToAge = 18d });
                db.AgeRanges.Add(new AgeRange { CompanyID = companyid, FromAge = 18d, ToAge = 25d });
                db.AgeRanges.Add(new AgeRange { CompanyID = companyid, FromAge = 25d, ToAge = 45d });
                db.AgeRanges.Add(new AgeRange { CompanyID = companyid, FromAge = 45d, ToAge = 60d });
                db.AgeRanges.Add(new AgeRange { CompanyID = companyid, FromAge = 60d, ToAge = 75d });
                db.AgeRanges.Add(new AgeRange { CompanyID = companyid, FromAge = 75d, ToAge = 120d });
                db.SaveChanges();
            }
            return View(company.AgeRanges.OrderBy(r=>r.FromAge).ThenBy(r=>r.ToAge).ToList());
        }

        public ActionResult AgeRangeCreate()
        {
            int companyid = CompanyID();
            Company company = db.Companies.Find(companyid);
            AgeRange agerange = new AgeRange {  CompanyID=companyid};
            return View(agerange);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AgeRangeCreate([Bind(Include = "id,CompanyID,FromAge,ToAge")] AgeRange agerange)
        {
            if (ModelState.IsValid)
            {
                db.AgeRanges.Add(agerange);
                await db.SaveChangesAsync();
                return RedirectToAction("AgeRangesIndex");
            }
            return View(agerange);
        }

        public async Task<ActionResult> AgeRangeEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AgeRange agerange = await db.AgeRanges.FindAsync(id);
            int companyid = CompanyID();
            if (agerange == null || agerange.CompanyID != companyid)
            {
                return HttpNotFound();
            }
            return View(agerange);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AgeRangeEdit([Bind(Include = "id,CompanyID,FromAge,ToAge")] AgeRange agerange)
        {
            if (ModelState.IsValid)
            {
                db.Entry(agerange).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("AgeRangesIndex");
            }
            return View(agerange);
        }

        public async Task<ActionResult> AgeRangeDelete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                int companyid = CompanyID();
                AgeRange agerange = await db.AgeRanges.FindAsync(id);
                if (agerange == null || agerange.CompanyID != companyid)
                {
                    return HttpNotFound();
                }
                return View(agerange);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, string.Format("{0} - {1}", "CustomersController", "Delate"));
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("AgeRangeDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AgeRangeDeleteConfirmed(int id)
        {
            try
            {
                AgeRange agerange = await db.AgeRanges.FindAsync(id);
                db.AgeRanges.Remove(agerange);
                await db.SaveChangesAsync();
                return RedirectToAction("AgeRangesIndex");
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, string.Format("{0} - {1}", "CustomersController", "Delete (Confirmed)"));
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