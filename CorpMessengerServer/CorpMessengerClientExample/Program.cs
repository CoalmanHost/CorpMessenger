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
using System.IO;

namespace CorpMessengerClientExample
{
    class Program
    {
        static Socket serverSocket;
        static int bufferSize = 10240;
        static User me;
        static string configFilePath = $"{Directory.GetCurrentDirectory()}\\CorpMessengerClientProfile.json";

        //static List<Message> mailbox;

        class ConfigProfile
        {
            public User user;
            public string serverIP;
            public string serverPort;
        }
        static class Commands
        {
            public static void CheckMailbox(TimePeriod period)
            {
                serverSocket.Send(PackString(JsonConvert.SerializeObject(RequestType.GetMessages), bufferSize));
                serverSocket.Send(PackString(JsonConvert.SerializeObject(new GetMessagesRequest(me.Id, period)), bufferSize));
                byte[] data = new byte[bufferSize];
                serverSocket.Receive(data);
                GetMessagesCallback callback = JsonConvert.DeserializeObject<GetMessagesCallback>(UnpackString(data));
                Console.WriteLine();
                List<Message> messages = new List<Message>();
                foreach (var item in callback.messages)
                {
                    serverSocket.Send(PackString(JsonConvert.SerializeObject(RequestType.GetUser), bufferSize));
                    serverSocket.Send(PackString(JsonConvert.SerializeObject(new GetUserRequest(me.Id, item.SenderID)), bufferSize));
                    byte[] userData = new byte[bufferSize];
                    serverSocket.Receive(userData);
                    GetUserCallback userCallback = JsonConvert.DeserializeObject<GetUserCallback>(UnpackString(userData));
                    messages.Add(new Message(userCallback.requested, me, item.Text));
                }
                foreach (var item in messages)
                {
                    Console.WriteLine(item.ToString());
                }
                Console.WriteLine();
            }
            /*public static void CheckMailbox()
            {
                Console.WriteLine();
                foreach (var item in mailbox)
                {
                    Console.WriteLine(item.ToString());
                }
                Console.WriteLine();
            }*/
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
            locker.Reset();
            switch (commandElements[0])
            {
                case "check":
                    TimePeriod period = TimePeriod.All;
                    if (commandElements.Length > 1)
                    {
                        switch (commandElements[1])
                        {
                            case "day":
                                period = TimePeriod.Day;
                                break;
                            case "week":
                                period = TimePeriod.Week;
                                break;
                            case "month":
                                period = TimePeriod.Month;
                                break;
                            default:
                                break;
                        }
                    }
                    Commands.CheckMailbox(period);
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
            locker.Set();
        }

        class MessageContainer
        {
            public byte[] rawMessageCallback = new byte[bufferSize];
            public Socket workingSocket;
        }

        static ManualResetEvent locker = new ManualResetEvent(false);
        /*static void ListenIncomingMessages()
        {
            while (true)
            {
                locker.Reset();

                MessageContainer message = new MessageContainer();
                message.workingSocket = serverSocket;
                serverSocket.BeginReceive(message.rawMessageCallback, 0, bufferSize, 0, new AsyncCallback(AcceptIncomingMessage), message);

                locker.WaitOne();
            }
        }*/

        /*static void AcceptIncomingMessage(IAsyncResult asyncResultMessage)
        {
            MessageContainer messageContainer = (MessageContainer)asyncResultMessage.AsyncState;
            messageContainer.workingSocket.EndReceive(asyncResultMessage);

            locker.Set();

            SentObjectCallback callback = JsonConvert.DeserializeObject<SentObjectCallback>(UnpackString(messageContainer.rawMessageCallback));

            Message message = (JsonConvert.DeserializeObject<Message>(JsonConvert.SerializeObject(callback.sentData)));
            mailbox.Add(message);
        }*/
        static void Main(string[] args)
        {
            ConfigProfile config = null;
            bool successfullyRead = true;

            if (File.Exists(configFilePath))
            {
                try
                {
                    config = JsonConvert.DeserializeObject<ConfigProfile>(File.ReadAllText(configFilePath));
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to read existing profile.");
                    successfullyRead = false;
                }
                me = config.user;
            }
            else
            {
                successfullyRead = false;
            }
            if (!successfullyRead)
            {
                Console.WriteLine("Create new user profile:");
                me = new User();
                //Console.Write("Input UID -->");
                //me.Id = int.Parse(Console.ReadLine());
                Console.Write("Input Name -->");
                me.Name = Console.ReadLine();
                Console.Write("Input Surname -->");
                me.Surname = Console.ReadLine();
                Console.Write("Input Position -->");
                me.Position = Console.ReadLine();
                Console.Write("Input Phone -->");
                me.Phone = Console.ReadLine();
                Console.Write("Input Email -->");
                me.Email = Console.ReadLine();

                config = new ConfigProfile();
                Console.Write("Input server ip address -->");
                config.serverIP = Console.ReadLine();
                Console.Write("Input server port -->");
                config.serverPort = Console.ReadLine();
                config.user = me;
                File.WriteAllText(configFilePath, JsonConvert.SerializeObject(config));
            }

            //mailbox = new List<Message>();

            string serverAddress = config.serverIP;
            int serverPort = int.Parse(config.serverPort);

            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine("Connecting to server...");
            try
            {
                serverSocket.Connect(serverAddress, serverPort);
            }
            catch (Exception)
            {
                Console.WriteLine($"Failed to connect to {serverAddress}:{serverPort}! Closing client...");
            }
            serverSocket.Send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new HelloRequest(me.Id, me))));
            byte[] recieved = new byte[bufferSize];
            serverSocket.Receive(recieved);
            HelloCallback hello;
            hello = JsonConvert.DeserializeObject<HelloCallback>(UnpackString(recieved));
            me.Id = hello.uid;
            Console.WriteLine(hello.helloWords);
            /*Thread messagesListener = new Thread(new ThreadStart(ListenIncomingMessages));
            messagesListener.Start();*/
            while (true)
            {
                Console.Write(">");
                ParseCommand(Console.ReadLine());
            }
        }
    }
}
