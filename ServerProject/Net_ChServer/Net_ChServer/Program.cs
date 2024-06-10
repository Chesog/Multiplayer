namespace Net_ChServer
{
    using System;

    internal class Program
    {
        private static NetworkManagerServer _networkManagerServer = new NetworkManagerServer();
        public static void Main(string[] args)
        {
            Console.WriteLine("Hola Server Mundo");
            Console.ReadKey();

            int port = int.Parse(args.GetValue(0).ToString());
            _networkManagerServer.StartServer(port);

            Console.WriteLine(port);
            
            while (_networkManagerServer.IsServerRuning)
            {
                _networkManagerServer.Update();
            }
        }
    }
}