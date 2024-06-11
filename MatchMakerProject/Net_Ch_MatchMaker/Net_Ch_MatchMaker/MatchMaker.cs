using System.Diagnostics;

namespace Net_Ch_MatchMaker
{
    using System.Net;
    using Net_Ch_System;
    public class MatchMaker : IReceiveData
    {
        public bool isMatchMakerRuning;
        
        private UdpConnection connection;
        private readonly Dictionary<int, Client> clients = new Dictionary<int, Client>();
        private readonly Dictionary<IPEndPoint, int> ipToId = new Dictionary<IPEndPoint, int>();

        private int conectionID;
        public int port { get; protected set; }
        
        public void InitMatchMaker()
        {
            port = 1441;
            connection = new UdpConnection(port,this);
            isMatchMakerRuning = true;
            Console.WriteLine("Init MatchMaker");
            //Process.Start(,)
        }

        public void CloseMatchMaker()
        {
            // Todo : Mandarle un mensaje a cada cliente para cerrar la conexion
            connection.Close();
        }

        public void HandleServerMessage(byte[] data, IPEndPoint ep = null)
        {
            MessageType receivedMessage = (MessageType)BitConverter.ToInt32(data, 0);
            switch (receivedMessage)
            {
                case MessageType.ClientToMatchMakerHS:
                    if (ep == null)
                        throw new ArgumentException($"MatchMaker : IPEndPoint is null");
                    
                    NetClientToMatchMakerHS C2MMHS = new NetClientToMatchMakerHS();
                    if (C2MMHS.CheckMessage(data))
                    {
                       if (!ipToId.ContainsKey(ep))
                       {
                           ipToId[ep] = conectionID;
                           float realtimeSinceStartup = 0.0f;
                       
                           clients.Add(conectionID, new Client(ep, conectionID,realtimeSinceStartup));
                           
                           NetMatchMakerToClientHS MM2CHS = new NetMatchMakerToClientHS();
                           SendToClient(MM2CHS.Serialize(),ep);
                           Console.WriteLine(nameof(MessageType.ClientToMatchMakerHS) + ": The message is ok");
                       
                           conectionID++;
                       }
                    }
                    else
                    {
                        Console.Error.WriteLine(nameof(MessageType.ClientToMatchMakerHS) + ": The message is corrupt");
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnReceiveData(byte[] data, IPEndPoint ipEndpoint)
        {
            HandleServerMessage(data,ipEndpoint);
        }
        
        public void SendToClient(byte[] data, IPEndPoint ip)
        {
            connection.Send(data, ip);
        }

        public void Broadcast(byte[] data)
        {
            using (var iterator = clients.GetEnumerator())
            {
                while (iterator.MoveNext())
                {
                    connection.Send(data, iterator.Current.Value.ipEndPoint);
                }
            }
        }

        public void BroadcastWithException(byte[] data, Client exceptionClient)
        {
            using (var iterator = clients.GetEnumerator())
            {
                while (iterator.MoveNext())
                {
                    if (iterator.Current.Value != exceptionClient)
                        connection.Send(data, iterator.Current.Value.ipEndPoint);
                }
            }
        }
        
        
        public virtual void Update()
        {
            Console.SetCursorPosition(0,0);
            Console.WriteLine("Current clients Count : " + clients.Count);
            
            // Flush the data in main thread
            if (connection != null)
                connection.FlushReceiveData();
        }
    }
}