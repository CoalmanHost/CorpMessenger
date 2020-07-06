using CorpMessengerServer.ServerCommandsExecutionModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpMessengerServer
{
    public interface ICommand
    {
        void Execute(IServer server, ServerCommandArgs args);
    }
}
