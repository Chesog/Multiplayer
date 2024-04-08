using System;
using UnityEngine;

public class MessageController : MonoBehaviourSingleton<MessageController>
{
    public void HandleMessage(byte[] message)
    {
        MessageType temp = (MessageType)BitConverter.ToInt32(message, 0);
        
        switch (temp)
        {
            case MessageType.HandShake:
                break;
            case MessageType.Console:
                break;
            case MessageType.Position:
                break;
        }
        
    }
}
