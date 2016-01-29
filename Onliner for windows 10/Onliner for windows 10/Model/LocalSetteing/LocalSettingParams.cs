using Onliner_for_windows_10.ProfilePage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onliner_for_windows_10.Model.LocalSetteing
{
    public static class LocalSettingParams
    {
        private static Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        private static readonly string DefAvatar = "/ImageCollection/default_avatar.png";

        private static string _autorization = "Autorization";
        private static string _login = "Login";
        private static string _avatarUrl = "AvatarUrl";

        public static string Autorization { get { return _autorization; } }
        public static string Login { get { return _login; } }
        public static string AvatarUrl { get { return _avatarUrl; } }

        public static void RemoveAllParams()
        {
            localSettings.Values[LocalSettingParams.Autorization] = "false";
            localSettings.Values[LocalSettingParams.Login] = null;
            Additionalinformation.Instance.Login = "Войдите";
            localSettings.Values[LocalSettingParams.AvatarUrl] = null;
            Additionalinformation.Instance.AvatarUrl = DefAvatar;
        }
    }
}
