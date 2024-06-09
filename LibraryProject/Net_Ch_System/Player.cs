using System.Numerics;

namespace Net_Ch_System
{
    public struct Player
    {
        public string playerName;
        public int playerID;
        public int hp;
        public const int maxHP = 3;
        public Vector3 playerPos;
        public Vector3 playerRot;

        public Player(string playerName, int playerID)
        {
            this.playerName = playerName;
            this.playerID = playerID;
            playerPos = Vector3.Zero;
            playerRot = Vector3.Zero;
            hp = maxHP;
        }

        public void SetPosition(Vector3 newPos)
        {
            playerPos = newPos;
        }
    }
}