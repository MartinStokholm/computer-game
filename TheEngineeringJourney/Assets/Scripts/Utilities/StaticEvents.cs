
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticEventHandler 
{
    // Room changed event
    public static event Action<RoomChangedEventArgs> OnRoomChanged;

    public static void CallRoomChangedEvent(Room room)
    {
        OnRoomChanged?.Invoke(new RoomChangedEventArgs() { Room = room });
    }
    
    
    public static event Action<RoomEnemiesDefeatedArgs> OnRoomEnemiesDefeated;

    public static void CallRoomEnemiesDefeatedEvent(Room room)
    {
        OnRoomEnemiesDefeated?.Invoke(new RoomEnemiesDefeatedArgs() { Room = room });
    }
    
    public static event Action<MultiplierArgs> OnMultiplier;

    public static void CallMultiplierEvent(bool multiplier)
    {
        OnMultiplier?.Invoke(new MultiplierArgs() { Multiplier = multiplier });
    }

    public static event Action OnPlayerDead;
    
    public static void CallPlayerDead()
    {
        OnPlayerDead?.Invoke();
    }
} 

public class RoomChangedEventArgs : EventArgs
{
    public Room Room;
}

public class RoomEnemiesDefeatedArgs : EventArgs
{
    public Room Room;
}

public class MultiplierArgs : EventArgs
{
    public bool Multiplier;
}