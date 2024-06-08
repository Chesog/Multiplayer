namespace Net_Ch_System
{
    public interface INetObject
    {
        public int GetID();
        public string GetOwner();
        public NetObject GetNetObject();
    }
}