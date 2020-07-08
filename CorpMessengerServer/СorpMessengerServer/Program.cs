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
            try
            {
                MessengerServer server = new MessengerServer();
                server.Initialize();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }
    }
}
