using System;
using UnityEngine;

public class MessageController : MonoBehaviourSingleton<MessageController>
{
    public Action<byte[]> OnRecieveHandShakeMessage;
    public Action<string> OnRecieveConsoleMessage;
    public Action<Vector3> OnRecievePositionMessage;
    public void HandleMessage(byte[] message)
    {
        MessageType temp = (MessageType)BitConverter.ToInt32(message, 0);
        
        switch (temp)
        {
            case MessageType.HandShake:
                //OnRecieveHandShakeMessage?.Invoke();
                break;
            case MessageType.Console:
                NetConsole a = new NetConsole(message);
                OnRecieveConsoleMessage?.Invoke(a.GetData());
                break;
            case MessageType.Position:
                //OnRecievePositionMessage?.Invoke();
                break;
        }
        
    }
}
