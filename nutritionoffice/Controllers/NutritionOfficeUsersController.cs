using nutritionoffice.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace nutritionoffice.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class NutritionOfficeUsersController : MyBaseController
    {
        private nutritionoffice.Models.ApplicationDbContext db = new Models.ApplicationDbContext();
        public ActionResult Index()
        {
            //var roles=(from p in db.Roles select new{ id=p.Id, Name=p.Name }).ToList();
            var userroles = (from p in db.Roles select new { id = p.Id, Name = p.Name, Users = p.Users.Select(r => r.UserId).ToList() }).ToList();
            List<partialcompanies> companies = null;

            using (Models.ndbContext edb = new Models.ndbContext())
            {
                companies = (from p in edb.Companies select new partialcompanies { id = p.id, Name = p.CompanyName }).ToList();
            }
            ViewBag.Companies = companies;
            ViewBag.Roles = userroles;
            var currentusers = from p in db.Users.ToList()
                               let claim = p.Claims.Where(r => r.ClaimType == "CompanyID").FirstOrDefault()
                               select new partialusers
                               {
                                   UserID = p.Id,
                                   UserName = p.UserName,
                                   UserEmail = p.Email,
                                   UserPassword = p.PasswordHash,
                                   CurrentRoles = (from r in userroles where r.Users.Any(k => k == p.Id) select r.id).ToArray(),
                                   CompanyID = (claim == null) ? "" : claim.ClaimValue
                               };
            return View(currentusers.OrderBy(r => r.UserName));
        }
        public JsonResult UpdateUser(string userid, string[] userrole, string usercompany, string userpassword)
        {
            string response = "Coundn't save data";
            string PasswordSaveMessage = "";
            try
            {
                var passwordhash = new Microsoft.AspNet.Identity.PasswordHasher();
                var currentuser = db.Users.Where(r => r.Id == userid).FirstOrDefault();
                if (!string.IsNullOrEmpty(userpassword))
                {
                    var regex = new System.Text.RegularExpressions.Regex(@"^.*(?=.*[!@#$%^&*\(\)_\-+=]).*$");
                    if (regex.Match(userpassword).Success)
                    {
                        currentuser.PasswordHash = passwordhash.HashPassword(userpassword);
                        currentuser.SecurityStamp = Guid.NewGuid().ToString();
                        PasswordSaveMessage = "Επιτυχής αλλαγή Password";
                    }
                    else
                    {
                        PasswordSaveMessage = "Το Password δεν ένημερώθηκε γιατί:\r\n" +
                          "πρέπει να αποτελείτε από 8 τουλάχιστο χαρακτήρες\r\n" +
                          "και να περίεχει τουλάχιστο ένα γράμμα κεφαλαίο, \r\n" +
                          "ένα γράμμα μικρό,\r\n" +
                          "έναν αριθμό και\r\n" +
                          "έναν ειδικό χαρακτήρα (!@$*)";
                    }
                }
                currentuser.Roles.Clear();
                Array.ForEach(userrole,r=>currentuser.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole { RoleId = r }));
                if (!string.IsNullOrEmpty(usercompany))
                {
                    var claim = currentuser.Claims.Where(r => r.ClaimType == "CompanyID").FirstOrDefault();
                    if (claim != null)
                    {
                        claim.ClaimValue = usercompany;
                    }
                    else
                    {
                        currentuser.Claims.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim { ClaimType = "CompanyID", ClaimValue = usercompany });
                    }
                }

                db.Entry(currentuser).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                response = "Επιτυχής αποθήκευση";
            }
            catch (Exception ex)
            {
                PasswordSaveMessage = "";
            }
            var data = new { response = string.Join("\r\n", new string[] { response, PasswordSaveMessage }) };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {
            var user = db.Users.Find(id);

            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}