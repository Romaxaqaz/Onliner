using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Onliner_for_windows_10.Login
{
    public class Request
    {
        private const string UserApiOnliner = "https://user.api.onliner.by/login";
        private string login = "";
        private string password = "";

        private string _resultPostRequest = "";
        CookieContainer saveCookie;
        private string ResultResponceToken { get; set; }
        private string JsonRequest = "";
        public string ResultPostRequest
        { get { return _resultPostRequest; } }




        private HttpClient httpClient = new HttpClient();
        HttpResponseMessage response;

        public Request() { }



        public string GetRequestOnliner(string url)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = saveCookie;
            HttpClient httpClient = new HttpClient(handler);
            var response = httpClient.SendAsync(new HttpRequestMessage(System.Net.Http.HttpMethod.Get, url)).Result;
            return response.Content.ReadAsStringAsync().ToString();
        }

        public async void PostRequestUserApi(string login, string password)
        {
            HttpRequestMessage req = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, UserApiOnliner);
            response = await httpClient.SendAsync(req);
            var responseBodyAsText = await response.Content.ReadAsStringAsync();
            string bufferToken = Regex.Match(responseBodyAsText, @"(?=token)(.*)(?=')").Value;
            string token = bufferToken.Replace("token('", "");
            ResultResponceToken = token;
            var json = new JsonParams
            {
                Token = ResultResponceToken,
                Login = login,
                Password = password,
                Session_id = "null",
                Recaptcha_challenge_field = "",
                Recaptcha_response_field = "",

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
            saveCookie = cookieContainer;
            _resultPostRequest = responseText.ToString();
        }
    }
}
