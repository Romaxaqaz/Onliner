using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onliner.Model.JsonModel.Authorization
{
    interface IAuthentication
    {
        string Token { get; set; }
        string Login { get; set; }
        string Password { get; set; }
        string Session_id { get; set; }
        string RecaptchaChallengeField { get; set; }
        string RecaptchaResponseField { get; set; }
    }
}
