namespace NetChesoNetwork
{
    public interface INetObject
    {
        public int GetID();
        public string GetOwner();
        public NetObject GetNetObject();
    }
}