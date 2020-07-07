using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace CorpMessengerObjects.UserRequests
{
    public enum RequestType
    {
        Hello,
        SendMessage,
        GetMessages,
        GetUser,
    }
    public static class RequestsExtension
    {
        public static dynamic GetRequest(this Request request)
        {
            return Convert.ChangeType(request, request.requestType);
        }
    }
}
