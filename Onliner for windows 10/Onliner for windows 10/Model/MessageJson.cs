using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onliner_for_windows_10.Model
{
    public class MessageJson
    {
        public Pagination pagination { get; set; }
        public string total { get; set; }
        public List<MessageList> messages { get; set; }
    }

    public class Pagination
    {
        public int maxpages { get; set; }
        public int curpos { get; set; }
    }

    public class MessageList
    {
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
    }
}
