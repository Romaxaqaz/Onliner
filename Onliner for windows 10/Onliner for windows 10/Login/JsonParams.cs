using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onliner_for_windows_10.Login
{
    public class JsonParams
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
            public string Recaptcha_challenge_field { get; set; }
            [JsonProperty("recaptcha_response_field")]
            public string Recaptcha_response_field { get; set; }
        
    }
}
