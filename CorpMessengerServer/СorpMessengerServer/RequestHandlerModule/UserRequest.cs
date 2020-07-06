using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpMessengerServer
{
    public class UserRequest
    {
        int userUID;
        string request;
        public UserRequest(byte[] rawRequest)
        {
            userUID = Convert.ToInt32(rawRequest.ToList().GetRange(0, sizeof(int)).ToArray());
            request = Convert.ToBase64String(rawRequest.ToList().GetRange(sizeof(int), rawRequest.Length).ToArray());
        }
    }
}
