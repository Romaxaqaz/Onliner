using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onliner.Model.JsonModel.Profile
{
    interface IEditProfileData
    {
        string Token { get; set; }
        string City { get; set; }
        string Occupation { get; set; }
        string Interests { get; set; }
        string User_bday { get; set; }
        string User_bmonth { get; set; }
        string User_byear { get; set; }
        string Jabber { get; set; }
        string Icq { get; set; }
        string Skype { get; set; }
        string Aim { get; set; }
        string Website { get; set; }
        string Blog { get; set; }
        string Devices { get; set; }
        string Signature { get; set; }
    }
}
