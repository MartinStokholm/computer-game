﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public string Id;
    public string TemplateID;
    public GameObject Prefab;
    public RoomNodeTypeSO RoomNodeType;
    public Vector2Int LowerBounds;
    public Vector2Int UpperBounds;
    public Vector2Int TemplateLowerBounds;
    public Vector2Int TemplateUpperBounds;
    public Vector2Int[] SpawnPositionArray;
    public List<string> ChildRoomIDList;
    public string ParentRoomID;
    public List<Doorway> DoorWayList;
    public bool IsPositioned = false;
    public InstantiatedRoom InstantiatedRoom;
    public bool IsLit = false;
    public bool IsClearedOfEnemies = false;
    public bool IsPreviouslyVisited = false;

    public Room()
    {
        ChildRoomIDList = new List<string>();
        DoorWayList = new List<Doorway>();
    }

}

public static class RoomNodeHelper
{
    /// <summary>
    /// Create room based on roomTemplate and layoutNode, and return the created room
    /// </summary>
    public static Room CreateRoomFromRoomTemplate(this RoomTemplateSO roomTemplate, RoomNodeSO roomNode)
    {
        var room = roomTemplate.CreateRoom(roomNode);

        return roomNode.IsEntrance()
            ? room.SetEntrance()
            : room.SetRoom(roomNode);
    }

    private static Room CreateRoom(this RoomTemplateSO roomTemplate, RoomNodeSO roomNode) =>
        new()
        {
            TemplateID = roomTemplate.Guid,
            Id = roomNode.Id,
            Prefab = roomTemplate.Prefab,
            RoomNodeType = roomTemplate.RoomNodeType,
            LowerBounds = roomTemplate.LowerBounds,
            UpperBounds = roomTemplate.UpperBounds,
            SpawnPositionArray = roomTemplate.SpawnPositions,
            TemplateLowerBounds = roomTemplate.LowerBounds,
            TemplateUpperBounds = roomTemplate.UpperBounds,
            ChildRoomIDList = roomNode.childRoomNodeIDs.CopyStringList(),
            DoorWayList = roomTemplate.Doorways.CopyDoorwayList()
        };
    
    private static Room SetEntrance(this Room room)
    {
        room.ParentRoomID = "";
        room.IsPreviouslyVisited = true;
        
        GameManager.Instance.CurrentRoom = room;
        return room;
    }
    
    private static Room SetRoom(this Room room, RoomNodeSO roomNode)
    {
        room.ParentRoomID = roomNode.parentRoomNodeIDList[0];
        return room;
    }
    
    /// <summary>
    /// Check if 2 rooms overlap each other - return true if they overlap or false if they don't overlap
    /// </summary>
    public static bool IsOverLappingRoom(this Room room1, Room room2)
    {
        var isOverlappingX = IsOverLappingInterval(room1.LowerBounds.x, room1.UpperBounds.x, room2.LowerBounds.x, room2.UpperBounds.x);
        var isOverlappingY = IsOverLappingInterval(room1.LowerBounds.y, room1.UpperBounds.y, room2.LowerBounds.y, room2.UpperBounds.y);

        return isOverlappingX && isOverlappingY;
    }
    
    /// <summary>
    /// Check if interval 1 overlaps interval 2 - this method is used by the IsOverlappingRoom method
    /// </summary>
    private static bool IsOverLappingInterval(int iMin1, int iMax1, int iMin2, int iMax2) =>
        Mathf.Max(iMin1, iMin2) <= Mathf.Min(iMax1, iMax2);
    
    /// <summary>
    /// Calculate room position (remember the room instantiation position needs to be adjusted by the room template lower bounds)
    /// </summary>
    /// <returns></returns>
    public static Vector3 CalculateAdjustedRoomPosition(this Room room) =>
        new(room.LowerBounds.x - room.TemplateLowerBounds.x, room.LowerBounds.y - room.TemplateLowerBounds.y, 0f);
}