using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpMessengerObjects
{
    public class Message
    {
        public string Text { get; }
        public DateTime RecieveTime { get; }
        public User Sender { get; }
        public Message(User sender, string text)
        {
            Sender = sender;
            Text = text;
            RecieveTime = DateTime.Now;
        }
        public override string ToString()
        {
            return $"{RecieveTime.ToString()} from {Sender.Name} {Sender.Surname} with id {Sender.Id} ->{Text}";
        }
    }
}
