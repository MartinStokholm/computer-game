using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class MovementToPositionEvent : MonoBehaviour
{
    public event Action<MovementToPositionEvent, MovementToPositionArgs> OnMovementToPosition;

    public void CallMovementToPositionEvent(
        Vector3 movementPosition,
        Vector3 currentPosition,
        float movementSpeed,
        Vector2 movementDirection)
    {
    Debug.Log(this.name);
        OnMovementToPosition?.Invoke(this, new MovementToPositionArgs()
        {
            MovementPosition = movementPosition,
            CurrentPosition = currentPosition,
            MovementSpeed = movementSpeed,
            MovementDirection = movementDirection
        });
    }
}

public class MovementToPositionArgs : EventArgs
{
    public Vector3 MovementPosition;
    public Vector3 CurrentPosition;
    public float MovementSpeed;
    public Vector2 MovementDirection;
}
