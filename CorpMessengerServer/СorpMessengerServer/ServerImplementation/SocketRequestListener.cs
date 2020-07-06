using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace CorpMessengerServer.ServerImplementation
{
    class SocketRequestListener : IRequestListener
    {
        Socket listener;
        public SocketRequestListener(int maxUsersCount, int port)
        {
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(IPAddress.Any, port));
            listener.Listen(maxUsersCount);
        }
        public SocketRequestListener(Socket socket)
        {
            listener = socket;
        }
        public void Listen()
        {
            while (true)
            {

            }
        }

        public UserRequest ThrowRequest()
        {
            throw new NotImplementedException();
        }
    }
}
