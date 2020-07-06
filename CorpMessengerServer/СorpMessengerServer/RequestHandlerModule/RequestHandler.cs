using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpMessengerServer
{
    public class RequestHandler
    {
        public IRequestListener Listener { get; }
        public RequestHandler(IRequestListener listener)
        {
            Listener = listener;
        }
        void HandleRequest(UserRequest request)
        {
            // TODO
        }
    }
}
