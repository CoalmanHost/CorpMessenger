using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpMessengerObjects.ServerCallbacks
{
    public class GetMessagesCallback : Callback
    {
        public Message[] messages;
        public GetMessagesCallback(int receiverUID, Message[] messages) : base(receiverUID)
        {
            this.messages = messages;
        }
    }
}
