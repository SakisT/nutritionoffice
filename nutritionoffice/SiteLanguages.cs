using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

namespace nutritionoffice
{
    public class SiteLanguages
    {
        public static List<Languages> AvailableLanguages = new List<Languages>
                    {
                            new Languages{ LangFullName = "Ελληνικά", LangCultureName = "el-GR"},
                            new Languages{ LangFullName = "English", LangCultureName = "en-GB"},
                    };

        public static bool IsLanguageAvailable(string lang)
        {
            return AvailableLanguages.Where(a => a.LangCultureName.Equals(lang)).FirstOrDefault() != null ? true : false;
        }

        public static string GetDefaultLanguage()
        {
            return AvailableLanguages[0].LangCultureName;
        }

        public void SetLanguage(string lang)
        {
            try
            {
                if (!IsLanguageAvailable(lang))
                    lang = GetDefaultLanguage();
                var cultureInfo = new CultureInfo(lang);
                //--------------------------------------------------------------------
                cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
                cultureInfo.NumberFormat.NumberGroupSeparator = "";
                cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";
                cultureInfo.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
                cultureInfo.DateTimeFormat.ShortTimePattern = "HH:mm";
                //--------------------------------------------------------------------
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
                Thread.CurrentThread.CurrentCulture = cultureInfo;// CultureInfo.CreateSpecificCulture(cultureInfo.Name);
                HttpCookie langCookie = new HttpCookie("culture", lang);
                langCookie.Expires = DateTime.Now.AddYears(1);
                HttpContext.Current.Response.Cookies.Add(langCookie);

            }
            catch (Exception ex)
            {

            }
        }
    }

    public class Languages
    {
        public string LangFullName { get; set; }
        public string LangCultureName { get; set; }
    }
}