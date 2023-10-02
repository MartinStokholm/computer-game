using System.Collections;
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