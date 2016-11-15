using nutritionoffice.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using nutritionoffice.Classes;


namespace nutritionoffice
{
    public class MyBaseController : Controller
    {
        private ndbContext db = new ndbContext();
        public int CompanyID()
        {

            if (User.Identity.IsAuthenticated && !User.IsInRole("InActive"))
            {
                if (!(User.IsInRole("Administrator") && Session["CompanyID"] != null && (string)Session["CompanyID"] != "0"))
                {
                    var identity = (ClaimsIdentity)User.Identity;
                    IEnumerable<Claim> claims = identity.Claims;
                    try
                    {
                        var help = claims.First(r => r.Type == "CompanyID").Value;
                        if (help != null)
                        {
                            if (Session["CompanyID"] == null || (string)Session["CompanyID"] == "0") { Session["CompanyID"] = help; }
                        }
                        else
                        {
                            Session["CompanyID"] = "0";
                        }
                    }
                    catch (Exception ex)
                    {
                        Session["CompanyID"] = "0";
                    }
                }
            }
            else
            {
                Session["CompanyID"] = "0";
            }
            return Convert.ToInt32(Session["CompanyID"]);
        }

        // Here I have created this for execute each time any controller (inherit this) load 
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string lang = null;
            HttpCookie langCookie = Request.Cookies["culture"];
            if (langCookie != null)
            {
                lang = langCookie.Value;
            }
            else
            {
                var userLanguage = Request.UserLanguages;
                var userLang = userLanguage != null ? userLanguage[0] : "";
                if (userLang != "")
                {
                    lang = userLang;
                }
                else
                {
                    lang = SiteLanguages.GetDefaultLanguage();
                }
            }

            new SiteLanguages().SetLanguage(lang);

            return base.BeginExecuteCore(callback, state);
        }
    }
}