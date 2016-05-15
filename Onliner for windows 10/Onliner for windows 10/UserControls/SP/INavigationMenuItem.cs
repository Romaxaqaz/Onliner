using System;

namespace OnlinerApp.UserControls.SP
{
    public interface INavigationMenuItem
    {
        Type DestinationPage { get; }
        object Arguments { get; }
        string Label { get; }
    }
}
