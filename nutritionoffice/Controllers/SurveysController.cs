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
using nutritionoffice.ViewModels;

namespace nutritionoffice.Controllers
{
    public class SurveysController : MyBaseController
    {
        private ndbContext db = new ndbContext();

        // GET: Surveys
        public async Task<ActionResult> Index()
        {
            var companyid = CompanyID();
            var surveys = db.Surveys.Include(s => s.Company).Where(r=>r.CompanyID==companyid);
            return View(await surveys.ToListAsync());
        }

        
        // GET: Surveys/Create
        public ActionResult Create()
        {
            var companyid = CompanyID();
            SurveyView surveyview = new SurveyView {Survey=new Survey { CompanyID=companyid, IsActive=true }, Questions=new QuestionView[] { new QuestionView { Deleted=false, Question=new Question { Type= Question.QuestionType.Freetext } } } };
            return View(surveyview);
        }

        // POST: Surveys/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public async Task<ActionResult> CreatePost( SurveyView surveyview)
        {
            //if (ModelState.IsValid)
            //{
            //    db.Surveys.Add(survey);
            //    await db.SaveChangesAsync();
            //    return RedirectToAction("Index");
            //}

            return View(surveyview);
        }

        public PartialViewResult AddQuestionView()
        {
            return PartialView();
        }


        // GET: Surveys/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Survey survey = await db.Surveys.FindAsync(id);
            if (survey == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyID = new SelectList(db.Companies, "id", "Notes", survey.CompanyID);
            return View(survey);
        }

        // POST: Surveys/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,CompanyID,Title,IsActive")] Survey survey)
        {
            if (ModelState.IsValid)
            {
                db.Entry(survey).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CompanyID = new SelectList(db.Companies, "id", "Notes", survey.CompanyID);
            return View(survey);
        }

        // GET: Surveys/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Survey survey = await db.Surveys.FindAsync(id);
            if (survey == null)
            {
                return HttpNotFound();
            }
            return View(survey);
        }

        // POST: Surveys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Survey survey = await db.Surveys.FindAsync(id);
            db.Surveys.Remove(survey);
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
