namespace Onliner.Expansion
{
    public static class DeviceType
    {
        public static bool IsMobile => 
            Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons");
    }
}
