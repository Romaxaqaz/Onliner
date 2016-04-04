using Onliner.Interface.News;
using SQLite.Net.Attributes;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Onliner.Model.News
{
    public class ItemsNews : INewsItems, INotifyPropertyChanged
    {
        private string _countViews = string.Empty;
        private string _themes = string.Empty;
        private string _title = string.Empty;
        private byte[] _image = null;
        private string _span = string.Empty;
        private string _description = string.Empty;
        private string _footer = string.Empty;
        private string _mediaicongray = string.Empty;
        private string _popularcount = string.Empty;
        private string _bmediaicon = string.Empty;

        public string NewsID { get; set; }

        public string Bmediaicon
        {
            get
            {
                return _bmediaicon;
            }

            set
            {
                _bmediaicon = value;
            }
        }

        public string CountViews
        {
            get
            {
                return _countViews;
            }
            set
            {
                _countViews = value;
                NotifyPropertyChanged();
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                _description = value;
            }
        }

        public string Footer
        {
            get
            {
                return _footer;
            }
            set
            {
                _footer = value;
                NotifyPropertyChanged();
            }
        }

        public byte[] Image
        {
            get
            {
                return _image;
            }

            set
            {
                _image = value;
            }
        }

        public string Mediaicongray
        {
            get
            {
                return _mediaicongray;
            }

            set
            {
                _mediaicongray = value;
            }
        }

        public string Popularcount
        {
            get
            {
                return _popularcount;
            }
            set
            {
                _popularcount = value;
                NotifyPropertyChanged();
            }
        }

        public string Span
        {
            get
            {
                return _span;
            }

            set
            {
                _span = value;
            }
        }

        public string Themes
        {
            get
            {
                return _themes;
            }

            set
            {
                _themes = value;
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }

        [PrimaryKey]
        public string LinkNews { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
