using CorpMessengerServer.ServerCommandsExecutionModule.CommandObjects;
using CorpMessengerServer.ServerCommandsExecutionModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpMessengerServer
{
    public enum ServerCommands
    {
        AddUser,
        SendObject,
    }
    public static class ServerCommandsMethods
    {
        public static void Execute(this ServerCommands command, IServer server, ServerCommandArgs args)
        {
            switch (command)
            {
                case ServerCommands.AddUser:
                    new AddUserCommand().Execute(server, args);
                    break;
                case ServerCommands.SendObject:
                    break;
                default:
                    break;
            }
        }
    }
}
