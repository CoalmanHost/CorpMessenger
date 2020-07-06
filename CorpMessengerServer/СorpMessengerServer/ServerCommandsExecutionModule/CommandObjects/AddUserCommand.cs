using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorpMessengerObjects;

namespace CorpMessengerServer.ServerCommandsExecutionModule.CommandObjects
{
    public class AddUserCommand : ICommand
    {
        public class AddUserCommandArgs : ServerCommandArgs
        {
            public User UserToAdd { get { return (User)ArgsObject; } }
        }
        public void Execute(IServer server, ServerCommandArgs args)
        {
            server.Users.AddUser(((AddUserCommandArgs)args).UserToAdd);
        }
    }
}
