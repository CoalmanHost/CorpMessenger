using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpMessengerObjects.UserRequests
{
    public class AddUserRequest : Request
    {
        public AddUserRequest(int senderUID) : base(typeof(AddUserRequest), senderUID)
        {
        }
    }
}
