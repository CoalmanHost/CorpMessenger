﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpMessengerObjects.ServerCallbacks
{
    public abstract class Callback
    {
        public int receiverUID;
        public Callback(int receiverUID)
        {
            this.receiverUID = receiverUID;
        }
    }
}
