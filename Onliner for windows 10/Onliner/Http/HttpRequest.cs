using Newtonsoft.Json;
using Onliner.Converters;
using Onliner.JsonModel.Authorization;
using Onliner.Model.Bestrate;
using Onliner.Model.Catalog;
using Onliner.Model.JsonModel.Message;
using Onliner.Model.JsonModel.Weather;
using Onliner.Model.LocalSetteing;
using Onliner.Model.ProfileModel;
using static Onliner.Setting.SettingParams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.UI.Popups;

namespace Onliner.Http
{
    public class HttpRequest
    {
        private const string InternerNotEnableMessage = "Упс, вы не подключены к интернету :(";

        #region Onliner url API
        private const string UserApiOnliner = "https://user.api.onliner.by/login";
        private const string CommentsApiTech = "https://tech.onliner.by/comments/add?";
        private const string CommentsApiPeople = "https://people.onliner.by/comments/add?";
        private const string CommentsApiAuto = "https://auto.onliner.by/comments/add?";
        private const string CommentsApiHouse = "https://realt.onliner.by/comments/add?";
        private const string UnreadApi = "http://www.onliner.by/sdapi/api/messages/unread/";
        private const string EdipProfile = "https://profile.onliner.by/edit";
        private const string EditPreferencesProfileApi = "https://profile.onliner.by/preferences";
        private const string ChangePassProfile = "https://profile.onliner.by/changepass";
        private const string StatusProfileApi = "https://profile.onliner.by/gapi/user/usersonline/?";
        private const string ViewNews = "https://people.onliner.by/viewcounter/view/";
        private const string QuoteCommentApi = "https://tech.onliner.by/api/quote/";
        #endregion

        private string EditProfileToken = string.Empty;

        private HttpClient httpClient = new HttpClient();
        private CookieContainer CookieSession;
        private HttpResponseMessage response;

        public delegate void WebResponnceResult(string ok);

        #region Properties
        private string JsonRequest = string.Empty;
        private string _resultPostRequest = string.Empty;

        private string ResultResponceToken { get; set; }
        public string ResultGetRequsetString { get; set; }
        public string ResultPostRequest
        {
            get { return _resultPostRequest; }
            set { _resultPostRequest = value; }
        }
        #endregion

        #region Constructor
        public HttpRequest()
        {
            Loadcookie("cookie");
        }
        #endregion

        #region GET request
        /// <summary>
        ///  sample GET requset
        /// </summary>
        public async Task<string> GetRequestOnlinerAsync(string url)
        {
            try
            {
                if (!HasInternet()) throw new WebException();
                HttpClient httpClient = new HttpClient();
                var response = httpClient.SendAsync(new HttpRequestMessage(System.Net.Http.HttpMethod.Get, url)).Result;
                ResultGetRequsetString = await response.Content.ReadAsStringAsync();
            }
            catch (WebException)
            {
                await Message(InternerNotEnableMessage);
                return null;
            }
            return ResultGetRequsetString;
        }

        /// <summary>
        /// Get request json
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<string> GetTypeRequestOnlinerAsync(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Accept = "application/json; charset=utf-8";
            var response = await request.GetResponseAsync();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            StringBuilder output = new StringBuilder();
            output.Append(reader.ReadToEnd());
            return output.ToString();
        }

        /// <summary>
        /// Get request
        /// </summary>
        /// <param name="url"></param>
        public async void GetRequestOnliner(string url)
        {
            try
            {
                if (!HasInternet()) ResultGetRequsetString = null;
                HttpClient httpClient = new HttpClient();
                var response = httpClient.SendAsync(new HttpRequestMessage(System.Net.Http.HttpMethod.Get, url)).Result;
                ResultGetRequsetString = await response.Content.ReadAsStringAsync();
            }
            catch (WebException ex) { await Message(ex.ToString()); }
        }

        /// <summary>
        /// Get byte array request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<byte[]> GetRequestByteOnliner(string url)
        {
            byte[] result = null;
            try
            {
                if (!HasInternet()) return null;
                HttpClient httpClient = new HttpClient();
                result = await httpClient.GetByteArrayAsync(new Uri(url));
                await Task.CompletedTask;
            }
            catch (WebException ex) { await Message(ex.ToString()); }
            return result;
        }

        /// <summary>
        /// Get request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="qStringParam"></param>
        /// <returns></returns>
        public async Task<string> GetRequest(string url, string qStringParam)
        {
            string result = string.Empty;
            string paramName = "name=";
            string urlPath = $"{url}{paramName}{qStringParam}";
            if (CookieSession == null)
            {
                Loadcookie("cookie");
            }
            try
            {
                if (!HasInternet()) throw new WebException();
                HttpClient httpClient = new HttpClient(HandlerCookie());
                var response = httpClient.SendAsync(new HttpRequestMessage(System.Net.Http.HttpMethod.Get, urlPath)).Result;
                result = await response.Content.ReadAsStringAsync();
            }
            catch (WebException) { }
            return result;

        }
        #endregion

        #region POST request
        /// <summary>
        /// POST requset for authorization
        /// </summary>
        public async Task<bool> PostRequestUserApi(string login, string password)
        {
            if (!HasInternet()) throw new WebException();

            CookieSession = new CookieContainer();
            bool resultReques = false;
            //get token for authorizton
            HttpRequestMessage req = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, UserApiOnliner);
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            response = await httpClient.SendAsync(req);
            var responseBodyAsText = await response.Content.ReadAsStringAsync();
            string bufferToken = Regex.Match(responseBodyAsText, @"(?=token)(.*)(?=')").Value;
            string token = bufferToken.Replace("token('", "");

            //create json params
            var json = new Authentication
            {
                Token = token,
                Login = login,
                Password = password,
                Session_id = "null",
                RecaptchaChallengeField = string.Empty,
                RecaptchaResponseField = string.Empty,
            };
            JsonRequest = JsonConvert.SerializeObject(json);

            //send post request
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
                    localSettings.Values[LocalSettingParams.Autorization] = "true";
                    Savecookie("cookie", cookieContainer, pageUri);
                    break;
                case 400:
                    resultReques = false;
                    break;
            }
            return resultReques;
        }

        /// <summary>
        /// POST request to add a comment
        /// </summary>
        public async Task<bool> AddComments(string newsID, string message, string linkNews)
        {
            if (newsID == string.Empty) { throw new Exception(); }
            if (message == string.Empty) { throw new Exception(); }

            if (!HasInternet()) throw new WebException();

            HttpClient httpClient = new HttpClient(HandlerCookie());
            var finalURl = GetApionLinks(linkNews) + ConvertToAjaxTime.ConvertToUnixTimestamp().ToString();
            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, finalURl);
            postRequest.Headers.Add("Accept", "application/json, text/javascript, */*; q=0.01");
            postRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
            postRequest.Headers.Add("Accept-Language", "en-US,en;q=0.8,ru;q=0.6");
            postRequest.Headers.Add("Referer", $"{linkNews}");
            postRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36");
            postRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
            StringBuilder postData = new StringBuilder();
            postData.Append("message=" + WebUtility.UrlEncode(message) + "&");
            postData.Append("postId=" + WebUtility.UrlEncode(newsID));
            postRequest.Content = new System.Net.Http.StringContent(postData.ToString(), UnicodeEncoding.UTF8, "application/x-www-form-urlencoded");
            response = await httpClient.SendAsync(postRequest);
            
            if(response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// POST request for edit profile
        /// </summary>
        public async void EditPreferencesProfile(string pmNotification, string hideOnlineStatus, string showEmail, string birthdayView)
        {
            if (EditProfileToken == string.Empty)
            {
                EditProfileToken = await GetEditProfileToken();
            }

            StringBuilder postData = new StringBuilder();
            postData.Append("token=" + EditProfileToken + "&");
            if (pmNotification == "1")
            {
                postData.Append("pmNotification=" + pmNotification + "&");
            }
            if (hideOnlineStatus == "1")
            {
                postData.Append("hideOnlineStatus=" + hideOnlineStatus + "&");
            }
            if (showEmail == "1")
            {
                postData.Append("showEmail=" + showEmail + "&");
            }
            postData.Append("birthdayView=" + birthdayView);

            await PostRequestFormData(EditPreferencesProfileApi, "profile.onliner.by", "http://profile.onliner.by", postData.ToString());
        }

        /// <summary>
        /// Sample POST request
        /// <param name="url"></param>
        /// <param name="host"></param>
        /// <param name="origin"></param>
        /// <param name="formdata">data for request</param>
        /// </summary>
        public async Task PostRequestFormData(string url, string host, string origin, string formdata)
        {
            try
            {
                if (!HasInternet()) throw new WebException();
                HttpClient httpClient = new HttpClient(HandlerCookie());
                HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, url);
                postRequest.Headers.Add("Accept", "application/json, text/javascript, */*; q=0.01");
                postRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
                postRequest.Headers.Add("Accept-Language", "en-US,en;q=0.8,ru;q=0.6");
                postRequest.Headers.Add("Host", host);
                postRequest.Headers.Add("Origin", origin);
                postRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36");

                postRequest.Content = new System.Net.Http.StringContent(formdata, UnicodeEncoding.UTF8, "application/x-www-form-urlencoded");
                response = httpClient.SendAsync(postRequest).Result;
            }
            catch (WebException ex)
            {
                await Message(ex.ToString());
            }
        }
        #endregion

        #region Message
        /// <summary>
        /// GET request to obtain the number of incoming messages
        /// </summary>
        public async Task<string> MessageUnread()
        {
            string result = string.Empty;
            if (CookieSession == null)
            {
                Loadcookie("cookie");
            }
            try
            {
                if (!HasInternet()) return null;
                HttpClient httpClient = new HttpClient(HandlerCookie());
                var response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, UnreadApi)).Result;
                result = await response.Content.ReadAsStringAsync();
            }
            catch (WebException)
            {
                return null;
            }
            return result;
        }

        /// <summary>
        /// GET request for a list of messages
        /// <param name="f">type message: inbox = 0, outbox = -1, saveimage = 1</param>
        /// <param name="p">1</param>
        /// </summary>
        public async Task<MessageJson> Message(string f, string p)
        {
            MessageJson message = null;
            string url = $"https://profile.onliner.by/messages/load/?f={f}&p={p}&token=" + ConvertToAjaxTime.ConvertToUnixTimestamp().ToString();
            try
            {
                if (!HasInternet()) return message;
                HttpClient httpClient = new HttpClient(HandlerCookie());
                var response = httpClient.SendAsync(new HttpRequestMessage(System.Net.Http.HttpMethod.Get, url)).Result;
                var resultJson = await response.Content.ReadAsStringAsync();

                var regex = new Regex("(\"m\\d+\":)", RegexOptions.Compiled | RegexOptions.Multiline);
                var myString = regex.Replace(resultJson, "").Replace("\"messages\":{", "\"messages\":[").Replace("}}}", "}]}");
                message = (MessageJson)JsonConvert.DeserializeObject<MessageJson>(myString);
            }
            catch (WebException) { }
            return message;
        }
        #endregion

        #region Menu Data (Shop, Weather, Bestrate)

        /// <summary>
        /// Shop counter
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<string> ShopCount(string userId)
        {
            if (!HasInternet()) throw new WebException();
            string url = $"https://cart.api.onliner.by/users/{userId}/positions/summary";
            HttpClient httpClient = new HttpClient();
            var response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)).Result;
            var content = await response.Content.ReadAsStringAsync();
            var result = (Carts)JsonConvert.DeserializeObject<Carts>(content);
            if (result == null) return "0";
            return result.total.ToString();
        }

        /// <summary>
        /// GET request for the exchange rate
        /// <param name="currency">currency type: USD, EUR, RUB</param>
        /// <param name="nbrb">type of bank: nbrb, buy, sold</param>
        /// </summary>
        public async Task<BestrateRespose> Bestrate(string currency = "USD", string type = "nbrb")
        {
            BestrateRespose bestrateResponse = null;
            try
            {
                if (!HasInternet()) return null;
                string url = "http://www.onliner.by/sdapi/kurs/api/bestrate?currency=" + currency + "&type=" + type;
                HttpClient httpClient = new HttpClient();
                var response = httpClient.SendAsync(new HttpRequestMessage(System.Net.Http.HttpMethod.Get, url)).Result;
                var resultJson = await response.Content.ReadAsStringAsync();
                bestrateResponse = JsonConvert.DeserializeObject<BestrateRespose>(resultJson);
            }
            catch (WebException ex) { await Message(ex.ToString()); }
            return bestrateResponse;
        }

        /// <summary>
        /// GET request for weather
        /// <param name="townID">id town</param>
        /// </summary>
        public async Task<WeatherJSon> Weather(string townID = "26850")
        {
            var town = GetParamsSetting(TownWeatherIdKey);
            if (town != null) townID = town.ToString();
            WeatherJSon weather = null;
            try
            {
                if (!HasInternet()) return null;

                string url = $"http://www.onliner.by/sdapi/pogoda/api/forecast/{townID}";
                HttpClient httpClient = new HttpClient();
                var response = httpClient.SendAsync(new HttpRequestMessage(System.Net.Http.HttpMethod.Get, url)).Result;
                var resultJson = await response.Content.ReadAsStringAsync();
                var regex = new Regex(@"(\S(19|20)[0-9]{2}-(0[1-9]|1[012])-(0[1-9]|[12][0-9]|3[01])\S\S)", RegexOptions.Compiled | RegexOptions.Multiline);
                var myString = regex.Replace(resultJson, "").Replace("\"forecast\":{", "\"forecast\":[").Replace("},\"now\":", "],\"now\":");
                weather = (WeatherJSon)JsonConvert.DeserializeObject<WeatherJSon>(myString);
            }
            catch (WebException)
            {
                return null;
            }
            return weather;
        }

        #endregion

        #region Edip profile
        /// <summary>
        /// POST request for storing profile data
        /// </summary>
        public async void SaveEditProfile(List<object> ParamsList, BirthDayDate bday)
        {
            if (!HasInternet()) throw new WebException();
            if (EditProfileToken == string.Empty)
            {
                EditProfileToken = await GetEditProfileToken();
            }
            StringBuilder postData = new StringBuilder();
            postData.Append("token=" + EditProfileToken + "&");
            postData.Append("city=" + WebUtility.UrlEncode(ParamsList[0].ToString()) + "&");
            postData.Append("occupation=" + WebUtility.UrlEncode(ParamsList[1].ToString()) + "&");
            postData.Append("interests=" + ParamsList[2].ToString() + "&");
            postData.Append("user_bday=" + bday.Day + "&");
            postData.Append("user_bmonth=" + bday.Mounth + "&");
            postData.Append("user_byear=" + bday.Year + "&");
            postData.Append("jabber=" + ParamsList[3].ToString() + "&");
            postData.Append("icq=" + ParamsList[4].ToString() + "&");
            postData.Append("skype=" + ParamsList[5].ToString() + "&");
            postData.Append("aim=" + ParamsList[6].ToString() + "&");
            postData.Append("website=" + ParamsList[7].ToString() + "&");
            postData.Append("blog=" + ParamsList[8].ToString() + "&");
            postData.Append("devices=" + ParamsList[9].ToString() + "&");
            postData.Append("signature=" + WebUtility.UrlEncode(ParamsList[10].ToString()));

            await PostRequestFormData(EdipProfile, "profile.onliner.by", "http://profile.onliner.by", postData.ToString());

        }

        /// <summary>
        /// POST request for changing password
        /// </summary>
        public async void Changepass(string oldPass, string repeatPass, string newPass)
        {
            if (!HasInternet()) throw new WebException();
            if (EditProfileToken == string.Empty)
            {
                EditProfileToken = await GetEditProfileToken();
            }
            StringBuilder postData = new StringBuilder();
            postData.Append("token=" + EditProfileToken + "&");
            postData.Append("old_password=" + oldPass + "&");
            postData.Append("password=" + repeatPass + "&");
            postData.Append("password_confirm=" + newPass + "&");

            await PostRequestFormData(ChangePassProfile, "profile.onliner.by", "http://profile.onliner.by", postData.ToString());

        }

        /// <summary>
        /// Get request for a token profile
        /// </summary>
        private async Task<string> GetEditProfileToken()
        {
            string token = string.Empty;
            try
            {
                if (!HasInternet()) throw new WebException();
                HttpRequestMessage req = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, EdipProfile);
                response = await httpClient.SendAsync(req);
                var responseBodyAsText = await response.Content.ReadAsStringAsync();
                string bufferToken = Regex.Match(responseBodyAsText, @"(token:(.*)')").Value;
                token = bufferToken.Replace("token: '", "").Replace("'", "");
            }
            catch (WebException ex) { await Message(ex.ToString()); }
            return token;
        }
        #endregion

        #region Cookie
        /// <summary>
        /// Clear cookie
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="rcookie"></param>
        /// <param name="uri"></param>
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

        /// <summary>
        /// Remove cookie
        /// </summary>
        public async void Remoovecookie()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await localFolder.CreateFileAsync("cookie", CreationCollisionOption.ReplaceExisting);
            CookieSession = null;
            await sampleFile.DeleteAsync();
        }

        /// <summary>
        /// Load cookie for requests
        /// </summary>
        private async void Loadcookie(string filename)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile;
            try
            {
                sampleFile = await localFolder.GetFileAsync(filename);
                if (sampleFile == null) return;
                using (Stream stream = await sampleFile.OpenStreamForReadAsync())
                {
                    CookieSession = SerializeCookie.Deserialize(stream, new Uri("http://www.onliner.by"));
                }
            }
            catch
            {
                await localFolder.CreateFileAsync("cookie", CreationCollisionOption.ReplaceExisting);
            }
            await Task.CompletedTask;
        }
        #endregion

        #region Comments

        /// <summary>
        /// Like request
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="method">POST like, DELETE unlike</param>
        /// <returns></returns>
        public async Task<string> LikeComment(string userID, string url, LikeType likeType)
        {
            HttpMethod httpMethod = likeType == LikeType.Like ? HttpMethod.Post : HttpMethod.Delete;
            string resultLike = string.Empty;
            string UserApi = GenerateApiLikeOnliner(url) + userID + "/like";
            if (!HasInternet()) throw new WebException();
            HttpClientHandler handler = new HttpClientHandler();
            if (CookieSession != null)
            {
                handler.CookieContainer = CookieSession;
            }
            HttpClient httpClient = new HttpClient(handler);
            HttpRequestMessage postRequest = new HttpRequestMessage(httpMethod, UserApi);
            postRequest.Headers.Add("Accept", "application/json; charset=utf-8");
            postRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
            var response = httpClient.SendAsync(postRequest).Result;
            var resultJson = await response.Content.ReadAsStringAsync();
            Regex pattern = new Regex(@"([0-9]+)");
            resultLike = pattern.Match(resultJson).ToString();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return resultLike;
            }
            return resultLike;
        }

        private string GenerateApiLikeOnliner(string url)
        {
            if (url.Contains("tech.onliner.by")) return "https://tech.onliner.by/sdapi/news.api/tech/comments/";
            if (url.Contains("people.onliner.by")) return "https://people.onliner.by/sdapi/news.api/people/comments/";
            if (url.Contains("auto.onliner.by")) return "https://auto.onliner.by/sdapi/news.api/auto/comments/";
            if (url.Contains("realt.onliner.by")) return "https://realt.onliner.by/sdapi/news.api/realt/comments/";
            return string.Empty;
        }

        public async Task<string> QuoteComment(string commentId, string url)
        {
            string quoteUrl = QuoteMessageUrlGenerate(url) + commentId + "?" + ConvertToAjaxTime.ConvertToUnixTimestamp().ToString();

            if (!HasInternet()) throw new WebException();
            HttpClientHandler handler = new HttpClientHandler();
            if (CookieSession != null)
            {
                handler.CookieContainer = CookieSession;
            }
            HttpClient httpClient = new HttpClient(handler);
            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Get, quoteUrl);
            postRequest.Headers.Add("Accept", "application/json, text/javascript, */*; q=0.01");
            postRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
            var response = httpClient.SendAsync(postRequest).Result;
            var resultJson = await response.Content.ReadAsStringAsync();
            return Regex.Unescape(resultJson);
        }

        private string QuoteMessageUrlGenerate(string url)
        {
            if (url.Contains("tech.onliner.by")) return "https://tech.onliner.by/api/quote/";
            if (url.Contains("people.onliner.by")) return "https://people.onliner.by/api/quote/";
            if (url.Contains("auto.onliner.by")) return "https://auto.onliner.by/api/quote/";
            if (url.Contains("realt.onliner.by")) return "https://realt.onliner.by/api/quote/";
            return string.Empty;
        }

        private string GetApionLinks(string link)
        {
            if (link.Contains("tech.onliner.by")) return CommentsApiTech;
            if (link.Contains("people.onliner.by")) return CommentsApiPeople;
            if (link.Contains("auto.onliner.by")) return CommentsApiAuto;
            if (link.Contains("realt.onliner.by")) return CommentsApiHouse;
            return string.Empty;
        }
        #endregion

        public async Task<string> NewsViewAll(string url, string newsIDList)
        {
            string quoteUrl = url + "viewcounter/count/?" + newsIDList + "token=" + ConvertToAjaxTime.ConvertToUnixTimestamp().ToString();

            if (!HasInternet()) throw new WebException();
            HttpClientHandler handler = new HttpClientHandler();
            if (CookieSession != null)
            {
                handler.CookieContainer = CookieSession;
            }
            HttpClient httpClient = new HttpClient(handler);
            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Get, quoteUrl);
            postRequest.Headers.Add("Accept", "application/json, text/javascript, */*; q=0.01");
            postRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
            var response = httpClient.SendAsync(postRequest).Result;
            var resultJson = await response.Content.ReadAsStringAsync();
            return resultJson;
        }

        /// <summary>
        /// Status profile
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task StatusSet(string userId)
        {
            string url = StatusProfileApi + userId;

            StringBuilder postData = new StringBuilder();
            postData.Append("usersIds[]=" + WebUtility.UrlEncode(userId));
            await PostRequestFormData(url + ConvertToAjaxTime.ConvertToUnixTimestamp().ToString(),
                 "profile.onliner.by",
                 "https://profile.onliner.by",
                 postData.ToString());
        }

        /// <summary>
        /// News viewer
        /// </summary>
        /// <param name="newsId"></param>
        /// <returns></returns>
        public async Task ViewNewsSet(string newsId)
        {
            string url = ViewNews;

            StringBuilder postData = new StringBuilder();
            postData.Append("news%5B%5D=" + WebUtility.UrlEncode(newsId));

            HttpClientHandler handler = new HttpClientHandler();
            if (CookieSession != null)
            {
                handler.CookieContainer = CookieSession;
            }
            HttpClient httpClient = new HttpClient(handler);
            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, url);
            postRequest.Headers.Add("Accept", "application/json, text/javascript, */*; q=0.01");
            postRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
            postRequest.Headers.Add("Accept-Language", "en-US,en;q=0.8,ru;q=0.6");
            postRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36");
            postRequest.Content = new System.Net.Http.StringContent(postData.ToString(), UnicodeEncoding.UTF8, "application/x-www-form-urlencoded");
            response = httpClient.SendAsync(postRequest).Result;
            await Task.CompletedTask;

        }

        private HttpClientHandler HandlerCookie()
        {
            HttpClientHandler handler = new HttpClientHandler();
            if (CookieSession == null)
            {
                Loadcookie("cookie");
            }
            else
            {
                handler.CookieContainer = CookieSession;
            }

            return handler;
        }

        public bool HasInternet()
        {
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            return (connectionProfile != null &&
                    connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
        }

        public async Task Message(string message)
        {
            MessageDialog msg = new MessageDialog(message);
            await msg.ShowAsync();
        }

    }

    public enum LikeType
    {
        Like, UnLike
    }
}
