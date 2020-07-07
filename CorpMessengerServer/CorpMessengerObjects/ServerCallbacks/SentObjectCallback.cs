using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpMessengerObjects.ServerCallbacks
{
    public class SentObjectCallback : Callback
    {
        public User sender;
        public object sentData;

        public SentObjectCallback(User sender, User receiver, object sentData) : base(receiver.Id)
        {
            this.sender = sender;
            this.sentData = sentData;
        }
    }
}
