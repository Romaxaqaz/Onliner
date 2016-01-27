using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onliner_for_windows_10.Converters
{
    public static class ConvertToAjaxTime
    {
        public static double ConvertToUnixTimestamp()
        {
            var time = new DateTime(DateTime.Today.Year,
                                DateTime.Today.Month,
                                DateTime.Today.Day,
                                DateTime.Now.Hour,
                                DateTime.Now.Minute,
                                DateTime.Now.Second,
                                DateTime.Now.Millisecond);

            DateTime OLDtime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = time.ToUniversalTime() - OLDtime;
            return Math.Floor(diff.TotalMilliseconds);
        }
    }
}
