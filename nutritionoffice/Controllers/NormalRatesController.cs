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
    public class NormalRatesController : MyBaseController
    {
        private ndbContext db = new ndbContext();

        // GET: NormalRates
        public async Task<ActionResult> Index()
        {
            return View(await db.NormalRates.OrderBy(r=>r.Type).ToListAsync());
        }

        // GET: NormalRates/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NormalRates normalRates = await db.NormalRates.FindAsync(id);
            if (normalRates == null)
            {
                return HttpNotFound();
            }
            return View(normalRates);
        }

        // GET: NormalRates/Create
        public ActionResult Create()
        {
            var normalRates = new NormalRates { Color = "#ff0000" };
            return View(normalRates);
        }

        // POST: NormalRates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,Type,Sex,FromAge,ToAge,FromValue,ToValue,ValueUnit,Notes,Color")] NormalRates normalRates)
        {
            if (ModelState.IsValid)
            {
                db.NormalRates.Add(normalRates);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(normalRates);
        }

        // GET: NormalRates/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NormalRates normalRates = await db.NormalRates.FindAsync(id);
            if (normalRates == null)
            {
                return HttpNotFound();
            }
            return View(normalRates);
        }

        // POST: NormalRates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,Type,Sex,FromAge,ToAge,FromValue,ToValue,ValueUnit,Notes,Color")] NormalRates normalRates)
        {
            if (ModelState.IsValid)
            {
                db.Entry(normalRates).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(normalRates);
        }

        // GET: NormalRates/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NormalRates normalRates = await db.NormalRates.FindAsync(id);
            if (normalRates == null)
            {
                return HttpNotFound();
            }
            return View(normalRates);
        }

        // POST: NormalRates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            NormalRates normalRates = await db.NormalRates.FindAsync(id);
            db.NormalRates.Remove(normalRates);
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
