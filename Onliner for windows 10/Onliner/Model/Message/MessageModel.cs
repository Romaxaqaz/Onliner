using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onliner.Model.Message
{

    public class MessageModel : IMessageModel
    {
        private string header = string.Empty;
        private string content = string.Empty;
        private string userSend = string.Empty;

        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        public string Header
        {
            get { return header; }
            set { header = value; }
        }

        public string UserSend
        {
            get { return userSend; }
            set { userSend = value; }
        }

        public MessageModel(string user, string header, string content)
        {
            UserSend = user;
            Header = header;
            Content = content;
        }

        public MessageModel()
        {
                
        }
    }
}
