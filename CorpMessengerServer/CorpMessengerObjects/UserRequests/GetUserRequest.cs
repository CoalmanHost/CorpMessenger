using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpMessengerObjects.UserRequests
{
    public class GetUserRequest : Request
    {
        public int targetUID;
        public GetUserRequest(int senderUID, int targetUID) : base(typeof(GetUserRequest), senderUID)
        {
            this.targetUID = targetUID;
        }
    }
}
