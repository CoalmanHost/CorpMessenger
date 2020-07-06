using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpMessengerServer
{
    public interface IServer
    {
        void Initialize();
        bool Awake { get; }
        void WakeUp();
        void Sleep();
        void SendObject(int senderUID, int recieverUID, object sentObject);
        Dictionary<int, UserSession> CurrentSessions { get; }
        RequestHandler RequestReciever { get; }
        IUserDatabase Users { get; }
    }
}
