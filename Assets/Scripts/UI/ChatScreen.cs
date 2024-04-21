      using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class ChatScreen : MonoBehaviourSingleton<ChatScreen>
{
    public Text messages;
    public InputField inputMessage;

    protected override void Initialize()
    {
        inputMessage.onEndEdit.AddListener(OnEndEdit);

        this.gameObject.SetActive(false);
    }

    public void ReceiveConsoleMessage(string obj)
    {
        messages.text += obj + System.Environment.NewLine;
        Debug.Log(obj);
    }

    void OnEndEdit(string str)
    {
        if (inputMessage.text != "")
        {
            NetConsole temp = new NetConsole(inputMessage.text);
            if (NetworkManager.Instance.isServer)
            {
                NetworkManager.Instance.Broadcast(temp.Serialize());
                messages.text += inputMessage.text + System.Environment.NewLine;
            }
            else
                NetworkManager.Instance.SendToServer(temp.Serialize());

            inputMessage.ActivateInputField();
            inputMessage.Select();
            inputMessage.text = "";
        }

    }

}
