using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using nutritionoffice.Models;
using System.Linq;

namespace nutritionoffice.Controllers
{
    [Authorize(Roles = "Administrator,Owner,CanEdit")]
    public class FoodCategoriesController : MyBaseController
    {
        private ndbContext db = new ndbContext();

        public async Task<ActionResult> Index()
        {
            IQueryable<FoodCategory> foodcategories = db.FoodCategories.Include(r => r.Company);
            if (!User.IsInRole("Administrator"))
            {
                int companyid = CompanyID();
                if (User.IsInRole("CanEdit"))//Έχει πρόσβαση στα δικά του και τα γενικά.
                {
                    foodcategories = foodcategories.Where(r => !r.CompanyID.HasValue || Nullable.Equals(r.CompanyID, companyid));
                }
                else
                {
                    foodcategories = foodcategories.Where(r => Nullable.Equals(r.CompanyID, companyid));
                }
            }
            return View(await foodcategories.ToListAsync());
        }

        public async Task<ActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                FoodCategory foodCategory = await db.FoodCategories.FindAsync(id);
                if (foodCategory == null)
                {
                    return HttpNotFound();
                }
                return View(foodCategory);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public ActionResult Create()
        {
            FoodCategory foodcategory = new FoodCategory();
            if (!User.IsInRole("Administrator"))//Μόνο εγώ δημιουργώ κατηγορίες προσβάσιμες από όλους (CompanyID=null).
            {
                int companyid = CompanyID();
                foodcategory.CompanyID = companyid;
            }
            return View(foodcategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,EnglishName, GreekName, CompanyID")] FoodCategory foodCategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Αν πάμε να δημιουργήσουμε κατηγορία ενός Company ελέγχουμε μήπως το ίδιο Company έχει ήδη κατηγορία με αυτό το όνομα
                    if (foodCategory.CompanyID.HasValue)
                    {
                        var existingcategory = db.FoodCategories.Where(r => Nullable.Equals(r.CompanyID, foodCategory.CompanyID) && r.GreekName.Equals(foodCategory.GreekName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                        if (existingcategory != null) { throw new Exception("Food Category allready exists!"); }
                    }
                    else
                    {
                        var existingcategory = db.FoodCategories.Where(r => !r.CompanyID.HasValue && r.GreekName.Equals(foodCategory.GreekName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                        if (existingcategory != null) { throw new Exception("Food Category allready exists!"); }
                    }
                    db.FoodCategories.Add(foodCategory);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

                return View(foodCategory);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public async Task<ActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                FoodCategory foodCategory = await db.FoodCategories.FindAsync(id);
                if (foodCategory == null)
                {
                    return HttpNotFound();
                }
                return View(foodCategory);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,EnglishName,GreekName, CompanyID")] FoodCategory foodCategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!User.IsInRole("Administrator"))
                    {
                        int compayid = CompanyID();
                        if (foodCategory.CompanyID == null)//Αν είναι γενικής χρήσης μόνο Administrators και CanEdit μπορούν να το αλλάξουν
                        {
                            if (!User.IsInRole("CanEdit")) { throw new Exception("Μόνο Administrators και CanEdit μπορούν να διορθώσουν αυτά τα FoodCategories"); }
                        }
                        else
                        {
                            if (!Nullable.Equals(foodCategory.CompanyID, compayid)) { throw new Exception("Μόνο Adnministrators ή κάτοχοι της Εταιρίας μπορούν να διορθώσουν αυτό το FoodCategory"); }
                        }
                    }
                    db.Entry(foodCategory).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                return View(foodCategory);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError,ex.Message);
            }
        }

        public async Task<ActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                FoodCategory foodCategory = await db.FoodCategories.FindAsync(id);
                if (foodCategory == null)
                {
                    return HttpNotFound();
                }
                if (foodCategory.Foods.Count() > 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Food Category has foods!");
                }
                return View(foodCategory);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError,ex.Message);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                FoodCategory foodCategory = await db.FoodCategories.FindAsync(id);
                if (!User.IsInRole("Administrator"))
                {
                    int companyid = CompanyID();
                    if (foodCategory.CompanyID == null)//Αν είναι γενικής χρήσης
                    {
                        if (!User.IsInRole("CanEdit"))
                        {
                            throw new Exception("Μόνο Administrators και CanEdit μπορούν να διαγράψουν τις γενικής χρήσης κατηγορίες.");
                        }
                    }
                   else//Ανήκει σε κάποια Εταιρία
                    {
                        if (!Nullable.Equals(foodCategory.CompanyID, companyid)) { throw new Exception("Μόνο Administartors και Owners μπορούν να διαγράψουν αυτό το FoodCategory."); }
                    }
                }
                db.FoodCategories.Remove(foodCategory);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
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
