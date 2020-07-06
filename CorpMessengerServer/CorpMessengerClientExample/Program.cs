using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorpMessengerObjects;
using CorpMessengerObjects.UserRequests;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using static CorpMessengerObjects.Utilities;
using CorpMessengerObjects.ServerCallbacks;
using System.Threading;
using System.Runtime.CompilerServices;

namespace CorpMessengerClientExample
{
    class Program
    {
        static Socket serverSocket;
        static int bufferSize = 1024;
        static User me;

        static List<Message> mailbox;
        static class Commands
        {
            public static void CheckMailbox()
            {
                Console.WriteLine();
                foreach (var item in mailbox)
                {
                    Console.WriteLine(item.ToString());
                }
                Console.WriteLine();
            }
            public static void SendMessage(int recieverUID, string message)
            {
                serverSocket.Send(PackString(JsonConvert.SerializeObject(new SendDataRequest(me.Id, recieverUID, message, typeof(string))), bufferSize));
            }
            public static void CloseClient()
            {
                serverSocket.Close();
                Environment.Exit(0);
            }
        }
        static void ParseCommand(string command)
        {
            string[] commandElements = command.Split(' ');
            if (commandElements.Length <= 0)
            {
                return;
            }
            switch (commandElements[0])
            {
                case "check":
                    Commands.CheckMailbox();
                    break;
                case "send":
                    serverSocket.Send(PackString(JsonConvert.SerializeObject(RequestType.SendMessage), bufferSize));
                    int receiverUID = int.Parse(commandElements[1]);
                    Commands.SendMessage(receiverUID, string.Join(" ", commandElements.ToList().GetRange(2, commandElements.Length - 2).ToArray()));
                    break;
                case "exit":
                    Commands.CloseClient();
                    break;
                default:
                    break;
            }
        }

        class MessageContainer
        {
            public byte[] rawMessageCallback = new byte[bufferSize];
            public Socket workingSocket;
        }

        static ManualResetEvent gotMessage = new ManualResetEvent(false);
        static void ListenIncomingMessages()
        {
            while (true)
            {
                gotMessage.Reset();

                MessageContainer message = new MessageContainer();
                message.workingSocket = serverSocket;
                serverSocket.BeginReceive(message.rawMessageCallback, 0, bufferSize, 0, new AsyncCallback(AcceptIncomingMessage), message);

                gotMessage.WaitOne();
            }
        }

        static void AcceptIncomingMessage(IAsyncResult asyncResultMessage)
        {
            gotMessage.Set();

            MessageContainer messageContainer = (MessageContainer)asyncResultMessage.AsyncState;
            messageContainer.workingSocket.EndReceive(asyncResultMessage);

            SentObjectCallback callback = JsonConvert.DeserializeObject<SentObjectCallback>(UnpackString(messageContainer.rawMessageCallback));

            Message message = (JsonConvert.DeserializeObject<Message>(JsonConvert.SerializeObject(callback.sentData)));
            mailbox.Add(message);
        }

        static void Main(string[] args)
        {
            me = new User();
            Console.Write("Input UID -->");
            me.Id = int.Parse(Console.ReadLine());
            me.Name = "George";
            me.Surname = "Bakush";
            me.Position = "C# Developer";
            me.Phone = "89135100304";
            me.Email = "bakush2108@gmail.com";

            mailbox = new List<Message>();

            string serverAddress = "";
            int serverPort = -1;
            Console.Write("Input server ip address -->");
            serverAddress = Console.ReadLine();
            Console.Write("Input server port -->");
            serverPort = int.Parse(Console.ReadLine());

            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine("Connecting to server...");
            serverSocket.Connect(serverAddress, serverPort);
            serverSocket.Send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new HelloRequest(me.Id, me))));
            byte[] recieved = new byte[bufferSize];
            serverSocket.Receive(recieved);
            HelloCallback hello;
            hello = JsonConvert.DeserializeObject<HelloCallback>(UnpackString(recieved));
            me.Id = hello.uid;
            Console.WriteLine(hello.helloWords);
            Thread messagesListener = new Thread(new ThreadStart(ListenIncomingMessages));
            messagesListener.Start();
            while (true)
            {
                Console.Write(">");
                ParseCommand(Console.ReadLine());
            }
        }
    }
}
