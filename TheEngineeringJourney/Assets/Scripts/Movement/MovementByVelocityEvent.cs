using System;
using UnityEngine;

[DisallowMultipleComponent]
public class MovementByVelocityEvent : MonoBehaviour
{
    public event Action<MovementByVelocityEvent, MovementByVelocityArgs> OnMovementByVelocity;

    public void CallMovementByVelocityEvent(Vector2 movementDirection, float movementSpeed)
    {
        OnMovementByVelocity?.Invoke(this, new MovementByVelocityArgs() { MovementDirection = movementDirection, MovementSpeed = movementSpeed});
    }
}

public class MovementByVelocityArgs : EventArgs
{
    public Vector2 MovementDirection;
    public float MovementSpeed;
}