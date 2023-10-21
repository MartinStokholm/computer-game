using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public class MapBuilder : SingletonMonobehaviour<MapBuilder>
{
    public readonly Dictionary<string, Room> MapBuilderRoomDictionary = new();
    private RoomNodeTypeListSO _roomNodeTypeList;

    protected override void Awake()
    {
        base.Awake();

        // Load the room node type list
        LoadRoomNodeTypeList();

        // Set dimmed material to fully visible
        GameResources.Instance.DimmedMaterial.SetFloat("Alpha_Slider", 1f);
    }

    /// <summary>
    /// Load the room node type list
    /// </summary>
    private void LoadRoomNodeTypeList()
    {
        _roomNodeTypeList = GameResources.Instance.RoomNodeTypes;
    }

    /// <summary>
    /// Generate random dungeon, returns true if dungeon built, false if failed
    /// </summary>
    public bool GenerateMap(MapLevelSO currentDungeonLevel)
    {
        var dungeonBuildSuccessful = false;
        var dungeonBuildAttempts = 0;
        var dungeonRebuildAttemptsForNodeGraph = 0;
        var roomTemplateList = currentDungeonLevel.RoomTemplates;
        
        while (!dungeonBuildSuccessful && dungeonBuildAttempts < Settings.MaxMapBuildAttempts)
        {
            dungeonBuildAttempts++;
            // Select a random room node graph from the list
            var roomNodeGraph = currentDungeonLevel.RoomNodeGraphs.SelectRandomRoomNodeGraph();
            
            // Loop until dungeon successfully built or more than max attempts for node graph
            while (!dungeonBuildSuccessful && dungeonRebuildAttemptsForNodeGraph <= Settings.MaxMapBuildAttempts)
            {
                // Clear dungeon room game objects and dungeon room dictionary
                ClearMap(MapBuilderRoomDictionary);
                
                dungeonRebuildAttemptsForNodeGraph++;

                // Attempt To Build A Random Dungeon For The Selected room node graph
                dungeonBuildSuccessful = AttemptToBuildRandomMap(roomNodeGraph, MapBuilderRoomDictionary, roomTemplateList, _roomNodeTypeList);
            }


            if (dungeonBuildSuccessful)
            {
                // Instantiate Room Game Objects
                InstantiateRoomGameObjects(MapBuilderRoomDictionary);
            }
        }

        return dungeonBuildSuccessful;
    }
    
    /// <summary>
    /// Attempt to randomly build the dungeon for the specified room nodeGraph. Returns true if a
    /// successful random layout was generated, else returns false if a problem was encountered and
    /// another attempt is required.
    /// </summary>
    private bool AttemptToBuildRandomMap(RoomNodeGraphSO roomNodeGraph, Dictionary<string, Room> mapBuilderRoomDictionary, 
        IReadOnlyCollection<RoomTemplateSO> roomTemplates, RoomNodeTypeListSO roomNodeTypes)
    {
        // Create Open Room Node Queue
        var openRoomNodeQueue = new Queue<RoomNodeSO>();
        
        // Add Entrance Node To Room Node Queue From Room Node Graph
        var entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypes.RoomNodeTypes.Find(x => x.isEntrance));

        if (entranceNode.IsEntranceDebug()) return false;

        openRoomNodeQueue.Enqueue(entranceNode);
        
        // Process open room nodes queue
        var noRoomOverlaps = ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, mapBuilderRoomDictionary, roomTemplates, roomNodeTypes);

        // If all the room nodes have been processed and there hasn't been a room overlap then return true
        return openRoomNodeQueue.Count == 0 && noRoomOverlaps;
    }
    
     
    /// <summary>
    /// Instantiate the dungeon room game objects from the prefabs
    /// </summary>
    private void InstantiateRoomGameObjects(Dictionary<string, Room>  mapBuilderRoomDictionary)
    {
        // Iterate through all dungeon rooms.
        foreach (var keyValuePair in mapBuilderRoomDictionary)
        {
            var room = keyValuePair.Value;
            
            var roomGameObject = Instantiate(room.Prefab, room.CalculateAdjustedRoomPosition(), Quaternion.identity, transform);

            // Get instantiated room component from instantiated prefab.
            var instantiatedRoom = roomGameObject.GetComponentInChildren<InstantiatedRoom>();

            instantiatedRoom.Room = room;
            
            instantiatedRoom.Initialise(roomGameObject);
            
            room.InstantiatedRoom = instantiatedRoom;
        }
    }
    
    /// <summary>
    /// Clear dungeon room game objects and dungeon room dictionary
    /// </summary>
    private static void ClearMap(Dictionary<string, Room> mapBuilderRoomDictionary)
    {
        if (mapBuilderRoomDictionary.Count <= 0) return;
        
        foreach (var room in mapBuilderRoomDictionary
                     .Select(keyValuePair => keyValuePair.Value)
                     .Where(room => room.InstantiatedRoom is not null))
        {
            Destroy(room.InstantiatedRoom.gameObject);
        }
        
        mapBuilderRoomDictionary.Clear();
    }

    #region Static 
    /// <summary>
    /// Process rooms in the open room node queue, returning true if there are no room overlaps
    /// </summary>
    private bool ProcessRoomsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, 
        Dictionary<string, Room> mapBuilderRoomDictionary, 
        IReadOnlyCollection<RoomTemplateSO> roomTemplates, RoomNodeTypeListSO roomNodeTypes)
    {
        var noRoomOverlaps = true;
        // While room nodes in open room node queue & no room overlaps detected.
        while (openRoomNodeQueue.Count > 0 && noRoomOverlaps)
        {
            // Get next room node from open room node queue.
            var roomNode = openRoomNodeQueue.Dequeue();
            
            // Add child Nodes to queue from room node graph (with links to this parent Room)
            roomNodeGraph
                .GetChildRoomNodes(roomNode)
                .ToList()
                .ForEach(openRoomNodeQueue.Enqueue);
            
            noRoomOverlaps = roomNode.roomNodeType.isEntrance 
                ? CanPlaceEntrance(roomNode, mapBuilderRoomDictionary, roomTemplates)
                : CanPlaceRoomWithNoOverlaps(roomNode, mapBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]], mapBuilderRoomDictionary , roomTemplates, roomNodeTypes);
        }

        return noRoomOverlaps;
    }

    private static bool CanPlaceEntrance(RoomNodeSO roomNode, IDictionary<string, Room> mapBuilderRoomDictionary, IEnumerable<RoomTemplateSO> roomTemplates)
    {
        var roomTemplate = roomNode.roomNodeType.GetRandomRoomTemplate(roomTemplates);

        var room = roomTemplate.CreateRoomFromRoomTemplate(roomNode);

        room.IsPositioned = true;
        
        mapBuilderRoomDictionary.Add(room.Id, room);

        return true;
    }

    /// <summary>
    /// Attempt to place the room node in the map - if room can be placed return the room, else return null
    /// </summary>
    private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom, Dictionary<string, Room> mapBuilderRoomDictionary, IReadOnlyCollection<RoomTemplateSO> roomTemplateList, RoomNodeTypeListSO roomNodeTypes)
    {
        var roomOverlaps = true;
        
        while (roomOverlaps)
        {
            var unconnectedAvailableParentDoorways = parentRoom.DoorWayList.GetUnconnectedAvailableDoorways().ToList();

            if (unconnectedAvailableParentDoorways.IsNoMoreDoorwaysToTryThenOverlapFailure()) return false; 
            
            var doorwayParent = unconnectedAvailableParentDoorways[Random.Range(0, unconnectedAvailableParentDoorways.Count)];

            var roomTemplate = GetRandomTemplateForRoomConsistentWithParent(roomNode, doorwayParent, roomTemplateList, roomNodeTypes);

            var room = roomTemplate.CreateRoomFromRoomTemplate(roomNode);
            
            if (!PlaceTheRoom(parentRoom, doorwayParent, room, mapBuilderRoomDictionary)) continue;
            
            roomOverlaps = false;
            room.IsPositioned = true;
            mapBuilderRoomDictionary.Add(room.Id, room);
        }

        return true; 
    }

    /// <summary>
    /// Get random room template for room node taking into account the parent doorway orientation.
    /// </summary>
    private static RoomTemplateSO GetRandomTemplateForRoomConsistentWithParent(RoomNodeSO roomNode, Doorway doorwayParent, IEnumerable<RoomTemplateSO> roomTemplateList, RoomNodeTypeListSO roomNodeTypes) => roomNode.roomNodeType.isCorridor 
            ? roomNodeTypes.GetCorridorOrientation(doorwayParent.Orientation, roomTemplateList) 
            : roomNode.roomNodeType.GetRandomRoomTemplate(roomTemplateList);


    /// <summary>
    /// Place the room - returns true if the room doesn't overlap, false otherwise
    /// </summary>
    private static bool PlaceTheRoom(Room parentRoom, Doorway doorwayParent, Room room, Dictionary<string, Room> mapBuilderRoomDictionary)
    {
        var doorway = doorwayParent.GetOppositeOrientationDoorway(room.DoorWayList);
        
        if (doorway.IsDoorWayInRoomOppositeToParentDoorway()) return MarkDoorwayUnavailableAndDontConnect(doorwayParent);

        // Calculate 'world' grid parent doorway position
        var parentDoorwayPosition = parentRoom.GridOfParentDoorway(doorwayParent);
        
        // Calculate room lower bounds and upper bounds based on positioning to align with parent doorway
        room.LowerBounds = parentDoorwayPosition + doorway.DoorPositionAdjustment() + room.TemplateLowerBounds - doorway.Position;
        room.UpperBounds = room.LowerBounds + room.TemplateUpperBounds - room.TemplateLowerBounds;
        
        return mapBuilderRoomDictionary.CheckForRoomOverlap(room) 
            ? MarkDoorwaysAsConnected(doorway, doorwayParent) 
            : MarkDoorwayUnavailableAndDontConnect(doorwayParent);
    }

    #region PlaceTheRoomHelper
    
    private static bool MarkDoorwayUnavailableAndDontConnect(Doorway doorwayParent)
    {
        doorwayParent.IsUnavailable = true;
        return false;
    }

    private static bool MarkDoorwaysAsConnected(Doorway doorway, Doorway doorwayParent)
    {
        doorway.IsConnected = true;
        doorway.IsUnavailable = true;
        
        doorwayParent.IsConnected = true;
        doorwayParent.IsUnavailable = true;
        
        return true;
    }
    
    #endregion

   
}
#endregion

#region Extension

public static class MapBuilderExtensions
{
    private static (IEnumerable<RoomTemplateSO> uniqueTemplates, IEnumerable<RoomTemplateSO> duplicates) 
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
    
    [CanBeNull]
    public static RoomTemplateSO GetCorridorOrientation(this RoomNodeTypeListSO roomNodeTypes, Orientation orientation, IEnumerable<RoomTemplateSO> roomTemplateList)
    {
        switch (orientation)
        {
            case Orientation.North:
            case Orientation.South:
                return roomNodeTypes.RoomNodeTypes.Find(x => x.isCorridorNS).GetRandomRoomTemplate(roomTemplateList);
            
            case Orientation.East:
            case Orientation.West:
                return roomNodeTypes.RoomNodeTypes.Find(x => x.isCorridorEW).GetRandomRoomTemplate(roomTemplateList);
            
            case Orientation.None:
                break;
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
    
    /// <summary>
    /// Select a random room node graph from the list of room node graphs
    /// </summary>
    public static RoomNodeGraphSO SelectRandomRoomNodeGraph(this IReadOnlyList<RoomNodeGraphSO> roomNodeGraphList)
    {
        if (roomNodeGraphList.Count > 0) return roomNodeGraphList[Random.Range(0, roomNodeGraphList.Count)];
        
        Debug.Log("No room node graphs in list");
        return null;
    }

    /// <summary>
    /// Get unconnected doorways
    /// </summary>
    public static IEnumerable<Doorway> GetUnconnectedAvailableDoorways(this IEnumerable<Doorway> roomDoorwayList) => 
        roomDoorwayList.Where(doorway => !doorway.IsConnected && !doorway.IsUnavailable);

    public static bool IsDoorWayInRoomOppositeToParentDoorway(this Doorway doorway) => doorway == null;
    
    /// <summary>
    /// Create deep copy of doorway list
    /// </summary>
    private static List<Doorway> CopyDoorwayList(this IEnumerable<Doorway> oldDoorwayList) =>
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
    /// Get a room template by room template ID, returns null if ID doesn't exist
    /// </summary>
    public static RoomTemplateSO GetRoomTemplate(this Dictionary<string, RoomTemplateSO> roomTemplateDictionary, string roomTemplateID) =>
        roomTemplateDictionary.TryGetValue(roomTemplateID, out var roomTemplate) ? roomTemplate : null;

    /// <summary>
    /// Get room by roomID, if no room exists with that ID return null
    /// </summary>
    public static Room GetRoomByRoomID(this Dictionary<string, Room> mapBuilderRoomDictionary, string roomID) =>
        mapBuilderRoomDictionary.TryGetValue(roomID, out var room) ? room : null;
    
    
    private static bool IsEntrance(this RoomNodeSO roomNode) => roomNode.parentRoomNodeIDList.Count == 0;
    
    /// <summary>
    /// Check if 2 rooms overlap each other - return true if they overlap or false if they don't overlap
    /// </summary>
    private static bool IsOverLappingRoom(this Room room1, Room room2)
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

    #region MapBuilderRoomDictionary

    /// <summary>
    /// Check for rooms that overlap the upper and lower bounds parameters, and if there are overlapping rooms then return room else return null
    /// </summary>
    public static bool CheckForRoomOverlap(this IDictionary<string, Room> mapBuilderRoomDictionary, Room roomToTest)
    {
        var room = mapBuilderRoomDictionary
            .Select(keyValuePair => keyValuePair.Value)
            .Where(room => room.Id != roomToTest.Id && room.IsPositioned)
            .FirstOrDefault(roomToTest.IsOverLappingRoom);
        
        return room == null;
    }

        #endregion
    
    
    public static bool IsNoMoreDoorwaysToTryThenOverlapFailure(this IReadOnlyList<Doorway> unconnectedAvailableParentDoorways) =>
        unconnectedAvailableParentDoorways.Count == 0;
    

    #region RoomTemplate
    
    /// <summary>
    /// Get a random room template from the room template list that matches the roomType and return it
    /// (return null if no matching room templates found).
    /// </summary>
    public static RoomTemplateSO GetRandomRoomTemplate(this RoomNodeTypeSO roomNodeType, IEnumerable<RoomTemplateSO> roomTemplateList)
    {
        // Loop through room template list
        var matchingRoomTemplateList = roomTemplateList
            .Where(x => x.RoomNodeType == roomNodeType)
            .ToList();
        
        // Return null if list is zero or Select random room template from list and return
        return matchingRoomTemplateList.Count is not 0
            ? matchingRoomTemplateList[Random.Range(0, matchingRoomTemplateList.Count)]
            : null;
    }

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
    /// Load the room templates into the dictionary
    /// </summary>
    public static Dictionary<string, RoomTemplateSO> LoadRoomTemplatesIntoDictionary(this IEnumerable<RoomTemplateSO> roomTemplateList, Dictionary<string, RoomTemplateSO> roomTemplateDictionary)
    {
        var (uniqueTemplates, duplicates) = roomTemplateList.GetSeperatedRoomTemplates();
        
        duplicates
            .ToList()
            .ForEach(x => Debug.Log("Duplicate Room Template Key In " + x));

        uniqueTemplates
            .ToList()
            .ForEach(roomTemplate => 
            {
                roomTemplateDictionary.Add(roomTemplate.Guid, roomTemplate);
            });
        
        return roomTemplateDictionary;
    }
    
    public static bool IsEntranceDebug(this RoomNodeSO entranceNode)
    {
        if (entranceNode is not null) return false;
        
        Debug.Log("No Entrance Node");
        return true;
    }
    
    /// <summary>
    /// Calculate room position (remember the room instantiation position needs to be adjusted by the room template lower bounds)
    /// </summary>
    /// <returns></returns>
    public static Vector3 CalculateAdjustedRoomPosition(this Room room) =>
        new(room.LowerBounds.x - room.TemplateLowerBounds.x, room.LowerBounds.y - room.TemplateLowerBounds.y, 0f);

    public static Vector2Int GridOfParentDoorway(this Room parentRoom, Doorway doorwayParent) =>
        parentRoom.LowerBounds + doorwayParent.Position - parentRoom.TemplateLowerBounds;
    
    public static Vector2Int DetermineUpperBoundRelativeToParentDoorway(this Room room) =>
        room.LowerBounds + room.TemplateUpperBounds - room.TemplateLowerBounds;
    #endregion
}

#endregion