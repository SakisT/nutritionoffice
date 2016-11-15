using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nutritionoffice.Classes
{
    public static class SharedClass
    {
        public static DateTime Now()
        {
            var greektimezone = TimeZoneInfo.FindSystemTimeZoneById("GTB Standard Time");// TimeZoneInfo.GetSystemTimeZones()[55];
            var greektimeoffset = greektimezone.GetUtcOffset(DateTime.UtcNow);
            return DateTime.UtcNow.Add(greektimeoffset);
        }
    }
}