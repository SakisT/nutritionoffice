using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using nutritionoffice.Models;
using PagedList;
using System;

namespace nutritionoffice.Controllers
{
    [Authorize(Roles = "Administrator,Owner,CanEdit")]
    public class FoodsController : MyBaseController
    {
        private ndbContext db = new ndbContext();

        public async Task<ActionResult> List(string searchterms="")
        {
            int companyid = CompanyID();
            IQueryable<Food> generalfoodd = db.Foods.Include(r=>r.FoodCategory).Where(r => r.FoodCategory.CompanyID == null && r.GreekName.ToLower().Contains(searchterms.ToLower()));
            IQueryable<Food> ownfood = db.Foods.Include(r => r.FoodCategory).Where(r => r.FoodCategory.CompanyID == companyid && r.GreekName.ToLower().Contains(searchterms.ToLower()));
            var result = generalfoodd.Concat(ownfood.AsEnumerable());
            return View(await result.ToListAsync());
        }

        public ActionResult Index(int? page, int? FoodCategoryID, string GreekSearch = "", string EnglishSearch = "", bool OrderByGreekName=false)
        {
            try
            {
                int pageSize = 10;
                ViewBag.FoodCategoryID = FoodCategoryID;
                ViewBag.GreekSearch = GreekSearch;
                ViewBag.EnglishSearch = EnglishSearch;
                ViewBag.OrderByGreekName = OrderByGreekName;

                IQueryable<Food> foods = db.Foods.Include(c => c.FoodCategory).OrderBy(r => r.EnglishName );
                if (OrderByGreekName)
                {
                    foods = foods.OrderBy(r => r.GreekName);
                }

                if (FoodCategoryID.HasValue) { foods = foods.Where(r => r.FoodCategoryID == FoodCategoryID.Value); }

                if (!string.IsNullOrEmpty(GreekSearch)) { foods = foods.Where(r => r.GreekName.Contains(GreekSearch)); }
                if (!string.IsNullOrEmpty(EnglishSearch)) { foods = foods.Where(r => r.EnglishName.Contains(EnglishSearch)); }

                if (!User.IsInRole("Administrator"))
                {
                    int companyid = CompanyID();
                    if (User.IsInRole("CanEdit"))
                    {
                        foods = foods.Where(r => r.FoodCategory.CompanyID == null || r.FoodCategory.CompanyID == companyid);
                    }
                    else
                    {
                        foods = foods.Where(r => r.FoodCategory.CompanyID == companyid);
                    }
                }

                int pageNumber = (page ?? 1);
                return View(foods.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public JsonResult UpdateFood(int id, string englishname, string greekname, bool isbreakfast, bool issnack, bool islunch, bool isdinner, bool iscollagene, bool isantioxidant, bool isdetox, bool isdiatrofogenomiki, bool ismenopause)
        {
            string response = "Coundn't save data";
            try
            {
                var food = db.Foods.Find(id);
                if (food == null) { throw new Exception("Food Not Found"); }
                food.EnglishName = englishname;
                food.GreekName = greekname;
                food.IsBreakfast = isbreakfast;
                food.IsSnack = issnack;
                food.IsLunch = islunch;
                food.IsDinner = isdinner;
                food.IsCollagene = iscollagene;
                food.IsAntioxidant = isantioxidant;
                food.IsDetox = isdetox;
                food.IsDiatrofogenomiki = isdiatrofogenomiki;
                food.IsMenopause = ismenopause;
                db.SaveChanges();
                response = "Save was successful";
            }
            catch (Exception ex)
            {
                response = ex.Message;
                Classes.ErrorHandler.LogException(ex, "");
            }
            var data = new { response = response };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteFood(int id)
        {
            string response = "Coundn't delete food";
            try
            {
                var food = db.Foods.Find(id);
                if (food == null) { throw new Exception("Food Not Found"); }
                db.Foods.Remove(food);
                db.SaveChanges();
                response = "Delete was successful";
            }
            catch (Exception ex)
            {
                response = ex.Message;
                Classes.ErrorHandler.LogException(ex, "");
            }
            var data = new { response = response };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Food food = await db.Foods.FindAsync(id);
                if (food == null)
                {
                    return HttpNotFound();
                }
                return View(food);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        public ActionResult Create()
        {
            try
            {
                IQueryable<FoodCategory> FoodCategories = db.FoodCategories;
                if (!User.IsInRole("Administrator"))
                {
                    int companyid = CompanyID();
                    if (User.IsInRole("CanEdit"))
                    {
                        FoodCategories = FoodCategories.Where(r => r.CompanyID == null || r.CompanyID == companyid);
                    }
                    else
                    {
                        FoodCategories = FoodCategories.Where(r => r.CompanyID == companyid);
                    }
                }
                ViewBag.FoodCategoryID = new SelectList(FoodCategories.AsEnumerable(), "id", "GreekName");
                Food newfood = new Food { IsBreakfast = true, IsSnack = true, IsLunch = true, IsDinner = true };
                return View(newfood);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,FoodCategoryID,EnglishName,GreekName,IsBreakfast,IsSnack,IsLunch,IsDinner,Energy,Protein,Carbohydrates,Calcium,Water,Lipid_Tot,Ash,Fiber_TD,SugarTot,Iron,Magnesium,Phosphorus,Potassium,Sodium,Zinc,Copper,Manganese,Selenium,Vitamin_C,Thiamin,Riboflavin,Niacin,Pantothenic,Vitamin_B_6,Folate_Tot,Folic_acid,Folate_food,Folate_DFE,Choline,Vitamin_B_12,Vitamin_A_IU,Vitamin_A_RAE,Retinol,Carotene_alpha,Carotene_beta,Cryptoxanthin_beta,Lycopene,Lutein_zeaxanthin,Vitamin_E,Vitamin_D,Vitamin_D_IU,Vitamin_K,Fat_Sat,Fat_Mono,Fat_Poly,Cholesterol")] Food food)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Foods.Add(food);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                IQueryable<FoodCategory> FoodCategories = db.FoodCategories;
                if (!User.IsInRole("Administrator"))
                {
                    int companyid = CompanyID();
                    if (User.IsInRole("CanEdit"))
                    {
                        FoodCategories = FoodCategories.Where(r => r.CompanyID == null || r.CompanyID == companyid);
                    }
                    else
                    {
                        FoodCategories = FoodCategories.Where(r => r.CompanyID == companyid);
                    }
                }
                ViewBag.FoodCategoryID = new SelectList(FoodCategories, "id", "GreekName", food.FoodCategoryID);
                return View(food);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
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
                Food food = await db.Foods.FindAsync(id);
                if (food == null)
                {
                    return HttpNotFound();
                }
                IQueryable<FoodCategory> FoodCategories = db.FoodCategories;
                if (!User.IsInRole("Administrator"))
                {
                    int companyid = CompanyID();
                    if (User.IsInRole("CanEdit"))
                    {
                        FoodCategories = FoodCategories.Where(r => r.CompanyID == null || r.CompanyID == companyid);
                    }
                    else
                    {
                        FoodCategories = FoodCategories.Where(r => r.CompanyID == companyid);
                    }
                }
                ViewBag.FoodCategoryID = new SelectList(FoodCategories, "id", "GreekName", food.FoodCategoryID);
                return View(food);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,FoodCategoryID,EnglishName,GreekName,IsBreakfast,IsSnack,IsLunch,IsDinner,Energy,Protein,Carbohydrates,Calcium,Water,Lipid_Tot,Ash,Fiber_TD,SugarTot,Iron,Magnesium,Phosphorus,Potassium,Sodium,Zinc,Copper,Manganese,Selenium,Vitamin_C,Thiamin,Riboflavin,Niacin,Pantothenic,Vitamin_B_6,Folate_Tot,Folic_acid,Folate_food,Folate_DFE,Choline,Vitamin_B_12,Vitamin_A_IU,Vitamin_A_RAE,Retinol,Carotene_alpha,Carotene_beta,Cryptoxanthin_beta,Lycopene,Lutein_zeaxanthin,Vitamin_E,Vitamin_D,Vitamin_D_IU,Vitamin_K,Fat_Sat,Fat_Mono,Fat_Poly,Cholesterol")] Food food)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(food).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                IQueryable<FoodCategory> FoodCategories = db.FoodCategories;
                if (!User.IsInRole("Administrator"))
                {
                    int companyid = CompanyID();
                    if (User.IsInRole("CanEdit"))
                    {
                        FoodCategories = FoodCategories.Where(r => r.CompanyID == null || r.CompanyID == companyid);
                    }
                    else
                    {
                        FoodCategories = FoodCategories.Where(r => r.CompanyID == companyid);
                    }
                }
                ViewBag.FoodCategoryID = new SelectList(FoodCategories, "id", "GreekName", food.FoodCategoryID);
                return View(food);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
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
                Food food = await db.Foods.FindAsync(id);
                if (food == null)
                {
                    return HttpNotFound();
                }
                return View(food);
            }
            catch (Exception ex)
            {
                Classes.ErrorHandler.LogException(ex, "");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                Food food = await db.Foods.FindAsync(id);
                db.Foods.Remove(food);
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
