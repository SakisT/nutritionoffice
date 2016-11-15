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
    public class RecipesController : MyBaseController
    {
        private ndbContext db = new ndbContext();

        public async Task<ActionResult> Index()
        {
            int companyid = this.CompanyID();
            var recipes = db.Recipes.Include(r => r.RecipeCategory).Where(r=>r.RecipeCategory.CompanyID==companyid);
            return View(await recipes.ToListAsync());
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = await db.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        public ActionResult Create()
        {
            int companyid = this.CompanyID();
            IQueryable<RecipeCategory> availablecategories = db.RecipeCategories.Where(r => r.CompanyID == companyid);
            ViewBag.RecipeCategoryID = new SelectList(availablecategories, "id", "Name");
            return View();
        }

        // POST: Recipes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,RecipeCategoryID,Name,Description,Notes,FileGuid")] Recipe recipe, HttpPostedFileBase upload)
        {
            int companyid = this.CompanyID();
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    System.IO.FileInfo sourcefi = new System.IO.FileInfo(upload.FileName);
                    Guid fileguid = Guid.NewGuid();

                    string DestinationDirectory = Server.MapPath("~/files/documents/");
                    System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(DestinationDirectory);
                    if (!di.Exists)
                    {
                        di.Create();
                        di.Refresh();
                    }
                    string filename = fileguid + sourcefi.Extension;

                    recipe.FileGuid = filename;
                    db.Recipes.Add(recipe);

                    string DestinationPath = System.IO.Path.Combine(di.FullName, filename);
                    using (System.IO.StreamWriter stream = new System.IO.StreamWriter(DestinationPath, false))
                    {
                        upload.InputStream.CopyTo(stream.BaseStream);
                        stream.Flush();
                        stream.Close();
                    }

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                List<RecipeCategory> CurrentCompanyRecipeCategories = await db.RecipeCategories.Where(r => r.CompanyID == companyid).ToListAsync();
                ViewBag.RecipeCategoryID = new SelectList(CurrentCompanyRecipeCategories, "id", "Name", recipe.RecipeCategoryID);
                return View(recipe);
            }
            IQueryable<RecipeCategory> availablecategories = db.RecipeCategories.Where(r => r.CompanyID == companyid);
            ViewBag.RecipeCategoryID = new SelectList(availablecategories, "id", "Name", recipe.RecipeCategoryID);
            return View(recipe);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = await db.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            ViewBag.RecipeCategoryID = new SelectList(db.RecipeCategories, "id", "Name", recipe.RecipeCategoryID);
            return View(recipe);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,RecipeCategoryID,Name,Description,Notes,FileGuid")] Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                db.Entry(recipe).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.RecipeCategoryID = new SelectList(db.RecipeCategories, "id", "Name", recipe.RecipeCategoryID);
            return View(recipe);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = await db.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Recipe recipe = await db.Recipes.FindAsync(id);
            db.Recipes.Remove(recipe);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public FileResult DownloadFile(int id)
        {
            int companyid = this.CompanyID();
            Recipe recipe = db.Recipes.Find(id);
            string fullpath = System.IO.Path.Combine(Server.MapPath("~/files/documents/" + recipe.FileGuid));
            System.IO.FileInfo fi = new System.IO.FileInfo(fullpath);
            return File(fi.FullName, System.Net.Mime.MediaTypeNames.Application.Octet, string.Format("{0}{1}", recipe.Name, fi.Extension));
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
