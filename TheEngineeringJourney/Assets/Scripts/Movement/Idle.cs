using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(IdleEvent))]
[DisallowMultipleComponent]
public class Idle : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    private IdleEvent _idleEvent;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _idleEvent = GetComponent<IdleEvent>();
    }

    private void OnEnable()
    {
        _idleEvent.OnIdle += IdleEvent_OnIdle;
    }

    private void OnDisable()
    {
        _idleEvent.OnIdle -= IdleEvent_OnIdle;
    }

    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        MoveRigidBody();
    }

    private void MoveRigidBody()
    {
        _rigidbody2D.velocity = Vector2.zero;
    }
}
