using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onliner.Model.Message
{
    public interface IMessageModel
    {
        string Header { get; set; }
        string UserSend { get; set; }
        string Content { get; set; }
    }
}
