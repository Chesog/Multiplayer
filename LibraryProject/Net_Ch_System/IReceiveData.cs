namespace Net_Ch_System
{
    using System.Net;
    public interface IReceiveData
    {
        void OnReceiveData(byte[] data, IPEndPoint ipEndpoint);
    }
}