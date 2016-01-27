using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Onliner_for_windows_10.Tiles
{
    class DesignTimeData
    {
        public string Title
        {
            get { return "Roma-qaz"; }
        }

        public int Number
        {
            get
            { return 15; }
        }

        public string Img
        {
            get { return "/ImageCollection/logoApp.png"; }
        }

        public SolidColorBrush BackgroundColour
        {
            get
            {
                return new SolidColorBrush(Colors.Aquamarine);
            }
        }
    }
}
