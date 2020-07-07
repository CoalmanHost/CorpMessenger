using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using CorpMessengerObjects;

namespace CorpMessengerServer
{
    public interface IServerDatabase : IDisposable
    {
        User AddUser(User user);
        User GetUser(int uid);
        int GetUID(User user);
        bool UserExists(User user);
        bool UserExists(int uid);
        void AddMessage(Message message);
        IEnumerable<Message> GetMessages(Expression<Func<Message, bool>> condition);
    }
}
