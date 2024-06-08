using System.Net;
namespace Net_Ch_System
{
    public interface IReceiveData
    {
        void OnReceiveData(byte[] data, IPEndPoint ipEndpoint);
    }
}