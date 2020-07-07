using CorpMessengerObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Net;
using LinqKit;
using System.Linq.Expressions;

namespace CorpMessengerServer.ServerImplementation
{
    public class DatabaseExample : IServerDatabase
    {
        public class UsersDatabaseContext : DbContext
        {
            public UsersDatabaseContext() : base("CorpMessengerServerDatabaseConnection")
            {
                Database.CreateIfNotExists();
            }
            public DbSet<User> Users { get; set; }
            public DbSet<Message> Messages { get; set; }
        }
        public UsersDatabaseContext database;
        public DatabaseExample()
        {
            database = new UsersDatabaseContext();
            MessengerServer.LogToConsole("Database is ready.");
        }
        public User AddUser(User user)
        {
            user.RegistrationDateTime = DateTime.Now;
            User added = database.Users.Add(user);
            database.SaveChanges();
            MessengerServer.LogToConsole($"Added user {added.Name} {added.Surname} with uid {added.Id}");
            return added;
        }

        public int GetUID(User user)
        {
            User found = null;
            try
            {
                var result = (from u in database.Users where u.Equals(user) select u).First();
                MessengerServer.LogToConsole(result.GetType().ToString());
                //found = ;
            }
            catch (Exception e)
            {
                MessengerServer.LogToConsole(e.Message);
                return -1;
            }
            return found.Id;
        }

        public User GetUser(int uid)
        {
            return database.Users.Find(uid);
        }

        public bool UserExists(User user)
        {
            MessengerServer.LogToConsole($"Trying to find user {user.Name} {user.Surname} with uid {user.Id} in the database...");
            return database.Users.Find(user.Id) != null;
        }

        public bool UserExists(int uid)
        {
            return GetUser(uid) == null;
        }

        public void AddMessage(Message message)
        {
            database.Messages.Add(message);
            database.SaveChanges();
        }
        public IEnumerable<Message> GetMessages(Expression<Func<Message, bool>> condition)
        {
            var messages = database.Messages.AsExpandable().Where(condition).AsEnumerable();
            return messages;
        }

        public void Dispose()
        {
            database.Dispose();
        }
    }
}
