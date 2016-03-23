using System;

namespace Onliner.Converters
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
