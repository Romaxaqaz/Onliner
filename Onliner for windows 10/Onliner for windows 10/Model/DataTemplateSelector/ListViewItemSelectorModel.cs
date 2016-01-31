using MyToolkit.Multimedia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onliner_for_windows_10.Model.DataTemplateSelector
{
    public class ListViewItemSelectorModel
    {
        private string newsID = string.Empty;
        private string type = string.Empty;
        private string value = string.Empty;
        private string additionalValue = string.Empty;
        private string categoryNews = string.Empty;
        private string time = string.Empty;
        private string title = string.Empty;
        private string image = string.Empty;
        private Uri uriYoutube = null;

        public string CategoryNews
        {
            get { return categoryNews; }
            set { categoryNews = value; }
        }

        public string Time
        {
            get { return time; }
            set { time = value; }
        }

        public string Image
        {
            get { return image; }
            set { image = value; }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string NewsID
        {
            get { return newsID; }
            set { newsID = value; }
        }

        public Uri UriYoutube
        {
            get { return uriYoutube; }
            set { uriYoutube = value; }
        }

        public string Type { get { return type; } set { type = value; } }
        public string Value { get { return value; } set { this.value = value; } }
        public string AddValue { get { return additionalValue; } set { this.additionalValue = value; } }

        public ListViewItemSelectorModel()
        {
        }

        public ListViewItemSelectorModel(string type, string category, string time)
        {
            this.Type = type;
            this.CategoryNews = category;
            this.Time = time;
        }

        public ListViewItemSelectorModel(string type, Uri uri)
        {
            this.Type = type;
            this.UriYoutube = uri;
        }

        public ListViewItemSelectorModel(string type, string param)
        {
            this.Type = type;
            Value = param;
        }
    }
}
