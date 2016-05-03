using Newtonsoft.Json;
using Onliner.Model.JsonModel.Authorization;

namespace Onliner.JsonModel.Authorization
{
    public class Authentication : IAuthentication
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("login")]
        public string Login { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("session_id")]
        public string Session_id { get; set; }
        [JsonProperty("recaptcha_challenge_field")]
        public string RecaptchaChallengeField { get; set; }
        [JsonProperty("recaptcha_response_field")]
        public string RecaptchaResponseField { get; set; }
    }
}
