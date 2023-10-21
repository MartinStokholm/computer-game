using System;
using UnityEngine;

[DisallowMultipleComponent]
public class IdleEvent : MonoBehaviour
{
    public event Action<IdleEvent> OnIdle;

    public void CallIdleEvent()
    {
        Debug.Log("Idle event");
        OnIdle?.Invoke(this);
    }
}
