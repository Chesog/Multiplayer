using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    private ServiceLocator _serviceLocator;
    [SerializeField] private Rigidbody _rigidbody;

    [Header("Movement")] 
    [SerializeField] private float speed = 5.0f;
    private Vector3 _CurrentMovement;

    private void OnEnable()
    {
        _serviceLocator = ServiceLocator.Global;
        _serviceLocator.Register<Player_Movement>(GetType(), this);
    }

    public void MovePlayer(Vector3 obj)
    {
        _CurrentMovement = new Vector3(obj.x, 0f, obj.y);
        _rigidbody.AddForce(_CurrentMovement * speed);
    }
}
