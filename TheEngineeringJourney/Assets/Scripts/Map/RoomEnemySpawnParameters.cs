using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class RoomEnemySpawnParameters
{
    #region Tooltip
    [Tooltip("Defines the dungeon level for this room with regard to how many enemies in total should be spawned")]
    #endregion Tooltip
    public MapLevelSO mapLevel;
    #region Tooltip
    [Tooltip("The minimum number of enemies to spawn in this room for this dungeon level.  The actual number will be a random value between the minimum and maximum values")]
    #endregion Tooltip
    public int MinTotalEnemiesToSpawn;
    #region Tooltip
    [Tooltip("The maximum number of enemies to spawn in this room for this dungeon level.  The actual number will be a random value between the minimum and maximum values.")]
    #endregion Tooltip
    public int MaxTotalEnemiesToSpawn;
    #region Tooltip
    [Tooltip("The minimum number of concurrent enemies to spawn in this room for this dungeon level.  The actual number will be a random value between the minimum and maximum values.")]
    #endregion Tooltip
    public int MinConcurrentEnemies;
    #region Tooltip
    [Tooltip("The maximum number of concurrent enemies to spawn in this room for this dungeon level.  The actual number will be a random value between the minimum and maximum values. ")]
    #endregion Tooltip
    public int MaxConcurrentEnemies;
    #region Tooltip
    [Tooltip("The minimum spawn interval in seconds for enemies in this room for this dungeon level.  The actual number will be a random value between the minimum and maximum values.")]
    #endregion Tooltip
    public int MinSpawnInterval;
    #region Tooltip
    [Tooltip("The maximum spawn interval in seconds for enemies in this room for this dungeon level.  The actual number will be a random value between the minimum and maximum values.")]
    #endregion Tooltip
    public int MaxSpawnInterval;
}


public static class RoomEnemySpawnParametersHelper
{
    /// <summary>
    /// Get the number of enemies to spawn for this room in this dungeon level
    /// </summary>
    public static int GetNumberOfEnemiesToSpawn(this IEnumerable<RoomEnemySpawnParameters> roomEnemySpawnParameters, MapLevelSO mapLevelSo)
    {
        var enemySpawnParameters = roomEnemySpawnParameters
            .SingleOrDefault(x => x.mapLevel == mapLevelSo);

        return enemySpawnParameters is not null
            ? Random.Range(enemySpawnParameters.MinTotalEnemiesToSpawn, enemySpawnParameters.MaxTotalEnemiesToSpawn)
            : 0;
    }
    
    /// <summary>
    /// Get the room enemy spawn parameters for this dungeon level - if none found then return null
    /// </summary>
    public static RoomEnemySpawnParameters GetRoomEnemySpawnParameters(this IEnumerable<RoomEnemySpawnParameters> roomEnemySpawnParameters, MapLevelSO mapLevelSo)  =>
        roomEnemySpawnParameters
            .SingleOrDefault(x => x.mapLevel == mapLevelSo);
}


