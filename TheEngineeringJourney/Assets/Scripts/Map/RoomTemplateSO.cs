using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Room_", menuName = "Scriptable Objects/Map/Room")]
public class RoomTemplateSO : ScriptableObject
{
    [HideInInspector] public string Guid;

    #region Header ROOM PREFAB

    [Space(10)]
    [Header("ROOM PREFAB")]

    #endregion

    #region Tooltip

    [Tooltip(
        "The Game Object Prefab For The Room (Contains All The Tilemaps For The Room And Environment Game Objects)")]

    #endregion
    public GameObject Prefab;

    // Copy the prefab generate a new guid and new game object
    [HideInInspector] public GameObject PreviousPrefab;

    #region Header ROOM CONFIGURATION

    [Space(10)]
    [Header("ROOM CONFIGURATION")]

    #endregion

    #region Tooltip

    [Tooltip(
        "The Room Node Type SO. These types correspond to the room nodes used in the room node graph. The only exception is the Corridor, that only show Corridor instead of CorridorNS CorridorEW")]

    #endregion

    public RoomNodeTypeSO RoomNodeType;
    
    #region Tooltip

    [Tooltip(
        "To find bottom left corner, look at the tilemap of the room. Use a special tool to figure out the grid position of the tile in the top right corner. The position is in relation to the room's layout")]

    #endregion
    public Vector2Int LowerBounds;
    
    #region Tooltip

    [Tooltip(
        "To find top right corner, look at the tilemap of the room. Use a special tool to figure out the grid position of the tile in the top right corner. The position is in relation to the room's layout")]
    #endregion
    public Vector2Int UpperBounds;

    #region Tooltip

    [Tooltip(
        "There should be four doorways at most, each aligned with a compass direction. They should all have a consistent size of three tiles wide, with the middle tile serving as the central point of the doorway")]

    #endregion
    [SerializeField] public List<Doorway> Doorways;

    #region Tooltip
    [Tooltip("Add all potential spawn positions (for enemies and chests) in tilemap coordinates to this list.")]
    #endregion
    public Vector2Int[] SpawnPositions;
    
    #region Header ENEMY DETAILS

    [Space(10)]
    [Header("ENEMY DETAILS")]

    #endregion Header ENEMY DETAILS
    

    #region Tooltip
    
    [Tooltip("Populate the list with all the enemies that can be spawned in this room by dungeon level, including the ratio (random) of this enemy type that will be spawned")]
    
    #endregion Tooltip
    
    public List<SpawnableObjectsByLevel<EnemyDetailsSO>> EnemiesByLevelList;

    #region Tooltip

    [Tooltip("Populate the list with the spawn parameters for the enemies.")]
    
    #endregion Tooltip
    
    public List<RoomEnemySpawnParameters> RoomEnemySpawnParametersList;

    
    /// <summary>
    /// Returns the list of Entrances for the room template
    /// </summary>
    public List<Doorway> GetDoorwayList() => Doorways;

    #region Validation

#if UNITY_EDITOR

    // Validate SO fields
    private void OnValidate()
    {
        if (IsGuidEmpty(Guid) || IsPrefabChanged(PreviousPrefab, Prefab))
        {
            Guid = GUID.Generate().ToString();
            PreviousPrefab = Prefab;
            EditorUtility.SetDirty(this);
        }

        EditorUtilities.ValidateCheckEnumerableValues(this, nameof(Doorways), Doorways);

        // Check spawn positions populated
        EditorUtilities.ValidateCheckEnumerableValues(this, nameof(SpawnPositions), SpawnPositions);
    }

    private static bool IsPrefabChanged(GameObject previous, GameObject current) => previous != current;

    private static bool IsGuidEmpty(string Guid) => Guid == string.Empty;

#endif

    #endregion Validation
}

public static class RoomTemplateSOHElper
{
    public static (IEnumerable<RoomTemplateSO> uniqueTemplates, IEnumerable<RoomTemplateSO> duplicates) 
        GetSeperatedRoomTemplates(this IEnumerable<RoomTemplateSO> roomTemplateList)
    {
        var duplicates = roomTemplateList
            .GroupBy(roomTemplate => roomTemplate.Guid)
            .Where(group => group.Count() > 1)
            .SelectMany(group => group);

        var uniqueTemplates = roomTemplateList
            .Except(duplicates)
            .ToList();

        return (uniqueTemplates, duplicates);
    }
}