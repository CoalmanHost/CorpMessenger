using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpMessengerObjects.UserRequests
{
    public abstract class Request
    {
        public readonly int senderUID;
        public readonly Type requestType;
        public Request(Type requestType, int senderUID)
        {
            this.requestType = requestType;
            this.senderUID = senderUID;
        }
    }
}
