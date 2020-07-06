using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Timers;

namespace CorpMessengerServer
{
    public class UserSession
    {
        public readonly int masterUID;
        public Socket workingSocket;
        public long Lifetime { get; }
        public UserSession(int userUID, Socket userSocket)
        {
            masterUID = userUID;
            workingSocket = userSocket;
        }
    }
}
