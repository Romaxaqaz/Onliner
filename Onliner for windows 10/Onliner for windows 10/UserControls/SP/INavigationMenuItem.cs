using System;

namespace Onliner_for_windows_10.UserControls.SP
{
    public interface INavigationMenuItem
    {
        Type DestinationPage { get; }
        object Arguments { get; }
        string Label { get; }
    }
}
