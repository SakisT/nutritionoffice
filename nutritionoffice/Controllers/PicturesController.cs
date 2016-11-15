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
    public class PicturesController : MyBaseController
    {
        private ndbContext db = new ndbContext();

        // GET: Pictures
        public async Task<ActionResult> Index(int id = 0)
        {
            var pictures = db.Pictures.Include(p => p.Company);
            if (id != 0)
            {
                return RedirectToAction("Edit", new { id = id });
            }
            return View(await pictures.ToListAsync());
        }

        // GET: Pictures/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Picture picture = await db.Pictures.FindAsync(id);
            if (picture == null)
            {
                return HttpNotFound();
            }
            return View(picture);
        }

        // GET: Pictures/Create
        public ActionResult Create()
        {
            ViewBag.PictureID = new SelectList(db.Companies, "id", "CompanyName");
            return View();
        }

        // POST: Pictures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PictureID,CompanyID,Logo")] Picture picture)
        {
            if (ModelState.IsValid)
            {
                db.Pictures.Add(picture);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.PictureID = new SelectList(db.Companies, "id", "CompanyName", picture.PictureID);
            return View(picture);
        }

        // GET: Pictures/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Picture picture = await db.Pictures.FindAsync(id);
            if (picture == null)
            {
                picture = new Picture { CompanyID = id.Value, PictureID = id.Value, Logo = new byte[] { }, ReportBackgroundPortrait = new byte[] { }, ReportBackgroundLandscape = new byte[] { } };
                db.Pictures.Add(picture);
                await db.SaveChangesAsync();
                //return HttpNotFound();
            }
            ViewBag.PictureID = new SelectList(db.Companies, "id", "CompanyName", picture.PictureID);
            return View(picture);
        }

        // POST: Pictures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PictureID,CompanyID,Logo,ReportBackgroundPortrait,ReportBackgroundLandscape")] Picture picture)
        {
            if (ModelState.IsValid)
            {
                foreach (string upload in Request.Files)
                {
                    if (Request.Files[upload].FileName == "") continue;
                    HttpPostedFileWrapper file = (HttpPostedFileWrapper)Request.Files[upload];
                    byte[] Image;
                    using (var binaryReader = new System.IO.BinaryReader(file.InputStream))
                    {
                        Image = binaryReader.ReadBytes(file.ContentLength);
                    }
                    if (Image.Count() != 0) {
                        switch (upload)
                        {
                            case "Logo":
                                picture.Logo = Image;
                                break;
                            case "ReportBackgroundPortrait":
                                picture.ReportBackgroundPortrait= Image;
                                break;
                            case "ReportBackgroundLandscape":
                                picture.ReportBackgroundLandscape = Image;
                                break;
                        }
                    }
                }
                //if (Request.Files["Logo"] != null)
                //{
                //    HttpPostedFileWrapper file = (HttpPostedFileWrapper)Request.Files["Logo"];
                //    if (file.FileName != "")
                //    {
                //        byte[] Image;
                //        using (var binaryReader = new System.IO.BinaryReader(file.InputStream))
                //        {
                //            Image = binaryReader.ReadBytes(file.ContentLength);
                //        }
                //        if (Image.Count() != 0) { picture.Logo = Image; }
                //    }
                //}
                db.Entry(picture).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.PictureID = new SelectList(db.Companies, "id", "CompanyName", picture.PictureID);
            return View(picture);
        }

        // GET: Pictures/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Picture picture = await db.Pictures.FindAsync(id);
            if (picture == null)
            {
                return HttpNotFound();
            }
            return View(picture);
        }

        // POST: Pictures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Picture picture = await db.Pictures.FindAsync(id);
            db.Pictures.Remove(picture);
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
