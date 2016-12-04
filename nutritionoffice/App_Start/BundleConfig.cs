using System.Web;
using System.Web.Optimization;

namespace nutritionoffice
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
#if (!DEBUG)
            bundles.Clear();
            bundles.ResetAll();
#endif

            //"~/Scripts/html2canvas.js", 
            //           "~/Scripts/canvas2image.js",
            //           "~/Scripts/base64.js",
            bundles.UseCdn = true;

            var RobotoCDN = "http://fonts.googleapis.com/css?family=Noto+Serif";
            bundles.Add(new StyleBundle("~/fonts", RobotoCDN));
            //                                

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                       "~/Scripts/jquery-{version}.js",
                       "~/Scripts/sitescripts.js"));

            //bundles.Add(new ScriptBundle("~/bundles/timepicker").Include("~/dateimepicker/"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-1.11.4.js",
                        "~/Scripts/localize/datepicker-el.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                                "~/Content/themes/base/core.css",
                                "~/Content/themes/base/jquery-ui.css",
                                "~/Content/themes/base/resizable.css",
                                "~/Content/themes/base/selectable.css",
                                "~/Content/themes/base/accordion.css",
                                "~/Content/themes/base/autocomplete.css",
                                "~/Content/themes/base/button.css",
                                "~/Content/themes/base/dialog.css",
                                "~/Content/themes/base/slider.css",
                                "~/Content/themes/base/tabs.css",
                                "~/Content/themes/base/datepicker.css",
                                "~/Content/themes/base/progressbar.css",
                                "~/Content/themes/base/theme.css", 
                                "~/Content/bootstrap.css",
                                "~/Content/Site.css"
                               ));

#if (DEBUG)
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif

        }
    }
}
