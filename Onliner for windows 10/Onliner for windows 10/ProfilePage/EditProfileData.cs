using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onliner_for_windows_10.ProfilePage
{
    public class EditProfileData
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("occupation")]
        public string Occupation { get; set; }
        [JsonProperty("interests")]
        public string Interests { get; set; }
        [JsonProperty("user_bday")]
        public string User_bday { get; set; }
        [JsonProperty("user_bmonth")]
        public string User_bmonth { get; set; }
        [JsonProperty("user_byear")]
        public string User_byear { get; set; }
        [JsonProperty("jabber")]
        public string Jabber { get; set; }
        [JsonProperty("icq")]
        public string Icq { get; set; }
        [JsonProperty("skype")]
        public string Skype { get; set; }
        [JsonProperty("aim")]
        public string Aim { get; set; }
        [JsonProperty("website")]
        public string Website { get; set; }
        [JsonProperty("blog")]
        public string Blog { get; set; }
        [JsonProperty("devices")]
        public string Devices { get; set; }
        [JsonProperty("signature")]
        public string Signature { get; set; }   
    }
}
