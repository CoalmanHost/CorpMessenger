using CorpMessengerServer.ServerImplementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace СorpMessengerServer
{
    class Program
    {
        static void Main(string[] args)
        {
            MessengerServer server = new MessengerServer();
            server.Initialize();
        }
    }
}
