using Windows.Storage;

namespace Onliner.Setting
{
    public static class SettingParams
    {
        private static readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        public const string AvatarKey = "Avatar";
        public const string NickNameKey = "NickName";
        public const string AuthorizationKey = "Authorization";
        public const string ImageLoadKey = "ImageLoad";
        public const string LastWeatherKey = "LastWeather";
        public const string LastCurrentKey = "LastCurrent";
        public const string LastMessageKey = "LastMessag";

        public const string AutoLoadNewsAtStartUpAppKey = "AutoLoadNewsAtStartUpApp";
        public const string LoadImageKey = "LoadImage";
        public const string ThemeAppKey = "ThemeApp";

        //app menu
        public const string TownWeatherIdKey = "TownWeatherId";
        public const string CurrentTypeKey = "CurrentType";
        public const string BankActionKey = "BankAction";

        public const string NumberOfNewsitemsToTheCacheKey = "NumberOfNewsitemsToTheCache";
        public const string NewsDataTemplateKey = "NewsDataTemplate";

        public const string ToggleSwitchNewsDataTemplateTypeKey = "ToggleSwitchNewsDataTemplateType";

        //type news section
        public const string TileDataTemplate = "NewsThemeList";
        public const string ListDataTemplate = "CompactNewsDataTemplate";


        public static int NewsSectionIndex { get; set; }


        public static void SetParamsSetting(string key, string value)
        {
            LocalSettings.Values[key] = value;
        }

        public static object GetParamsSetting(string key)
        {
            return LocalSettings.Values[key];
        }

        public static void RemoveparamsSetting(string key)
        {
            LocalSettings.Values.Remove(key);
        }

    }
}
