using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Onliner.Model.JsonModel.Message
{
    public class MessageJson
    {
        public Pagination pagination { get; set; }
        public string total { get; set; }
        public IEnumerable<MessageList> messages { get; set; }
    }

    public class Pagination
    {
        public int maxpages { get; set; }
        public int curpos { get; set; }
    }

    public class MessageList : INotifyPropertyChanged
    {
        [JsonIgnore]
        private bool isCheked = false;

        public string id { get; set; }
        public string fid { get; set; }
        public string folder { get; set; }
        public string authorId { get; set; }
        public string recipientId { get; set; }
        public string subject { get; set; }
        public string rawText { get; set; }
        public string text { get; set; }
        public string unread { get; set; }
        public string time { get; set; }
        public string authorName { get; set; }
        public string authorPosts { get; set; }
        public string authorRank { get; set; }
        public string authorSignature { get; set; }
        public string recipientName { get; set; }
        [JsonIgnore]
        public bool IsChecked
        {
            get { return isCheked; }
            set
            {
                isCheked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
