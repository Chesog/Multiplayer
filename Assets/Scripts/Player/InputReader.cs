using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    private ServiceLocator _serviceLocator;
    private ChatScreen _chatScreen;
    [SerializeField] private PlayerInput _input;

    void OnEnable()
    {
        _serviceLocator = ServiceLocator.Global;
        _serviceLocator.Get<ChatScreen>(out ChatScreen chat);
        _chatScreen = chat;
        _input = GetComponent<PlayerInput>();
    }


    /// <summary>
    /// Action Event For The Player Movement
    /// </summary>
    public event Action<Vector2> OnPlayerMove;

    public event Action<bool> OnPlayerOpenChat;

    /// <summary>
    /// Triggers The Movement Event
    /// </summary>
    /// <param name="input"></param>
    public void OnMove(InputValue input)
    {
        OnPlayerMove?.Invoke(input.Get<Vector2>());
    }

    public void OnOpenChat(InputValue input)
    {
        if (_chatScreen.isActiveAndEnabled)
        {
            _chatScreen.inputMessage.ActivateInputField();
        }
        else
        {
            _chatScreen.SwitchToChat();
        }
    }
}