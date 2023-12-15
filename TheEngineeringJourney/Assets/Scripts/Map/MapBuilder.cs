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
    private Dictionary<string, RoomTemplateSO> roomTemplateDictionary = new();
    private RoomNodeTypeListSO _roomNodeTypes;
    
    private enum Attempt
    {
        Failed,
        Success,
        Building,
        RetryBuild,
        RetryCreateNodeGraph,
        CreateNodeGraph
    }

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
        _roomNodeTypes = GameResources.Instance.RoomNodeTypes;
    }

    /// <summary>
    /// Generate random dungeon, returns true if dungeon built, false if failed
    /// </summary>
    public Build GenerateMap(MapLevelSO currentDungeonLevel)
    {
        var status = Attempt.CreateNodeGraph;
        var buildAttempts = 0;
        var rebuildNodeGraph = 0;
        var roomTemplates = currentDungeonLevel.RoomTemplates;
        // This load something please don't delete me!!!
        LoadRoomTemplatesIntoDictionary(roomTemplates);
        // Select a random room node graph from the list
        var roomNodeGraph = currentDungeonLevel.RoomNodeGraphs.SelectRandomRoomNodeGraph();
        
        while (true)
        {
            switch (status)
            {
                case Attempt.CreateNodeGraph:
                    ClearMap(MapBuilderRoomDictionary);
                    ++rebuildNodeGraph;
                    status = AttemptToBuildRandomMap(roomNodeGraph, MapBuilderRoomDictionary, roomTemplates, _roomNodeTypes);
                    break;
                case Attempt.RetryCreateNodeGraph:
                    status = rebuildNodeGraph == Settings.MaxMapRebuildAttemptsForRoomGraph
                        ? Attempt.RetryBuild
                        : Attempt.Building;
                    break;
                case Attempt.RetryBuild:
                    status = buildAttempts == Settings.MaxMapBuildAttempts
                        ? Attempt.Failed
                        : Attempt.Building;
                    break;
                case Attempt.Building:
                    roomNodeGraph = currentDungeonLevel.RoomNodeGraphs.SelectRandomRoomNodeGraph();
                    rebuildNodeGraph = 0;
                    ++buildAttempts;
                    status = Attempt.CreateNodeGraph;
                    break;
                case Attempt.Failed:
                    return Build.Failed;
                case Attempt.Success:
                    InstantiateRoomGameObjects(MapBuilderRoomDictionary);
                    return Build.Success;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            //Debug.Log(status.ToString());
        }
        
    }

    /// <summary>
    /// Attempt to randomly build the dungeon for the specified room nodeGraph. Returns true if a
    /// successful random layout was generated, else returns false if a problem was encountered and
    /// another attempt is required.
    /// </summary>
    private Attempt AttemptToBuildRandomMap(RoomNodeGraphSO roomNodeGraph, Dictionary<string, Room> mapBuilderRoomDictionary, 
        IReadOnlyCollection<RoomTemplateSO> roomTemplates, RoomNodeTypeListSO roomNodeTypes)
    {
        // Create Open Room Node Queue
        var openRoomNodeQueue = new Queue<RoomNodeSO>();
        
        // Add Entrance Node To Room Node Queue From Room Node Graph
        var entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypes.RoomNodeTypes.Find(x => x.isEntrance));

        if (entranceNode.IsEntranceDebug()) return Attempt.RetryBuild;

        openRoomNodeQueue.Enqueue(entranceNode);
        
        // Process open room nodes queue
        var noRoomOverlaps = ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, mapBuilderRoomDictionary, roomTemplates, roomNodeTypes);
        
        // If all the room nodes have been processed and there hasn't been a room overlap then return true
        return openRoomNodeQueue.IsQueueEmpty() && noRoomOverlaps is RoomOverlapping.Overlapping
            ? Attempt.RetryBuild
            : Attempt.Success;
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
    private RoomOverlapping ProcessRoomsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, 
        Dictionary<string, Room> mapBuilderRoomDictionary, 
        IReadOnlyCollection<RoomTemplateSO> roomTemplates, RoomNodeTypeListSO roomNodeTypes)
    {
        var noRoomOverlaps = RoomOverlapping.Attempt;
        // While room nodes in open room node queue & no room overlaps detected.
        while (openRoomNodeQueue.Count > 0 && noRoomOverlaps is not RoomOverlapping.Overlapping)
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

    /// <summary>
    /// Entrance can't overlap, because non other exists
    /// </summary>
    private static RoomOverlapping CanPlaceEntrance(RoomNodeSO roomNode, IDictionary<string, Room> mapBuilderRoomDictionary, IEnumerable<RoomTemplateSO> roomTemplates)
    {
        var roomTemplate = roomNode.roomNodeType.GetRandomRoomTemplate(roomTemplates);

        var room = roomTemplate.CreateRoomFromRoomTemplate(roomNode);

        room.IsPositioned = true;
        
        mapBuilderRoomDictionary.Add(room.Id, room);

        return RoomOverlapping.Contiguous;
    }

    /// <summary>
    /// Attempt to place the room node in the map - if room can be placed return the room, else return null
    /// </summary>
    private RoomOverlapping CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom, Dictionary<string, Room> mapBuilderRoomDictionary, IReadOnlyCollection<RoomTemplateSO> roomTemplateList, RoomNodeTypeListSO roomNodeTypes)
    {
        var roomOverlaps = RoomOverlapping.Overlapping;
        
        while (roomOverlaps is RoomOverlapping.Overlapping or RoomOverlapping.Attempt)
        {
            var unconnectedAvailableParentDoorways = parentRoom.DoorWayList.GetUnconnectedAvailableDoorways().ToList();

            if (unconnectedAvailableParentDoorways.IsNoMoreDoorwaysToTryThenOverlapFailure()) return RoomOverlapping.Contiguous; 
            
            var doorwayParent = unconnectedAvailableParentDoorways[Random.Range(0, unconnectedAvailableParentDoorways.Count)];

            var roomTemplate = GetRandomTemplateForRoomConsistentWithParent(roomNode, doorwayParent, roomTemplateList, roomNodeTypes);

            var room = roomTemplate.CreateRoomFromRoomTemplate(roomNode);
            
            if (!PlaceTheRoom(parentRoom, doorwayParent, room, mapBuilderRoomDictionary)) continue;
            
            roomOverlaps = RoomOverlapping.Contiguous;
            room.IsPositioned = true;
            mapBuilderRoomDictionary.Add(room.Id, room);
        }

        return roomOverlaps; 
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
        
        if (doorway.IsDoorWayInRoomOppositeToParentDoorway()) return DoorwayHelper.MarkDoorwayUnavailableAndDontConnect(doorwayParent);

        // Calculate 'world' grid parent doorway position
        var parentDoorwayPosition = parentRoom.GridOfParentDoorway(doorwayParent);
        
        // Calculate room lower bounds and upper bounds based on positioning to align with parent doorway
        room.LowerBounds = parentDoorwayPosition + doorway.DoorPositionAdjustment() + room.TemplateLowerBounds - doorway.Position;
        room.UpperBounds = room.LowerBounds + room.TemplateUpperBounds - room.TemplateLowerBounds;
        
        return mapBuilderRoomDictionary.CheckForRoomOverlap(room) 
            ? DoorwayHelper.MarkDoorwaysAsConnected(doorway, doorwayParent) 
            : DoorwayHelper.MarkDoorwayUnavailableAndDontConnect(doorwayParent);
    }


    /// <summary>
    /// Get a room template by room template ID, returns null if ID doesn't exist
    /// </summary>
    public RoomTemplateSO GetRoomTemplate(string roomTemplateID) =>
        roomTemplateDictionary.TryGetValue(roomTemplateID, out var roomTemplate) ? roomTemplate : null;

    /// <summary>
    /// Get room by roomID, if no room exists with that ID return null
    /// </summary>
    public Room GetRoomByRoomID(string roomID) =>
        MapBuilderRoomDictionary.TryGetValue(roomID, out var room) ? room : null;
   
    /// <summary>
    /// Load the room templates into the dictionary
    /// </summary>
    private void LoadRoomTemplatesIntoDictionary(IEnumerable<RoomTemplateSO> roomTemplates)
    {
        // Clear room template dictionary
        roomTemplateDictionary.Clear();

        // Load room template list into dictionary
        foreach (var roomTemplate in roomTemplates)
        {
            if (!roomTemplateDictionary.ContainsKey(roomTemplate.Guid))
            {
                roomTemplateDictionary.Add(roomTemplate.Guid, roomTemplate);
            }
            else
            {
                Debug.Log("Duplicate Room Template Key In " + roomTemplates);
            }
        }
    }
}
#endregion

#region Extension

public static class MapBuilderExtensions
{
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
        
    #region RoomTemplate
    
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

    public static Vector2Int GridOfParentDoorway(this Room parentRoom, Doorway doorwayParent) =>
        parentRoom.LowerBounds + doorwayParent.Position - parentRoom.TemplateLowerBounds;
    
    #endregion
}

#endregion