using CorpMessengerServer;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using CorpMessengerObjects;
using CorpMessengerObjects.UserRequests;
using Newtonsoft.Json;
using CorpMessengerObjects.ServerCallbacks;
using static CorpMessengerObjects.Utilities;
using System.Linq.Expressions;
using System.Data.Entity;

namespace CorpMessengerServer.ServerImplementation
{
    class MessengerServer : IServer
    {
        public readonly int serverPort = 1001;
        public readonly int usersMaxCount = 100;
        public const int bufferSize = 10240;
        public bool Awake => throw new NotImplementedException();

        public Dictionary<int, UserSession> CurrentSessions => throw new NotImplementedException();

        public IServerDatabase Database { get; private set; }

        Dictionary<int, UserSession> currentSessions;
        public static void LogToConsole(string message)
        {
            Console.WriteLine($"{DateTime.Now} > {message}");
        }

        public void Initialize()
        {
            LogToConsole("Initializing database...");
            Database = new DatabaseExample();
            currentSessions = new Dictionary<int, UserSession>();
            LogToConsole("Starting listening...");
            StartListening();
        }

        public void SendObject(int senderUID, int receiverUID, object sentObject)
        {
            Message message = new Message(Database.GetUser(senderUID), Database.GetUser(receiverUID), (string)sentObject);
            Database.AddMessage(message);
            if (currentSessions.ContainsKey(receiverUID))
            {
                UserSession sender = currentSessions[senderUID];
                UserSession receiver = currentSessions[receiverUID];

                SentObjectCallback callback = new SentObjectCallback(Database.GetUser(sender.masterUID), Database.GetUser(receiver.masterUID), message);

                byte[] toSend = Utilities.PackString(JsonConvert.SerializeObject(callback), bufferSize);
                receiver.workingSocket.BeginSend(toSend, 0, bufferSize, 0, new AsyncCallback(EndSendingObject), receiver.workingSocket);
            }
        }
        
        void SendMessages(int receiverUID, TimePeriod period)
        {
            UserSession session = currentSessions[receiverUID];
            MessengerServer.LogToConsole($"Trying to get messages for user {receiverUID} from the database...");
            Expression<Func<Message, bool>> condition = m => m.ReceiverID == receiverUID;
            switch (period)
            {
                case TimePeriod.All:
                    condition = m => m.ReceiverID == receiverUID;
                    break;
                case TimePeriod.Day:
                    condition = m => m.ReceiverID == receiverUID && DbFunctions.DiffDays(DateTime.Now, m.RecieveTime) <= 1;
                    break;
                case TimePeriod.Week:
                    condition = m => m.ReceiverID == receiverUID && DbFunctions.DiffDays(DateTime.Now, m.RecieveTime) <= 7;
                    break;
                case TimePeriod.Month:
                    condition = m => m.ReceiverID == receiverUID && DbFunctions.DiffMonths(DateTime.Now, m.RecieveTime) <= 1;
                    break;
                default:
                    break;
            }
            GetMessagesCallback callback = new GetMessagesCallback(receiverUID, Database.GetMessages(condition).ToArray());

            byte[] toSend = PackString(JsonConvert.SerializeObject(callback), bufferSize);
            session.workingSocket.BeginSend(toSend, 0, bufferSize, 0, new AsyncCallback(EndSendingObject), session.workingSocket);
        }
        void SendUser(int receiverUID, int targetUID)
        {
            UserSession session = currentSessions[receiverUID];
            MessengerServer.LogToConsole($"Trying to get user info for user {receiverUID} from the database...");
            User target = Database.GetUser(targetUID);
            GetUserCallback callback = new GetUserCallback(receiverUID, target);

            byte[] toSend = PackString(JsonConvert.SerializeObject(callback), bufferSize);
            session.workingSocket.BeginSend(toSend, 0, bufferSize, 0, new AsyncCallback(EndSendingObject), session.workingSocket);
        }
        void EndSendingObject(IAsyncResult asyncResult)
        {
            ((Socket)(asyncResult.AsyncState)).EndSend(asyncResult);
        }


        public void Sleep()
        {
            throw new NotImplementedException();
        }

        public void WakeUp()
        {
            throw new NotImplementedException();
        }

        class ShippedData
        {
            public const int size = bufferSize;
            public byte[] data = new byte[size];
            public Socket workingSocket;
        }

        ManualResetEvent locker = new ManualResetEvent(false);
        void StartListening()
        {
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(IPAddress.Any, serverPort));
            listener.Listen(usersMaxCount);

            LogToConsole("Listener initialized.");
            LogToConsole("Ready!");

            while (true)
            {
                locker.Reset();

                listener.BeginAccept(new AsyncCallback(AcceptConnection), listener);

                locker.WaitOne();
            }


        }
        void AcceptConnection(IAsyncResult acceptInfo)
        {
            locker.Set();

            Socket accepted = ((Socket)acceptInfo.AsyncState).EndAccept(acceptInfo);
            LogToConsole($"Incoming connection from {accepted.RemoteEndPoint.ToString()}");
            ShippedData dataContainer = new ShippedData();
            dataContainer.workingSocket = accepted;
            accepted.BeginReceive(dataContainer.data, 0, ShippedData.size, 0, new AsyncCallback(ReadConnection), dataContainer);
        }
        void ReadConnection(IAsyncResult ar)
        {
            ShippedData dataContainer = (ShippedData)ar.AsyncState;
            dataContainer.workingSocket.EndReceive(ar);

            dynamic helloUser = JsonConvert.DeserializeObject<HelloRequest>(Utilities.UnpackString(dataContainer.data));
            User connectedUser = helloUser.sender;
            if (Database.UserExists(connectedUser))
            {
                LogToConsole($"Accepting existing user {connectedUser.Name} {connectedUser.Surname} with uid {connectedUser.Id}");
                UserSession us = new UserSession(connectedUser.Id, dataContainer.workingSocket);
                HelloCallback callback = new HelloCallback(connectedUser.Id, $"Hello user {connectedUser.Name} {connectedUser.Surname} with {us.masterUID}");
                us.workingSocket.BeginSend(PackString(JsonConvert.SerializeObject(callback), bufferSize), 0, bufferSize, 0, new AsyncCallback(CompleteUserSessionCreation), us);
            }
            else
            {
                LogToConsole($"New user {connectedUser.Name} {connectedUser.Surname} incoming. Adding to database...");
                User added = Database.AddUser(connectedUser);
                UserSession us = new UserSession(added.Id, dataContainer.workingSocket);
                HelloCallback callback = new HelloCallback(added.Id, $"Connected new user! Hello user {added.Name} {added.Surname} with {us.masterUID}");
                us.workingSocket.BeginSend(PackString(JsonConvert.SerializeObject(callback), bufferSize), 0, bufferSize, 0, new AsyncCallback(CompleteUserSessionCreation), us);
            }
        }
        void CompleteUserSessionCreation(IAsyncResult asyncResultUserSession)
        {
            UserSession us = (UserSession)asyncResultUserSession.AsyncState;
            us.workingSocket.EndSend(asyncResultUserSession);
            currentSessions.Add(us.masterUID, us);
            LogToConsole($"Created user session of user with uid {us.masterUID}");
            HandleUserSession(us);
        }


        class UserSessionIncomingRequestType
        {
            public UserSession session;
            public ShippedData dataContainer;
            public UserSessionIncomingRequestType(UserSession session)
            {
                this.session = session;
                dataContainer = new ShippedData();
            }
        }

        class UserSessionIncomingRequest : UserSessionIncomingRequestType
        {
            public RequestType requestType;
            public UserSessionIncomingRequest(UserSession session, RequestType requestType) : base(session)
            {
                this.requestType = requestType;
            }
        }

        void HandleUserSession(UserSession session)
        {
            Socket reciever = session.workingSocket;
            int uid = session.masterUID;
            UserSessionIncomingRequestType requestContainer = new UserSessionIncomingRequestType(session);
            reciever.BeginReceive(requestContainer.dataContainer.data, 0, bufferSize, 0, new AsyncCallback(ReadRequestType), requestContainer);
        }
        void ReadRequestType(IAsyncResult asyncResultUserSession)
        {
            UserSessionIncomingRequestType requestContainer = (UserSessionIncomingRequestType)asyncResultUserSession.AsyncState;
            try
            {
                requestContainer.session.workingSocket.EndReceive(asyncResultUserSession);
            }
            catch (Exception)
            {
                LogToConsole($"Disconnected user {requestContainer.session.masterUID}");
                currentSessions.Remove(requestContainer.session.masterUID);
                return;
            }

            byte[] requestTypeData = requestContainer.dataContainer.data;
            RequestType requestType = (RequestType)Enum.Parse(typeof(RequestType), JsonConvert.DeserializeObject<int>(UnpackString(requestTypeData)).ToString());
            GetRequestFrom(requestContainer.session, requestType);
        }
        
        void GetRequestFrom(UserSession session, RequestType requestType)
        {
            Socket reciever = session.workingSocket;
            int uid = session.masterUID;
            UserSessionIncomingRequest requestContainer = new UserSessionIncomingRequest(session, requestType);
            reciever.BeginReceive(requestContainer.dataContainer.data, 0, bufferSize, 0, new AsyncCallback(HandleRequest), requestContainer);
        }
        void HandleRequest(IAsyncResult asyncResultRequest)
        {
            UserSessionIncomingRequest requestContainer = (UserSessionIncomingRequest)asyncResultRequest.AsyncState;
            UserSession session = requestContainer.session;
            RequestType requestType = requestContainer.requestType;
            ShippedData dataContainer = requestContainer.dataContainer;
            switch (requestType)
            {
                case RequestType.Hello:
                    break;
                case RequestType.SendMessage:
                    SendDataRequest request = JsonConvert.DeserializeObject<SendDataRequest>(UnpackString(dataContainer.data));
                    SendObject(session.masterUID, request.recieverUID, request.data);
                    LogToConsole($"Message from user {request.senderUID} to user {request.recieverUID} with text: {(string)request.data}");
                    break;
                case RequestType.GetMessages:
                    LogToConsole($"Requested messages for {session.masterUID}");
                    GetMessagesRequest mrequest = JsonConvert.DeserializeObject<GetMessagesRequest>(UnpackString(dataContainer.data));
                    SendMessages(mrequest.senderUID, mrequest.period);
                    LogToConsole($"Sent messages to user {mrequest.senderUID}");
                    break;
                case RequestType.GetUser:
                    LogToConsole($"Requested user info for {session.masterUID}");
                    GetUserRequest urequest = JsonConvert.DeserializeObject<GetUserRequest>(UnpackString(dataContainer.data));
                    SendUser(urequest.senderUID, urequest.targetUID);
                    break;
                default:
                    break;
            }
            HandleUserSession(session);
        }
    }
}
