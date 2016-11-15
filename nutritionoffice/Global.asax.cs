using nutritionoffice.notificationschedules;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace nutritionoffice
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            JobScheduler.Start();
        }
        protected void Application_BeginRequest()
        {
            var currentCulture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            currentCulture.NumberFormat.NumberDecimalSeparator = ".";
            currentCulture.NumberFormat.NumberGroupSeparator = "";
            currentCulture.NumberFormat.CurrencyDecimalSeparator = ".";
            currentCulture.DateTimeFormat.ShortDatePattern = "d/M/yyyy";
            currentCulture.DateTimeFormat.ShortTimePattern = "HH:mm";

            Thread.CurrentThread.CurrentCulture = currentCulture;

        }
    }
}
