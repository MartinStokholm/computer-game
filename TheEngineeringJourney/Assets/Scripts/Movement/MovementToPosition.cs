using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementToPositionEvent))]
[DisallowMultipleComponent]
public class MovementToPosition : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    private MovementToPositionEvent _movementToPositionEvent;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _movementToPositionEvent = GetComponent<MovementToPositionEvent>();
    }

    private void OnEnable()
    {
        _movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;
    }

    private void OnDisable()
    {
        _movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
    }

    /// <summary>
    /// Move the rigidbody component
    /// </summary>
    private void MovementToPositionEvent_OnMovementToPosition(
        MovementToPositionEvent @event,
        MovementToPositionArgs args)
    {
        Vector2 unitVector = Vector3.Normalize(args.MovementPosition - args.CurrentPosition);
        _rigidbody2D.MovePosition(_rigidbody2D.position + (unitVector * args.MovementSpeed * Time.fixedDeltaTime));
    }
}
