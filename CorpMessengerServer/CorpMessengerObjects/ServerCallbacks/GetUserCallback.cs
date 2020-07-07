using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpMessengerObjects.ServerCallbacks
{
    public class GetUserCallback : Callback
    {
        public User requested;
        public GetUserCallback(int receiverUID, User requested) : base(receiverUID)
        {
            this.requested = requested;
        }
    }
}
