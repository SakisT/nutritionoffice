using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using nutritionoffice.Models;
using System.IO;
using PagedList;

namespace nutritionoffice.Controllers
{
    [Authorize(Roles = "Administrator,Owner")]
    public class CompaniesController : MyBaseController
    {
        private ndbContext db = new ndbContext();

        [HttpGet]
        public async Task<JsonResult> GetSMSBalance()
        {
            int companyid = CompanyID();
            var company = await db.Companies.FindAsync(companyid);
            if (company.SMSSign != null && company.SMSUserName != null && company.SMSPassword != null)
            {
                using (var client = new WebClient())
                {
                    var balace = client.DownloadString("http://ez4usms.com/api/http/credits.php?username=_SakisT&password=81164583");
                    return Json(new
                    {
                        Result = balace.Contains("Credits:") ? "Success" : "Error",
                        Credits = balace
                    }, JsonRequestBehavior.AllowGet);
                }                    
            }
            return Json(new { Result="Error" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost,Authorize(Roles = "Administrator")]
        public async Task<JsonResult> ChangeActiveCompany(int? id)
        {
            if (id == 0)
            {
                return Json(new { Result="Error"}, JsonRequestBehavior.AllowGet);
            }
            var company = await db.Companies.FindAsync(id.Value);
            if (company == null)
            {
                return Json(new { Result = "Error" }, JsonRequestBehavior.AllowGet);
            }
            Session["CompanyID"] = id.Value.ToString();
            return Json(new { Result ="Success", Message=string.Format($"{company.CompanyName} is Active Company!")}, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Index(int? page)
        {
            try
            {
                int pageSize = 10;
                List<Company> companies = await db.Companies.ToListAsync();
                if (!User.IsInRole("Administrator"))
                {
                    int CompID = CompanyID();
                    companies = companies.Where(r => r.id == CompID).ToList();
                }
                int pageNumber = (page ?? 1);
                return View(companies.ToPagedList(pageNumber, pageSize));
                //return View(companies);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // GET: Companies/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Company company = await db.Companies.FindAsync(id);
                if (company == null)
                {
                    return HttpNotFound();
                }
                return View(company);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // GET: Companies/Create
        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            try
            {
                Company company = new Company { SMTPEnableSSL = true, SMTPHost = "smtp.gmail.com", SMTPPort = 587, IsDemo=true, StartDate=DateTime.Today };
                return View(company);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // POST: Companies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,CompanyName,Owner,Phone,email,SMSSign,EmergencyPhone,Address,City,PostCode,FaceBook,Twitter,Instagram,SMTPHost,SMTPEnableSSL,SMTPPort,SMTPUserName,SMTPPassword,logo,SMSUserName,SMSPassword,IsDemo,StartDate,LastPayment,Euro,Notes")] Company company)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (Request.Files["files"] != null)
                    {
                        byte[] Image;
                        using (var binaryReader = new BinaryReader(Request.Files["files"].InputStream))
                        {
                            Image = binaryReader.ReadBytes(Request.Files["files"].ContentLength);
                        }
                        if (Image.Count() != 0) { company.logo = Image; }
                    }
                    db.Companies.Add(company);

                    Picture companypicture = new Picture { Company = company, Logo = new byte[] { }, ReportBackgroundLandscape = new byte[] { }, ReportBackgroundPortrait = new byte[] { } };

                    db.Pictures.Add(companypicture);

                    await db.SaveChangesAsync();
                    if (!User.IsInRole("Administrator")) { return RedirectToAction("Index", "Home"); }
                    return RedirectToAction("Index");
                }

                return View(company);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // GET: Companies/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                int CompID = CompanyID();
                Company company = await db.Companies.FindAsync(id);
                if (!User.IsInRole("Administrator")) { company = await db.Companies.FindAsync(CompID); }
                if (company == null)
                {
                    return HttpNotFound();
                }
                return View(company);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,CompanyName,Owner,Phone,email,SMSSign,EmergencyPhone,Address,City,PostCode,FaceBook,Twitter,Instagram,SMTPHost,SMTPEnableSSL,SMTPPort,SMTPUserName,SMTPPassword,logo,SMSUserName,SMSPassword,IsDemo,StartDate,LastPayment,Euro,Notes")] Company company)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (Request.Files["files"] != null)
                    {
                        byte[] Image;
                        using (var binaryReader = new BinaryReader(Request.Files["files"].InputStream))
                        {
                            Image = binaryReader.ReadBytes(Request.Files["files"].ContentLength);
                        }
                        if (Image.Count() != 0) { company.logo = Image; }
                    }
                    db.Entry(company).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    if (!User.IsInRole("Administrator")) { return RedirectToAction("Index", "Home"); }
                    return RedirectToAction("Index");
                }
                return View(company);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (User.IsInRole("Administrator"))
                {
                    Company company = await db.Companies.FindAsync(id);
                    if (company == null)
                    {
                        return HttpNotFound();
                    }
                    return View(company);
                }
                return HttpNotFound();
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                Company company = await db.Companies.FindAsync(id);
                db.Companies.Remove(company);
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
