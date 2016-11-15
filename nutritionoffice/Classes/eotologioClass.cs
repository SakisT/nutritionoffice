using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
namespace nutritionoffice.Classes
{
    public class eotologioClass
    {
        private static DateTime LastDataDate = DateTime.MinValue;
        private static List<NameDay> _NameDays = new List<NameDay>();
        public static List<NameDay> NameDays()
        {
            DateTime CheckDate = DateTime.UtcNow.AddHours(3);

            if (LastDataDate < CheckDate.Date)
            {
                try
                {
                    var file = "http://www.eortologio.gr/rss/si_av_me_el.xml";
                    XDocument doc = XDocument.Load(new Uri(file).AbsoluteUri);
                    IEnumerable<XElement> NameDates = doc.Descendants("item");
                    _NameDays = new List<NameDay>();
                    foreach (var item in NameDates)
                    {
                        string datestring = item.Element("title").Value;
                        datestring = datestring.Split(new[] { ':' }).FirstOrDefault().Trim();
                        datestring = datestring.Replace("μεθαύριο", "").Replace("σήμερα", "").Replace("αύριο", "").Trim();
                        DateTime date = DateTime.Parse(datestring, new System.Globalization.CultureInfo("el-GR"));
                        string namesline = item.Element("title").Value.ToString().Replace("(πηγή : www.eortologio.gr)", "");
                        string[] HeadNames = namesline.Split(new[] { ':' }).Skip(1).FirstOrDefault().Split(new[] { ',' }).Select(r => r.Trim()).ToArray();
                        _NameDays.Add(new NameDay { Date = date.Date, HeadNames = HeadNames, Names = GetAllCashings(HeadNames), });
                    }
                    LastDataDate = DateTime.Today;
                }
                catch(Exception ex)
                {
                    Console.WriteLine();
                }

            }
            return _NameDays;
        }

        private static string[] GetAllCashings(string[] Names)
        {
            List<string> newnames = new List<string>();
            foreach (string name in Names)
            {
                newnames.Add(name);
                string removetones = name.Replace("ά", "α").
                    Replace("έ", "ε").
                    Replace("ή", "η").
                    Replace("ί", "ι").
                    Replace("ό", "ο").
                    Replace("ύ", "υ").
                    Replace("ώ", "ω").
                    Replace("ϊ", "ι").
                    Replace("ΐ", "ι").
                    Replace("ΰ", "υ");
                newnames.Add(removetones);
                newnames.Add(removetones.Replace("ς", "σ").ToUpper());
            }
            return newnames.Distinct().ToArray();
        }
        public class NameDay
        {
            public DateTime Date { get; set; }

            public string[] HeadNames { get; set; }

            public string[] Names { get; set; }


        }
    }
}