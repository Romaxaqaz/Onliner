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

            var olDtime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var diff = time.ToUniversalTime() - olDtime;
            return Math.Floor(diff.TotalMilliseconds);
        }
    }
}
