using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Doorway
{
    public Vector2Int Position;
    
    public Orientation Orientation;
    
    public GameObject DoorPrefab;

    #region Header
    [Header("The Upper Left Position To Start Copying From")]
    #endregion
    public Vector2Int DoorwayStartCopyPosition;
    
    #region Header
    [Header("The Width Of Tiles In The Doorway To Copy Over")]
    #endregion
    public int DoorwayCopyTileWidth;
    
    #region Header
    [Header("The Height Of Tiles In The Doorway To Copy Over")]
    #endregion
    public int DoorwayCopyTileHeight;

    [HideInInspector] public bool IsConnected = false;
    
    [HideInInspector] public bool IsUnavailable = false;
    
}

public static class DoorwayHelper
{
    public static bool IsDoorWayInRoomOppositeToParentDoorway(this Doorway doorway) => doorway == null;
    
    public static bool IsNoMoreDoorwaysToTryThenOverlapFailure(this IReadOnlyList<Doorway> unconnectedAvailableParentDoorways) =>
        unconnectedAvailableParentDoorways.Count == 0;
    
    public static Vector2Int DetermineUpperBoundRelativeToParentDoorway(this Room room) =>
        room.LowerBounds + room.TemplateUpperBounds - room.TemplateLowerBounds;
    
    /// <summary>
    /// Create deep copy of doorway list
    /// </summary>
    public static List<Doorway> CopyDoorwayList(this IEnumerable<Doorway> oldDoorwayList) =>
        oldDoorwayList.Select(doorway => new Doorway
            {
                Position = doorway.Position,
                Orientation = doorway.Orientation,
                DoorPrefab = doorway.DoorPrefab,
                IsConnected = doorway.IsConnected,
                IsUnavailable = doorway.IsUnavailable,
                DoorwayStartCopyPosition = doorway.DoorwayStartCopyPosition,
                DoorwayCopyTileWidth = doorway.DoorwayCopyTileWidth,
                DoorwayCopyTileHeight = doorway.DoorwayCopyTileHeight
            })
            .ToList();
    
    /// <summary>
    /// Get unconnected doorways
    /// </summary>
    public static IEnumerable<Doorway> GetUnconnectedAvailableDoorways(this IEnumerable<Doorway> roomDoorwayList) => 
        roomDoorwayList.Where(doorway => !doorway.IsConnected && !doorway.IsUnavailable);
    
    /// <summary>
    /// Get the doorway from the doorway list that has the opposite orientation to doorway
    /// </summary>
    public static Doorway GetOppositeOrientationDoorway(this Doorway parentDoorway, List<Doorway> doorwayList)
    {
        foreach (var doorwayToCheck in doorwayList)
        {
            switch (parentDoorway.Orientation)
            {
                case Orientation.East when doorwayToCheck.Orientation == Orientation.West:
                    return doorwayToCheck;
                case Orientation.West when doorwayToCheck.Orientation == Orientation.East:
                    return doorwayToCheck;
                case Orientation.North when doorwayToCheck.Orientation == Orientation.South:
                    return doorwayToCheck;
                case Orientation.South when doorwayToCheck.Orientation == Orientation.North:
                    return doorwayToCheck;
            }
        }

        return null;
    }
    
    /// <summary>
    ///  Calculate adjustment position offset based on room doorway position that we are trying to connect (e.g. if this doorway is west then we need to add (1,0) to the east parent doorway)
    /// </summary>
    /// <param name="doorway"></param>
    public static Vector2Int DoorPositionAdjustment(this Doorway doorway)
    {
        switch (doorway.Orientation)
        {
            case Orientation.North:
                return new Vector2Int(0, -1);
            
            case Orientation.East:
                return new Vector2Int(-1, 0);
            
            case Orientation.South:
                return new Vector2Int(0, 1);
            
            case Orientation.West:
                return new Vector2Int(1, 0);
            
            case Orientation.None:
                return new Vector2Int(0, 0);

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    #region PlaceTheRoomHelper
    
    public static bool MarkDoorwayUnavailableAndDontConnect(Doorway doorwayParent)
    {
        doorwayParent.IsUnavailable = true;
        return false;
    }

    public static bool MarkDoorwaysAsConnected(Doorway doorway, Doorway doorwayParent)
    {
        doorway.IsConnected = true;
        doorway.IsUnavailable = true;
        
        doorwayParent.IsConnected = true;
        doorwayParent.IsUnavailable = true;
        
        return true;
    }
    
    #endregion
}