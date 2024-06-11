

using System.Diagnostics;

namespace Net_ChServer
{
    using System.Net;
    using Net_Ch_System;
    
    public class NetworkManagerServer : NetworkManager
    {
        private readonly Dictionary<int, Client> clients = new Dictionary<int, Client>();
        private readonly Dictionary<IPEndPoint, int> ipToId = new Dictionary<IPEndPoint, int>();
        private Dictionary<Client, DateTime> lastPingTimeForClient = new Dictionary<Client, DateTime>();
        private int clientId;
        public bool IsServerRuning;

        public void StartServer(int port)
        {
            this.port = port;
            connection = new UdpConnection(this.port, this);

            IsServerRuning = true;
            Console.WriteLine("Start Server");
        }
        
        public void AddClient(IPEndPoint ip, Player lean) // servidor 
        {
            if (!ipToId.ContainsKey(ip))
            {
                lean.playerID = clientId;
                ipToId[ip] = clientId;
                float realtimeSinceStartup = 0.0f;

                clients.Add(clientId, new Client(ip, lean.playerID,realtimeSinceStartup));
                playersInMatch.Add(lean);
                clientId++;

                //Process.Start(" ");
                //ProcessStartInfo asd;
            }

            foreach (Player player in playersInMatch)
            {
                Console.Write("Player Name : " + player.playerName + " Player ID : " + player.playerID);
            }
        }

        public void RemoveClient(IPEndPoint ip) // servidor 
        {
            if (ipToId.ContainsKey(ip))
            {
                Console.Write("Removing client: " + ip.Address);
                clients.Remove(ipToId[ip]);
                RemovePlayer(ipToId[ip]);
            }
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

        public void SetLastRecivedPingTime(DateTime currentTime, Client client)
        {
            lastPingTimeForClient[client] = currentTime;
        }

        public bool CheckTimeDiference(DateTime currentTime, Client client)
        {
            if (lastPingTimeForClient.ContainsKey(client))
            {
                float diference = currentTime.Second - lastPingTimeForClient[client].Second;
                if (diference > TimeOut)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public override void OnReceiveData(byte[] data, IPEndPoint ip)
        {
            HandleServerMessage(data, ip);
        }

        public void HandleServerMessage(byte[] data, IPEndPoint ep = null)
        {
            MessageType temp = (MessageType)BitConverter.ToInt32(data, 0);

            switch (temp)
            {
                case MessageType.Console:
                    NetConsole con = new NetConsole(data);
                    if (con.CheckMessage(data))
                    {
                        Broadcast(con.Serialize());
                        NetConsole.OnDispatch?.Invoke(con.GetData());
                        Console.Write(nameof(MessageType.Console) + ": The message is ok");
                    }
                    else
                        Console.Error.Write(nameof(MessageType.Console) + ": The message is corrupt");

                    break;
                case MessageType.Position:
                    NetPosition pos = new NetPosition(data);
                    if (pos.CheckMessage(data))
                    {
                        //if (spawnedPlayers.ContainsKey(ipToId[ep]))
                        //{
                        //    spawnedPlayers[pos.GetData().Item2].transform.position = pos.GetData().Item3;
                        //    playersInMatch[pos.GetData().Item2].SetPosition(pos.GetData().Item3);
                        //    BroadcastWithException(pos.Serialize(),clients[ipToId[ep]]);
                        //}


                        Console.Write(nameof(MessageType.Position) + ": The message is ok");
                    }
                    else
                    {
                        Console.Error.Write(nameof(MessageType.Position) + ": The message is corrupt");
                    }

                    break;
                case MessageType.ClientToServerHS:
                    if (ep == null)
                        throw new ArgumentException($"NetworkManagerServer : IPEndPoint is null");

                    NetClientToServerHS c2s = new NetClientToServerHS(data);
                    if (c2s.CheckMessage(data))
                    {
                        AddClient(ep, c2s.GetData());

                        NetServerToClientHS s2c = new NetServerToClientHS(GetCurrentPlayers());
                        Broadcast(s2c.Serialize());

                        NetPlayerList updatedList = new NetPlayerList(GetCurrentPlayers());
                        Broadcast(updatedList.Serialize());

                        foreach (Player player in playersInMatch)
                        {
                            //if (!spawnedPlayers.ContainsKey(player.playerID))
                            //spawnedPlayers.Add(player.playerID,SpawnSidePlayer());
                        }

                        Console.Write(nameof(MessageType.ClientToServerHS) + ": The message is ok");
                    }
                    else
                    {
                        Console.Write(nameof(MessageType.ClientToServerHS) + ": The message is corrupt");
                    }

                    break;
                case MessageType.Ping:
                    if (ep == null)
                        throw new ArgumentException($"NetworkManagerServer : IPEndPoint is null");

                    NetPing ping = new NetPing();
                    if (ping.CheckMessage(data))
                    {
                        if (!lastPingTimeForClient.ContainsKey(clients[ipToId[ep]]))
                            lastPingTimeForClient.TryAdd(clients[ipToId[ep]], DateTime.UtcNow);

                        if (CheckTimeDiference(DateTime.UtcNow, clients[ipToId[ep]]))
                        {
                            SetLastRecivedPingTime(DateTime.UtcNow, clients[ipToId[ep]]);
                            SendToClient(ping.Serialize(), ep);
                        }
                        else
                        {
                            // NetworkManager.Instance.RemoveClient(ep);
                        }

                        Console.Write(nameof(MessageType.Ping) + ": The message is ok");
                    }
                    else
                    {
                        Console.Write(nameof(MessageType.Ping) + ": The message is corrupt");
                    }

                    break;
            }
        }

        public override void Update()
        {
            base.Update();
            if (clients.Count > 0)
            {
                foreach (KeyValuePair<int, Client> client in clients)
                {
                    if (!CheckTimeDiference(DateTime.UtcNow, client.Value))
                    {
                        Console.Write("NetworkManagerServer : Disconect client " + client.Value.id);
                        //Desconectar al Server
                    }
                }
            }
        }
    }
}