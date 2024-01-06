using RIAPP.DataService.Core.Types;
using System;
using System.Globalization;

namespace RIAPP.DataService.Utils
{
    public static class DateTimeHelper
    {
        public static int GetTimezoneOffset()
        {
            DateTime uval = DATEZERO.ToUniversalTime();
            TimeSpan tspn = uval - DATEZERO;
            return (int)tspn.TotalMinutes;
        }

        public static DateTime ParseDateTime(string val, DateConversion dateConversion)
        {
            DateTime dt = DateTime.ParseExact(val, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            if (dateConversion == DateConversion.UtcToClientLocal)
            {
                return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            }
            else
            {
                DateTime d = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                return d.ToLocalTime();
            }
        }

        public static string DateToString(DateTime dt, DateConversion dateConversion)
        { 
            if (dateConversion == DateConversion.UtcToClientLocal)
            {
                DateTime d = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                return d.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
            }
            else
            {
                DateTime d = DateTime.SpecifyKind(dt, DateTimeKind.Local);
                return d.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
            }
        }

        public static string TimeToString(TimeSpan time, DateConversion dateConversion)
        {
            return DateToString(DateTimeHelper.DATEZERO + time, dateConversion);
        }

        public static string DateOffsetToString(DateTimeOffset dtoff, DateConversion dateConversion)
        {
            return DateToString(dtoff.DateTime, dateConversion);
        }


        public static readonly DateTime DATEZERO = new DateTime(1900, 1, 1);
    }
}