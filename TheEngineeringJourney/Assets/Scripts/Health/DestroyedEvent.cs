using System;
using UnityEngine;

[DisallowMultipleComponent]
public class DestroyedEvent : MonoBehaviour
{
    public event Action<DestroyedEvent, DestroyedEventArgs> OnDestroyed;

    public void CallDestroyedEvent(bool playerDead, int points)
    {
        OnDestroyed?.Invoke(this, new DestroyedEventArgs() { PlayerDead = playerDead, Points = points });
    }
}

public class DestroyedEventArgs : EventArgs
{
    public bool PlayerDead;
    public int Points;
}