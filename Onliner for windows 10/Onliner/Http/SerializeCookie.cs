using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Onliner.Http
{
    public static class SerializeCookie
    {
        /// <summary>
        /// Serialize cookie after aothorization
        /// </summary>
        /// <param name="cookies"></param>
        /// <param name="address"></param>
        /// <param name="stream"></param>
        public static void Serialize(CookieCollection cookies, Uri address, Stream stream)
        {
            var formatter = new DataContractSerializer(typeof(List<Cookie>));
            var cookieList = new List<Cookie>();
            for (var enumerator = cookies.GetEnumerator(); enumerator.MoveNext();)
            {
                var cookie = enumerator.Current as Cookie;
                if (cookie == null) continue;
                cookieList.Add(cookie);

            }
            formatter.WriteObject(stream, cookieList);
        }

        /// <summary>
        /// Deserialize cookie after aothorization
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static CookieContainer Deserialize(Stream stream, Uri uri)
        {
            var container = new CookieContainer();
            var formatter = new DataContractSerializer(typeof(List<Cookie>));
            var cookies = (List<Cookie>)formatter.ReadObject(stream);
            var cookieco = new CookieCollection();

            foreach (var cookie in cookies)
            {
                cookieco.Add(cookie);
            }
            container.Add(uri, cookieco);
            return container;
        }
    }
}
