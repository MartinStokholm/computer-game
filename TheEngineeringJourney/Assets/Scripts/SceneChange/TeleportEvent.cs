using System;
using UnityEngine;

public class TeleportEvent : MonoBehaviour
{
    public Vector3 teleportPosition;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Settings.PlayerTag))
        {
            StaticTeleportPositionEvent.CallOnTeleportEvent(teleportPosition);
        }
    }
}

public static class StaticTeleportPositionEvent
{
    public static event Action<TeleportPositionArgs> OnTeleportEvent;

    public static void CallOnTeleportEvent(Vector3 teleportPosition)
    {
        OnTeleportEvent?.Invoke(new TeleportPositionArgs() { TeleportPosition = teleportPosition});
    }
}

public class TeleportPositionArgs : EventArgs

{
    public Vector3 TeleportPosition;
}
