using System.Linq;
using UnityEngine;

public static class PlayerUtils
{
    /// <summary>
    /// Get the nearest spawnable position for the player
    /// </summary>
    public static Vector3 GetSpawnPosition(Vector3 playerPosition)
    {
        var grid = GameManager.Instance.CurrentRoom.InstantiatedRoom.Grid;
        
        return GameManager.Instance.CurrentRoom.SpawnPositionArray
            .Select(spawnPositionGrid => grid.CellToWorld((Vector3Int)spawnPositionGrid))
            .OrderBy(spawnPositionWorld => Vector3.Distance(spawnPositionWorld, playerPosition))
            .FirstOrDefault();
    }

    /// <summary>
    /// Get AimDirection enum value
    /// </summary>
    public static AimDirection GetAimDirection(float angle) =>
        angle switch
        {
            >= 22f and <= 158f => AimDirection.Up,
            <= 180f and > 158f or > -180 and <= -135f => AimDirection.Left,
            > -135f and <= -45f => AimDirection.Down,
            > -45f and <= 0f or > 0 and < 22f => AimDirection.Right,
            _ => AimDirection.Right
        };
    
    
}