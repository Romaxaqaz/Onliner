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

        private string _editProfileToken = string.Empty;

        private readonly HttpClient _httpClient = new HttpClient();
        private CookieContainer _cookieSession;
        private HttpResponseMessage _response;

        public delegate void WebResponnceResult(string ok);

        #region Properties
        private string _jsonRequest = string.Empty;

        public string ResultGetRequsetString { get; set; }
        public string ResultPostRequest { get; set; } = string.Empty;

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
                var httpClient = new HttpClient();
                var response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)).Result;
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
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Accept = "application/json; charset=utf-8";
            var response = await request.GetResponseAsync();
            var reader = new StreamReader(response.GetResponseStream());
            var output = new StringBuilder();
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
                var httpClient = new HttpClient();
                var response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)).Result;
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
                var httpClient = new HttpClient();
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
            var result = string.Empty;
            var paramName = "name=";
            var urlPath = NewMethod(url, qStringParam, paramName);
            if (_cookieSession == null)
            {
                Loadcookie("cookie");
            }
            try
            {
                if (!HasInternet()) throw new WebException();
                var httpClient = new HttpClient(HandlerCookie());
                var response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, urlPath)).Result;
                result = await response.Content.ReadAsStringAsync();
            }
            catch (WebException) { }
            return result;

        }

        private static string NewMethod(string url, string qStringParam, string paramName)
        {
            return $"{url}{paramName}{qStringParam}";
        }
        #endregion

        #region POST request
        /// <summary>
        /// POST requset for authorization
        /// </summary>
        public async Task<bool> PostRequestUserApi(string login, string password)
        {
            if (!HasInternet()) throw new WebException();

            _cookieSession = new CookieContainer();
            var resultReques = false;
            //get token for authorizton
            var req = new HttpRequestMessage(HttpMethod.Get, UserApiOnliner);
            var localSettings = ApplicationData.Current.LocalSettings;
            _response = await _httpClient.SendAsync(req);
            var responseBodyAsText = await _response.Content.ReadAsStringAsync();
            var bufferToken = Regex.Match(responseBodyAsText, @"(?=token)(.*)(?=')").Value;
            var token = bufferToken.Replace("token('", "");

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
            _jsonRequest = JsonConvert.SerializeObject(json);

            //send post request
            req = new HttpRequestMessage(HttpMethod.Post, UserApiOnliner);
            req.Headers.Add("Accept", "application/json, text/javascript, */*; q=0.01");
            req.Method = HttpMethod.Post;
            req.Content = new StringContent(_jsonRequest);
            req.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            _response = await _httpClient.SendAsync(req);
            var responseText = await _response.Content.ReadAsStringAsync();
            var pageUri = _response.RequestMessage.RequestUri;

            var cookieContainer = new CookieContainer();
            IEnumerable<string> cookies;

            if (_response.Headers.TryGetValues("set-cookie", out cookies))
            {
                foreach (var c in cookies)
                {
                    cookieContainer.SetCookies(pageUri, c);
                }
            }

            ResultPostRequest = responseText;

            switch ((int)_response.StatusCode)
            {
                case 200:
                    resultReques = true;
                    localSettings.Values[LocalSettingParams.Autorization] = "true";
                    Savecookie("cookie", cookieContainer, pageUri);
                    break;
                case 400:
                    break;
            }
            return resultReques;
        }

        /// <summary>
        /// POST request to add a comment
        /// </summary>
        public async Task<bool> AddComments(string newsId, string message, string linkNews)
        {
            if (newsId == string.Empty) { throw new Exception(); }
            if (message == string.Empty) { throw new Exception(); }

            if (!HasInternet()) throw new WebException();

            var httpClient = new HttpClient(HandlerCookie());
            var finalURl = GetApionLinks(linkNews) + ConvertToAjaxTime.ConvertToUnixTimestamp();
            var postRequest = new HttpRequestMessage(HttpMethod.Post, finalURl);
            postRequest.Headers.Add("Accept", "application/json, text/javascript, */*; q=0.01");
            postRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
            postRequest.Headers.Add("Accept-Language", "en-US,en;q=0.8,ru;q=0.6");
            postRequest.Headers.Add("Referer", $"{linkNews}");
            postRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36");
            postRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
            var postData = new StringBuilder();
            postData.Append("message=" + WebUtility.UrlEncode(message) + "&");
            postData.Append("postId=" + WebUtility.UrlEncode(newsId));
            postRequest.Content = new StringContent(postData.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");
            _response = await httpClient.SendAsync(postRequest);
            
            return _response.StatusCode == HttpStatusCode.OK;
        }

        /// <summary>
        /// POST request for edit profile
        /// </summary>
        public async void EditPreferencesProfile(string pmNotification, string hideOnlineStatus, string showEmail, string birthdayView)
        {
            if (_editProfileToken == string.Empty)
            {
                _editProfileToken = await GetEditProfileToken();
            }

            var postData = new StringBuilder();
            postData.Append("token=" + _editProfileToken + "&");
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
                var httpClient = new HttpClient(HandlerCookie());
                var postRequest = new HttpRequestMessage(HttpMethod.Post, url);
                postRequest.Headers.Add("Accept", "application/json, text/javascript, */*; q=0.01");
                postRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
                postRequest.Headers.Add("Accept-Language", "en-US,en;q=0.8,ru;q=0.6");
                postRequest.Headers.Add("Host", host);
                postRequest.Headers.Add("Origin", origin);
                postRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36");

                postRequest.Content = new StringContent(formdata, Encoding.UTF8, "application/x-www-form-urlencoded");
                _response = httpClient.SendAsync(postRequest).Result;
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
            string result;
            if (_cookieSession == null)
            {
                Loadcookie("cookie");
            }
            try
            {
                if (!HasInternet()) return null;
                var httpClient = new HttpClient(HandlerCookie());
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
            var url = $"https://profile.onliner.by/messages/load/?f={f}&p={p}&token=" + ConvertToAjaxTime.ConvertToUnixTimestamp();
            try
            {
                if (!HasInternet()) return null;
                var httpClient = new HttpClient(HandlerCookie());
                var response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)).Result;
                var resultJson = await response.Content.ReadAsStringAsync();

                var regex = new Regex("(\"m\\d+\":)", RegexOptions.Compiled | RegexOptions.Multiline);
                var myString = regex.Replace(resultJson, "").Replace("\"messages\":{", "\"messages\":[").Replace("}}}", "}]}");
                message = JsonConvert.DeserializeObject<MessageJson>(myString);
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
            var httpClient = new HttpClient();
            var response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)).Result;
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Carts>(content);
            return result == null ? "0" : result.total.ToString();
        }

        /// <summary>
        /// GET request for the exchange rate
        /// <param name="currency">currency type: USD, EUR, RUB</param>
        /// <param name="type">type of bank: nbrb, buy, sold</param>
        /// </summary>
        public async Task<BestrateRespose> Bestrate(string currency = "USD", string type = "nbrb")
        {
            BestrateRespose bestrateResponse = null;
            try
            {
                if (!HasInternet()) return null;
                var url = "http://www.onliner.by/sdapi/kurs/api/bestrate?currency=" + currency + "&type=" + type;
                var httpClient = new HttpClient();
                var response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)).Result;
                var resultJson = await response.Content.ReadAsStringAsync();
                bestrateResponse = JsonConvert.DeserializeObject<BestrateRespose>(resultJson);
            }
            catch (WebException ex) { await Message(ex.ToString()); }
            return bestrateResponse;
        }

        /// <summary>
        /// GET request for weather
        /// <param name="townId">id town</param>
        /// </summary>
        public async Task<WeatherJSon> Weather(string townId = "26850")
        {
            var town = GetParamsSetting(TownWeatherIdKey);
            if (town != null) townId = town.ToString();
            WeatherJSon weather;
            try
            {
                if (!HasInternet()) return null;

                string url = $"http://www.onliner.by/sdapi/pogoda/api/forecast/{townId}";
                var httpClient = new HttpClient();
                var response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)).Result;
                var resultJson = await response.Content.ReadAsStringAsync();
                var regex = new Regex(@"(\S(19|20)[0-9]{2}-(0[1-9]|1[012])-(0[1-9]|[12][0-9]|3[01])\S\S)", RegexOptions.Compiled | RegexOptions.Multiline);
                var myString = regex.Replace(resultJson, "").Replace("\"forecast\":{", "\"forecast\":[").Replace("},\"now\":", "],\"now\":");
                weather = JsonConvert.DeserializeObject<WeatherJSon>(myString);
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
        public async void SaveEditProfile(List<object> paramsList, BirthDayDate bday)
        {
            if (!HasInternet()) throw new WebException();
            if (_editProfileToken == string.Empty)
            {
                _editProfileToken = await GetEditProfileToken();
            }
            var postData = new StringBuilder();
            postData.Append("token=" + _editProfileToken + "&");
            postData.Append("city=" + WebUtility.UrlEncode(paramsList[0].ToString()) + "&");
            postData.Append("occupation=" + WebUtility.UrlEncode(paramsList[1].ToString()) + "&");
            postData.Append("interests=" + paramsList[2] + "&");
            postData.Append("user_bday=" + bday.Day + "&");
            postData.Append("user_bmonth=" + bday.Mounth + "&");
            postData.Append("user_byear=" + bday.Year + "&");
            postData.Append("jabber=" + paramsList[3] + "&");
            postData.Append("icq=" + paramsList[4] + "&");
            postData.Append("skype=" + paramsList[5] + "&");
            postData.Append("aim=" + paramsList[6] + "&");
            postData.Append("website=" + paramsList[7] + "&");
            postData.Append("blog=" + paramsList[8] + "&");
            postData.Append("devices=" + paramsList[9] + "&");
            postData.Append("signature=" + WebUtility.UrlEncode(paramsList[10].ToString()));

            await PostRequestFormData(EdipProfile, "profile.onliner.by", "http://profile.onliner.by", postData.ToString());

        }

        /// <summary>
        /// POST request for changing password
        /// </summary>
        public async void Changepass(string oldPass, string repeatPass, string newPass)
        {
            if (!HasInternet()) throw new WebException();
            if (_editProfileToken == string.Empty)
            {
                _editProfileToken = await GetEditProfileToken();
            }
            var postData = new StringBuilder();
            postData.Append("token=" + _editProfileToken + "&");
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
            var token = string.Empty;
            try
            {
                if (!HasInternet()) throw new WebException();
                var req = new HttpRequestMessage(HttpMethod.Get, EdipProfile);
                _response = await _httpClient.SendAsync(req);
                var responseBodyAsText = await _response.Content.ReadAsStringAsync();
                var bufferToken = Regex.Match(responseBodyAsText, @"(token:(.*)')").Value;
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
            var localFolder = ApplicationData.Current.LocalFolder;
            var sampleFile = await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            using (var transaction = await sampleFile.OpenTransactedWriteAsync())
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
            var localFolder = ApplicationData.Current.LocalFolder;
            var sampleFile = await localFolder.CreateFileAsync("cookie", CreationCollisionOption.ReplaceExisting);
            _cookieSession = null;
            await sampleFile.DeleteAsync();
        }

        /// <summary>
        /// Load cookie for requests
        /// </summary>
        private async void Loadcookie(string filename)
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            try
            {
                var sampleFile = await localFolder.GetFileAsync(filename);
                if (sampleFile == null) return;
                using (var stream = await sampleFile.OpenStreamForReadAsync())
                {
                    _cookieSession = SerializeCookie.Deserialize(stream, new Uri("http://www.onliner.by"));
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
        /// <param name="userId"></param>
        /// <param name="url"></param>
        /// <param name="likeType"></param>
        /// <returns></returns>
        public async Task<string> LikeComment(string userId, string url, LikeType likeType)
        {
            var httpMethod = likeType == LikeType.Like ? HttpMethod.Post : HttpMethod.Delete;
            var userApi = GenerateApiLikeOnliner(url) + userId + "/like";
            if (!HasInternet()) throw new WebException();
            var handler = new HttpClientHandler();
            if (_cookieSession != null)
            {
                handler.CookieContainer = _cookieSession;
            }
            var httpClient = new HttpClient(handler);
            var postRequest = new HttpRequestMessage(httpMethod, userApi);
            postRequest.Headers.Add("Accept", "application/json; charset=utf-8");
            postRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
            var response = httpClient.SendAsync(postRequest).Result;
            var resultJson = await response.Content.ReadAsStringAsync();
            var pattern = new Regex(@"([0-9]+)");
            return pattern.Match(resultJson).ToString();
        }

        private static string GenerateApiLikeOnliner(string url)
        {
            if (url.Contains("tech.onliner.by")) return "https://tech.onliner.by/sdapi/news.api/tech/comments/";
            if (url.Contains("people.onliner.by")) return "https://people.onliner.by/sdapi/news.api/people/comments/";
            if (url.Contains("auto.onliner.by")) return "https://auto.onliner.by/sdapi/news.api/auto/comments/";
            if (url.Contains("realt.onliner.by")) return "https://realt.onliner.by/sdapi/news.api/realt/comments/";
            return string.Empty;
        }

        public async Task<string> QuoteComment(string commentId, string url)
        {
            var quoteUrl = QuoteMessageUrlGenerate(url) + commentId + "?" + ConvertToAjaxTime.ConvertToUnixTimestamp();

            if (!HasInternet()) throw new WebException();
            var handler = new HttpClientHandler();
            if (_cookieSession != null)
            {
                handler.CookieContainer = _cookieSession;
            }
            var httpClient = new HttpClient(handler);
            var postRequest = new HttpRequestMessage(HttpMethod.Get, quoteUrl);
            postRequest.Headers.Add("Accept", "application/json, text/javascript, */*; q=0.01");
            postRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
            var response = httpClient.SendAsync(postRequest).Result;
            var resultJson = await response.Content.ReadAsStringAsync();
            return Regex.Unescape(resultJson);
        }

        private static string QuoteMessageUrlGenerate(string url)
        {
            if (url.Contains("tech.onliner.by")) return "https://tech.onliner.by/api/quote/";
            if (url.Contains("people.onliner.by")) return "https://people.onliner.by/api/quote/";
            if (url.Contains("auto.onliner.by")) return "https://auto.onliner.by/api/quote/";
            if (url.Contains("realt.onliner.by")) return "https://realt.onliner.by/api/quote/";
            return string.Empty;
        }

        private static string GetApionLinks(string link)
        {
            if (link.Contains("tech.onliner.by")) return CommentsApiTech;
            if (link.Contains("people.onliner.by")) return CommentsApiPeople;
            if (link.Contains("auto.onliner.by")) return CommentsApiAuto;
            if (link.Contains("realt.onliner.by")) return CommentsApiHouse;
            return string.Empty;
        }
        #endregion

        public async Task<string> NewsViewAll(string url, string newsIdList)
        {
            var quoteUrl = url + "viewcounter/count/?" + newsIdList + "token=" + ConvertToAjaxTime.ConvertToUnixTimestamp();

            if (!HasInternet()) throw new WebException();
            var handler = new HttpClientHandler();
            if (_cookieSession != null)
            {
                handler.CookieContainer = _cookieSession;
            }
            var httpClient = new HttpClient(handler);
            var postRequest = new HttpRequestMessage(HttpMethod.Get, quoteUrl);
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
            var url = StatusProfileApi + userId;

            var postData = new StringBuilder();
            postData.Append("usersIds[]=" + WebUtility.UrlEncode(userId));
            await PostRequestFormData(url + ConvertToAjaxTime.ConvertToUnixTimestamp(),
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
            var url = ViewNews;

            var postData = new StringBuilder();
            postData.Append("news%5B%5D=" + WebUtility.UrlEncode(newsId));

            var handler = new HttpClientHandler();
            if (_cookieSession != null)
            {
                handler.CookieContainer = _cookieSession;
            }
            var httpClient = new HttpClient(handler);
            var postRequest = new HttpRequestMessage(HttpMethod.Post, url);
            postRequest.Headers.Add("Accept", "application/json, text/javascript, */*; q=0.01");
            postRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
            postRequest.Headers.Add("Accept-Language", "en-US,en;q=0.8,ru;q=0.6");
            postRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36");
            postRequest.Content = new StringContent(postData.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");
            _response = httpClient.SendAsync(postRequest).Result;
            await Task.CompletedTask;

        }

        private HttpClientHandler HandlerCookie()
        {
            var handler = new HttpClientHandler();
            if (_cookieSession == null)
            {
                Loadcookie("cookie");
            }
            else
            {
                handler.CookieContainer = _cookieSession;
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
            var msg = new MessageDialog(message);
            await msg.ShowAsync();
        }

    }

    public enum LikeType
    {
        Like, UnLike
    }
}
