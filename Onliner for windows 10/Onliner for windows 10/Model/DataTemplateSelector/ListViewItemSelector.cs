using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Onliner_for_windows_10.Model.DataTemplateSelector
{
    public class ListViewItemSelector : Windows.UI.Xaml.Controls.DataTemplateSelector
    {
        public DataTemplate HeaderTemlate { get; set; }
        public DataTemplate TitleTemlate { get; set; }
        public DataTemplate PictureTemlate { get; set; }

        public DataTemplate StoryTemlate { get; set; }
        public DataTemplate ImageTemlate { get; set; }
        public DataTemplate VideoTemlate { get; set; }
        public DataTemplate OtherTemlate { get; set; }
        public DataTemplate WebTemlate { get; set; }


        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var listItem = item as ListViewItemSelectorModel;
            if (listItem.Type.Equals("story"))
                return StoryTemlate;
            else if (listItem.Type.Equals("image"))
                return ImageTemlate;
            else if (listItem.Type.Equals("video"))
                return VideoTemlate;
            else if (listItem.Type.Equals("header"))
                return HeaderTemlate;
            else if (listItem.Type.Equals("title"))
                return TitleTemlate;
            else if (listItem.Type.Equals("picture"))
                return PictureTemlate;
            else if (listItem.Type.Equals("web"))
                return WebTemlate;

            return base.SelectTemplateCore(item, container);
        }
    }
}
