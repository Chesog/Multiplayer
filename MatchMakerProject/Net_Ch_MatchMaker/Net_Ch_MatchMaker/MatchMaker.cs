namespace Net_ChMatchMaker
{
    using Net_Ch_System;
    using System;
    using System.Net;
    public class MatchMaker : IReceiveData
    {
        private UdpConnection connection;
        public int port { get; protected set; }
        
        public void InitMatchMaker()
        {
            port = 1441;
            connection = new UdpConnection(port,this);
            Console.WriteLine("Init MatchMaker");
        }

        public void OnReceiveData(byte[] data, IPEndPoint ipEndpoint)
        {
            throw new System.NotImplementedException();
        }
    }
}