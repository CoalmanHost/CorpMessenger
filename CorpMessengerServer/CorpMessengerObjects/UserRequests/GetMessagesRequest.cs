using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpMessengerObjects.UserRequests
{
    public enum TimePeriod
    {
        All,
        Day,
        Week,
        Month
    }
    public class GetMessagesRequest : Request
    {
        public TimePeriod period;
        public GetMessagesRequest(int senderUID, TimePeriod period) : base(typeof(GetMessagesRequest), senderUID)
        {
            this.period = period;
        }
    }
}
