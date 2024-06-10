﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Net_Ch_System;

public class ChatScreen : MonoBehaviour
{
    public Text messages;
    public InputField inputMessage;
    
    private ServiceLocator _serviceLocator;
    private NetworkManagerClient _networkManagerClient = new NetworkManagerClient();
    private float hideScreenTime = 10.0f;

    protected void Start()
    {
        _serviceLocator = ServiceLocator.Global;
        _serviceLocator.Register<ChatScreen>(GetType(), this);
        this.gameObject.SetActive(false);
    }

    public void InitChatScreen()
    {
       // _serviceLocator.Get(out NetworkManagerClient clietn);
       // if (clietn != null)
       //     _networkManagerClient = clietn;

        inputMessage.onEndEdit.AddListener(OnEndEdit);
    }

    public void SwitchToChat()
    {
        if (gameObject.activeInHierarchy)
            gameObject.SetActive(false);
        else
        {
            gameObject.SetActive(true);
            inputMessage.ActivateInputField();
            StopCoroutine(HideChat());
        }
    }

    public void ReceiveConsoleMessage(string obj)
    {
        gameObject.SetActive(true);
        messages.text += obj + System.Environment.NewLine;
        StartCoroutine(HideChat());
    }

    private IEnumerator HideChat()
    {
        yield return new WaitForSeconds(hideScreenTime);
        gameObject.SetActive(false);
    }

    void OnEndEdit(string str)
    {
        if (!string.IsNullOrEmpty(inputMessage.text))
        {
            NetConsole temp;
            
            temp = new NetConsole(_networkManagerClient.playerName + " : " + inputMessage.text);
            _networkManagerClient.SendToServer(temp.Serialize());


            inputMessage.ActivateInputField();
            inputMessage.Select();
            inputMessage.text = "";

            if (gameObject.activeInHierarchy)
                StartCoroutine(HideChat());
        }
    }
}