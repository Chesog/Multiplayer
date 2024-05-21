using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    private ServiceLocator _serviceLocator;
    [SerializeField] private InputReader _input;

    private void OnEnable()
    {
        _serviceLocator = ServiceLocator.Global;
        _input.OnPlayerMove += MovePlayer;
    }

    private void MovePlayer(Vector2 obj)
    {
        
    }
}
