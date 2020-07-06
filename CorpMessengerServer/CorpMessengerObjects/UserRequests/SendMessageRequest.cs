using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace CorpMessengerObjects.UserRequests
{
    public class SendDataRequest : Request
     {
        public readonly int recieverUID;
        public readonly Type dataType;
        public readonly object data;

        public SendDataRequest(int senderUID, int recieverUID, object data, Type dataType) : base(typeof(SendDataRequest), senderUID)
        {
            this.recieverUID = recieverUID;
            this.data = data;
            this.dataType = dataType;
        }
    }
}
