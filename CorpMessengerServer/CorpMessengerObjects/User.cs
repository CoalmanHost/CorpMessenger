using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpMessengerObjects
{
    public class User : IEquatable<User>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Position { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime RegistrationDateTime { get; set; }

        public User()
        {
             
        }

        public bool Equals(User other)
        {
            return Id == other.Id;
        }
    }
}
