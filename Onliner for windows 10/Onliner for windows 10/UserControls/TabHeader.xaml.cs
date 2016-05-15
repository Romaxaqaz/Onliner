
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OnlinerApp.UserControls
{
    public partial class TabHeader : UserControl
    {
        public string Glyph
        {
            get { return (string)GetValue(GlyphProperty); }
            set { SetValue(GlyphProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Glyph.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GlyphProperty =
            DependencyProperty.Register("Glyph", typeof(string), typeof(TabHeader), null);

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(TabHeader), null);

        public TabHeader()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
    }
}
