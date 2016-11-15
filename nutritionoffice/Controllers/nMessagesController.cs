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
    public class nMessagesController : Controller
    {
        private ndbContext db = new ndbContext();

        public async Task<ActionResult> Index()
        {
            var messages = db.Messages.Include(n => n.Company);
            return View(await messages.ToListAsync());
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            nMessage nMessage = await db.Messages.FindAsync(id);
            if (nMessage == null)
            {
                return HttpNotFound();
            }
            return View(nMessage);
        }

        public ActionResult Create()
        {
            ViewBag.CompanyID = new SelectList(db.Companies, "id", "CompanyName");
            return View();
        }

        // POST: nMessages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,CompanyID,Recipient,Subject,MessageBody,Attatchments,Type,Status,StatusCode")] nMessage nMessage)
        {
            if (ModelState.IsValid)
            {
                db.Messages.Add(nMessage);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CompanyID = new SelectList(db.Companies, "id", "CompanyName", nMessage.CompanyID);
            return View(nMessage);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            nMessage nMessage = await db.Messages.FindAsync(id);
            if (nMessage == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyID = new SelectList(db.Companies, "id", "CompanyName", nMessage.CompanyID);
            return View(nMessage);
        }

        // POST: nMessages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,CompanyID,Recipient,Subject,MessageBody,Attatchments,Type,Status,StatusCode")] nMessage nMessage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nMessage).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CompanyID = new SelectList(db.Companies, "id", "CompanyName", nMessage.CompanyID);
            return View(nMessage);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            nMessage nMessage = await db.Messages.FindAsync(id);
            if (nMessage == null)
            {
                return HttpNotFound();
            }
            return View(nMessage);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            nMessage nMessage = await db.Messages.FindAsync(id);
            db.Messages.Remove(nMessage);
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
