using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpMessengerObjects
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime RecieveTime { get; set; }

        public int SenderID { get; set; }
        public int ReceiverID { get; set; }
        public User Sender { get; }
        public User Receiver { get; }

        public Message()
        {

        }
        public Message(User sender, User receiver, string text)
        {
            Sender = sender;
            SenderID = sender.Id;
            Receiver = receiver;
            ReceiverID = receiver.Id;
            Text = text;
            RecieveTime = DateTime.Now;
        }
        public override string ToString()
        {
            return $"{RecieveTime.ToString()} from {Sender?.Name} {Sender?.Surname} id {SenderID} to {Receiver?.Name} {Receiver?.Surname} id {ReceiverID} ->{Text}";
        }
    }
}
