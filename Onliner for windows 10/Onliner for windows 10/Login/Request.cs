using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;

namespace Onliner_for_windows_10.Login
{
    public class Request
    {
        private const string UserApiOnliner = "https://user.api.onliner.by/login";

        private HttpClient httpClient = new HttpClient();
        private CookieContainer CookieSession;
        private HttpResponseMessage response;

        private string JsonRequest = string.Empty;
        private string _resultPostRequest = string.Empty;

        private string ResultResponceToken { get; set; }
        public string ResultGetRequsetString { get; set; }
        public string ResultPostRequest
        {
            get { return _resultPostRequest; }
            set { _resultPostRequest = value; }
        }

        public async void GetRequestOnliner(string url)
        {
            Loadcookie("cookie");
            HttpClientHandler handler = new HttpClientHandler();
            if (CookieSession != null)
            {
                handler.CookieContainer = CookieSession;
            }
            HttpClient httpClient = new HttpClient(handler);
            var response = httpClient.SendAsync(new HttpRequestMessage(System.Net.Http.HttpMethod.Get, url)).Result;
            ResultGetRequsetString = await response.Content.ReadAsStringAsync();
        }

        public async Task<bool> PostRequestUserApi(string login, string password)
        {
            bool resultReques = false;
            HttpRequestMessage req = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, UserApiOnliner);
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            response = await httpClient.SendAsync(req);
            var responseBodyAsText = await response.Content.ReadAsStringAsync();
            string bufferToken = Regex.Match(responseBodyAsText, @"(?=token)(.*)(?=')").Value;
            string token = bufferToken.Replace("token('", "");

            var json = new JsonParams
            {
                Token = token,
                Login = login,
                Password = password,
                Session_id = "null",
                Recaptcha_challenge_field = string.Empty,
                Recaptcha_response_field = string.Empty,
            };

            JsonRequest = JsonConvert.SerializeObject(json);

            req = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, UserApiOnliner);
            req.Headers.Add("Accept", "application/json, text/javascript, */*; q=0.01");
            req.Method = System.Net.Http.HttpMethod.Post;
            req.Content = new System.Net.Http.StringContent(JsonRequest);
            req.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            response = await httpClient.SendAsync(req);
            var responseText = await response.Content.ReadAsStringAsync();
            var pageUri = response.RequestMessage.RequestUri;

            var cookieContainer = new CookieContainer();
            IEnumerable<string> cookies;

            if (response.Headers.TryGetValues("set-cookie", out cookies))
            {
                foreach (var c in cookies)
                {
                    cookieContainer.SetCookies(pageUri, c);
                }
            }
            ResultPostRequest = responseText.ToString();

            switch ((int)response.StatusCode)
            {
                case 200:
                    resultReques = true;
                    localSettings.Values["Autorization"] = "yes";
                    Savecookie("cookie", cookieContainer, pageUri);
                    break;
                case 400:
                    resultReques = false;
                    break;
            }
            return resultReques;
        }

        private async void Savecookie(string filename, CookieContainer rcookie, Uri uri)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            using (StorageStreamTransaction transaction = await sampleFile.OpenTransactedWriteAsync())
            {
                SerializeCookie.Serialize(rcookie.GetCookies(uri), uri, transaction.Stream.AsStream());
                await transaction.CommitAsync();
            }
        }

        private async void Loadcookie(string filename)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile;
            try
            {
                sampleFile = await localFolder.GetFileAsync(filename);
                using (Stream stream = await sampleFile.OpenStreamForReadAsync())
                {
                    CookieSession = SerializeCookie.Deserialize(stream, new Uri("http://www.onliner.by"));
                }
            }
            catch
            {
                return;
            }
        }
    }
}
