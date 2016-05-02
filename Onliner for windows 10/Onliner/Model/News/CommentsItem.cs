using Onliner.Interface.News;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Onliner.Model.News
{
    public class CommentsItem : ICommentsItem, INotifyPropertyChanged
    {
        private string id = string.Empty;
        private string data = string.Empty;
        private string image = string.Empty;
        private string likeCount = string.Empty;
        private string nickname = string.Empty;
        private string time = string.Empty;
        private string colorItem = string.Empty;
        private bool like = false;


        public string ID
        {
            get { return id; }
            set
            {
                id = value;
                NotifyPropertyChanged();
            }
        }

        public string Data
        {
            get { return data; }
            set
            {
                data = value;
                NotifyPropertyChanged();
            }
        }

        public string Image
        {
            get { return image; }
            set
            {
                image = value;
                NotifyPropertyChanged();
            }
        }

        public string LikeCount
        {
            get { return likeCount; }
            set
            {
                likeCount = value;
                NotifyPropertyChanged();
            }
        }

        public string Nickname
        {
            get { return nickname; }
            set
            {
                nickname = value;
                NotifyPropertyChanged();
            }
        }

        public string Time
        {
            get { return time; }
            set
            {
                time = value;
                NotifyPropertyChanged();
            }
        }

        public string ColorItem
        {
            get { return colorItem; }
            set
            {
                colorItem = value;
                NotifyPropertyChanged();
            }
        }

        public bool Like
        {
            get { return like; }
            set
            {
                like = value;
                NotifyPropertyChanged();
            }
        }

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
