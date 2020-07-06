using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorpMessengerObjects;

namespace CorpMessengerServer
{
    public interface IUserDatabase : IDisposable
    {
        User AddUser(User user);
        User GetUser(int uid);
        int GetUID(User user);
        bool UserExists(User user);
        bool UserExists(int uid);
    }
}
