using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpMessengerObjects.UserRequests
{
    public class HelloRequest : Request
    {
        public User sender;
        public HelloRequest(int senderUID, User sender) : base(typeof(HelloRequest), senderUID)
        {
            this.sender = sender;
        }
    }
}
