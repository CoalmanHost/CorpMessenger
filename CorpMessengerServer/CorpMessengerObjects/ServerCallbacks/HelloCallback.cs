using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpMessengerObjects.ServerCallbacks
{
    public class HelloCallback : Callback
    {
        public string helloWords;
        public int uid;
        public HelloCallback(int uid, string helloWords) : base(uid)
        {
            this.uid = uid;
            this.helloWords = helloWords;
        }
    }
}
