using System.Collections.Generic;
using System.Linq;
using System.Xml;
using JetBrains.Annotations;
using UnityEngine;

[DisallowMultipleComponent]
public class MapBuilder : SingletonMonobehaviour<MapBuilder>
{
    public readonly Dictionary<string, Room> MapBuilderRoomDictionary = new();
    private readonly Dictionary<string, RoomTemplateSO> _roomTemplateDictionary = new();
    private List<RoomTemplateSO> _roomTemplateList;
    private RoomNodeTypeListSO _roomNodeTypes;
    private bool _mapBuildSuccessful;

    protected override void Awake()
    {
        base.Awake();

        // Load the room node type list
        LoadRoomNodeTypeList();
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
    public bool GenerateMap(MapLevelSO currentMapLevel)
    {
        _roomTemplateList = currentMapLevel.RoomTemplates;

        // Load the scriptable object room templates into the dictionary
        LoadRoomTemplatesIntoDictionary();

        _mapBuildSuccessful = false;
        var dungeonBuildAttempts = 0;

        while (!_mapBuildSuccessful && dungeonBuildAttempts < Settings.MaxMapBuildAttempts)
        {
            dungeonBuildAttempts++;

            // Select a random room node graph from the list
            RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentMapLevel.RoomNodeGraphs);

            var dungeonRebuildAttemptsForNodeGraph = 0;
            _mapBuildSuccessful = false;

            // Loop until dungeon successfully built or more than max attempts for node graph
            while (!_mapBuildSuccessful && dungeonRebuildAttemptsForNodeGraph <= Settings.MaxMapRebuildAttemptsForRoomGraph)
            {
                // Clear dungeon room game objects and dungeon room dictionary
                ClearDungeon();

                dungeonRebuildAttemptsForNodeGraph++;

                // Attempt To Build A Random Dungeon For The Selected room node graph
                _mapBuildSuccessful = AttemptToBuildRandomDungeon(roomNodeGraph);
            }


            if (_mapBuildSuccessful)
            {
                // Instantiate Room Game objects
                InstantiateRoomGameobjects();
            }
        }

        return _mapBuildSuccessful;
    }

    /// <summary>
    /// Load the room templates into the dictionary
    /// </summary>
    private void LoadRoomTemplatesIntoDictionary()
    {
        // Clear room template dictionary
        _roomTemplateDictionary.Clear();
        
        // Load room template list into dictionary
        foreach (var roomTemplate in _roomTemplateList)
        {
            if (!_roomTemplateDictionary.ContainsKey(roomTemplate.Guid))
            {
                _roomTemplateDictionary.Add(roomTemplate.Guid, roomTemplate);
            }
            else
            {
                Debug.Log("Duplicate Room Template Key In " + _roomTemplateList);
            }
        }
    }

    /// <summary>
    /// Attempt to randomly build the dungeon for the specified room nodeGraph. Returns true if a
    /// successful random layout was generated, else returns false if a problem was encoutered and
    /// another attempt is required.
    /// </summary>
    private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)
    {

        // Create Open Room Node Queue
        var openRoomNodeQueue = new Queue<RoomNodeSO>();

        // Add Entrance Node To Room Node Queue From Room Node Graph
        var entranceNode = roomNodeGraph.GetRoomNode(_roomNodeTypes.RoomNodeTypes.Find(x => x.isEntrance));

        if (entranceNode is not null)
        {
            openRoomNodeQueue.Enqueue(entranceNode);
        }
        else
        {
            Debug.Log("No Entrance Node");
            return false;  // Dungeon Not Built
        }

        // Start with no room overlaps
        var noRoomOverlaps = true;


        // Process open room nodes queue
        noRoomOverlaps = ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);

        // If all the room nodes have been processed and there hasn't been a room overlap then return true
        return openRoomNodeQueue.Count == 0 && noRoomOverlaps;
    }

    /// <summary>
    /// Process rooms in the open room node queue, returning true if there are no room overlaps
    /// </summary>
    private bool ProcessRoomsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps)
    {

        // While room nodes in open room node queue & no room overlaps detected.
        while (openRoomNodeQueue.Count > 0 && noRoomOverlaps)
        {
            // Get next room node from open room node queue.
            var roomNode = openRoomNodeQueue.Dequeue();

            // Add child Nodes to queue from room node graph (with links to this parent Room)
            foreach (var childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode))
            {
                openRoomNodeQueue.Enqueue(childRoomNode);
            }

            // if the room is the entrance mark as positioned and add to room dictionary
            if (roomNode.roomNodeType.isEntrance)
            {
                var roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);

                var room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

                room.IsPositioned = true;

                // Add room to room dictionary
                MapBuilderRoomDictionary.Add(room.Id, room);
            }

            // else if the room type isn't an entrance
            else
            {
                // Else get parent room for node
                var parentRoom = MapBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];

                // See if room can be placed without overlaps
                noRoomOverlaps = CanPlaceRoomWithNoOverlaps(roomNode, parentRoom);
            }

        }

        return noRoomOverlaps;
    }


    /// <summary>
    /// Attempt to place the room node in the dungeon - if room can be placed return the room, else return null
    /// </summary>
    private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom)
    {

        // initialise and assume overlap until proven otherwise.
        var roomOverlaps = true;

        // Do While Room Overlaps - try to place against all available doorways of the parent until
        // the room is successfully placed without overlap.
        while (roomOverlaps)
        {
            // Select random unconnected available doorway for Parent
            var unconnectedAvailableParentDoorways = GetUnconnectedAvailableDoorways(parentRoom.DoorWayList).ToList();

            if (unconnectedAvailableParentDoorways.Count == 0)
            {
                // If no more doorways to try then overlap failure.
                return false; // room overlaps
            }

            var doorwayParent = unconnectedAvailableParentDoorways[Random.Range(0, unconnectedAvailableParentDoorways.Count)];

            // Get a random room template for room node that is consistent with the parent door orientation
            var roomtemplate = GetRandomTemplateForRoomConsistentWithParent(roomNode, doorwayParent);

            // Create a room
            var room = CreateRoomFromRoomTemplate(roomtemplate, roomNode);

            // Place the room - returns true if the room doesn't overlap
            if (PlaceTheRoom(parentRoom, doorwayParent, room))
            {
                // If room doesn't overlap then set to false to exit while loop
                roomOverlaps = false;

                // Mark room as positioned
                room.IsPositioned = true;

                // Add room to dictionary
                MapBuilderRoomDictionary.Add(room.Id, room);

            }
        }

        return true;  // no room overlaps

    }

    /// <summary>
    /// Get random room template for room node taking into account the parent doorway orientation.
    /// </summary>
    private RoomTemplateSO GetRandomTemplateForRoomConsistentWithParent(RoomNodeSO roomNode, Doorway doorwayParent)
    {
        return roomNode.roomNodeType.isCorridor 
            ? GetCorridorOrientation(doorwayParent.Orientation) 
            : GetRandomRoomTemplate(roomNode.roomNodeType);
    }

    [CanBeNull]
    private RoomTemplateSO GetCorridorOrientation(Orientation orientation)
    {
        switch (orientation)
        {
            case Orientation.North:
            case Orientation.South:
                return GetRandomRoomTemplate(_roomNodeTypes.RoomNodeTypes.Find(x => x.isCorridorNS));


            case Orientation.East:
            case Orientation.West:
                return GetRandomRoomTemplate(_roomNodeTypes.RoomNodeTypes.Find(x => x.isCorridorEW));


            case Orientation.None:
                break;
            
        }

        return null;
    }


    /// <summary>
    /// Place the room - returns true if the room doesn't overlap, false otherwise
    /// </summary>
    private bool PlaceTheRoom(Room parentRoom, Doorway doorwayParent, Room room)
    {

        // Get current room doorway position
        var doorway = GetOppositeDoorway(doorwayParent, room.DoorWayList);

        // Return if no doorway in room opposite to parent doorway
        if (doorway == null)
        {
            // Just mark the parent doorway as unavailable so we don't try and connect it again
            doorwayParent.IsUnavailable = true;

            return false;
        }

        // Calculate 'world' grid parent doorway position
        var parentDoorwayPosition = parentRoom.LowerBounds + doorwayParent.Position - parentRoom.TemplateLowerBounds;

        var adjustment = Vector2Int.zero;

        // Calculate adjustment position offset based on room doorway position that we are trying to connect (e.g. if this doorway is west then we need to add (1,0) to the east parent doorway)

        switch (doorway.Orientation)
        {
            case Orientation.North:
                adjustment = new Vector2Int(0, -1);
                break;

            case Orientation.East:
                adjustment = new Vector2Int(-1, 0);
                break;

            case Orientation.South:
                adjustment = new Vector2Int(0, 1);
                break;

            case Orientation.West:
                adjustment = new Vector2Int(1, 0);
                break;

            case Orientation.None:
                break;

            default:
                break;
        }

        // Calculate room lower bounds and upper bounds based on positioning to align with parent doorway
        room.LowerBounds = parentDoorwayPosition + adjustment + room.TemplateLowerBounds - doorway.Position;
        room.UpperBounds = room.LowerBounds + room.TemplateUpperBounds - room.TemplateLowerBounds;

        var overlappingRoom = CheckForRoomOverlap(room);

        if (overlappingRoom == null)
        {
            // mark doorways as connected & unavailable
            doorwayParent.IsConnected = true;
            doorwayParent.IsUnavailable = true;

            doorway.IsConnected = true;
            doorway.IsUnavailable = true;

            // return true to show rooms have been connected with no overlap
            return true;
        }
        // Just mark the parent doorway as unavailable so we don't try and connect it again
        doorwayParent.IsUnavailable = true;

        return false;
    }


    /// <summary>
    /// Get the doorway from the doorway list that has the opposite orientation to doorway
    /// </summary>
    private static Doorway GetOppositeDoorway(Doorway parentDoorway, List<Doorway> doorwayList)
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
    /// Check for rooms that overlap the upper and lower bounds parameters, and if there are overlapping rooms then return room else return null
    /// </summary>
    private Room CheckForRoomOverlap(Room roomToTest)
    {
        // Iterate through all rooms
        return MapBuilderRoomDictionary
            .Select(keyValuePair => keyValuePair.Value)
            .Where(room => room.Id != roomToTest.Id && room.IsPositioned)
            .FirstOrDefault(room => IsOverLappingRoom(roomToTest, room));
    }


    /// <summary>
    /// Check if 2 rooms overlap each other - return true if they overlap or false if they don't overlap
    /// </summary>
    private bool IsOverLappingRoom(Room room1, Room room2)
    {
        var isOverlappingX = IsOverLappingInterval(room1.LowerBounds.x, room1.UpperBounds.x, room2.LowerBounds.x, room2.UpperBounds.x);
        var isOverlappingY = IsOverLappingInterval(room1.LowerBounds.y, room1.UpperBounds.y, room2.LowerBounds.y, room2.UpperBounds.y);

        return isOverlappingX && isOverlappingY;
    }


    /// <summary>
    /// Check if interval 1 overlaps interval 2 - this method is used by the IsOverlappingRoom method
    /// </summary>
    private bool IsOverLappingInterval(int imin1, int imax1, int imin2, int imax2) =>
        Mathf.Max(imin1, imin2) <= Mathf.Min(imax1, imax2);


    /// <summary>
    /// Get a random room template from the roomtemplatelist that matches the roomType and return it
    /// (return null if no matching room templates found).
    /// </summary>
    private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType)
    {
        var matchingRoomTemplateList = new List<RoomTemplateSO>();

        // Loop through room template list
        var matching = _roomTemplateList
            .Where(x => x.RoomNodeType == roomNodeType)
            .ToList();
        
        matching.ForEach(x => matchingRoomTemplateList.Add(x));

        // Return null if list is zero
        if (matchingRoomTemplateList.Count == 0)
            return null;

        // Select random room template from list and return
        return matchingRoomTemplateList[Random.Range(0, matchingRoomTemplateList.Count)];

    }


    /// <summary>
    /// Get unconnected doorways
    /// </summary>
    private static IEnumerable<Doorway> GetUnconnectedAvailableDoorways(IEnumerable<Doorway> roomDoorwayList) => 
        roomDoorwayList.Where(doorway => !doorway.IsConnected && !doorway.IsUnavailable);
    


    /// <summary>
    /// Create room based on roomTemplate and layoutNode, and return the created room
    /// </summary>
    private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode)
    {
        // Initialise room from template
        var room = new Room();

        room.TemplateID = roomTemplate.Guid;
        room.Id = roomNode.Id;
        room.Prefab = roomTemplate.Prefab;
        room.RoomNodeType = roomTemplate.RoomNodeType;
        room.LowerBounds = roomTemplate.LowerBounds;
        room.UpperBounds = roomTemplate.UpperBounds;
        room.SpawnPositionArray = roomTemplate.SpawnPositions;
        room.TemplateLowerBounds = roomTemplate.LowerBounds;
        room.TemplateUpperBounds = roomTemplate.UpperBounds;
        room.ChildRoomIDList = CopyStringList(roomNode.childRoomNodeIDs);
        room.DoorWayList = CopyDoorwayList(roomTemplate.Doorways);

        // Set parent ID for room
        if (roomNode.parentRoomNodeIDList.Count == 0) // Entrance
        {
            room.ParentRoomID = "";
            room.IsPreviouslyVisited = true;
        }
        else
        {
            room.ParentRoomID = roomNode.parentRoomNodeIDList[0];
        }

        return room;
    }


    /// <summary>
    /// Select a random room node graph from the list of room node graphs
    /// </summary>
    private static RoomNodeGraphSO SelectRandomRoomNodeGraph(IReadOnlyList<RoomNodeGraphSO> roomNodeGraphList)
    {
        if (roomNodeGraphList.Count > 0)
        {
            return roomNodeGraphList[Random.Range(0, roomNodeGraphList.Count)];
        }
        
        Debug.Log("No room node graphs in list"); 
        return null;
    }


    /// <summary>
    /// Create deep copy of doorway list
    /// </summary>
    private static List<Doorway> CopyDoorwayList(IEnumerable<Doorway> oldDoorwayList) =>
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
    /// Create deep copy of string list
    /// </summary>
    private static List<string> CopyStringList(IEnumerable<string> oldStringList) => 
        oldStringList.ToList();
    

    /// <summary>
    /// Instantiate the dungeon room game objects from the prefabs
    /// </summary>
    private void InstantiateRoomGameobjects()
    {
        // Iterate through all dungeon rooms.
        foreach (KeyValuePair<string, Room> keyvaluepair in MapBuilderRoomDictionary)
        {
            Room room = keyvaluepair.Value;

            // Calculate room position (remember the room instantiatation position needs to be adjusted by the room template lower bounds)
            var roomPosition = new Vector3(room.LowerBounds.x - room.TemplateLowerBounds.x, room.LowerBounds.y - room.TemplateLowerBounds.y, 0f);

            // Instantiate room
            var roomGameObject = Instantiate(room.Prefab, roomPosition, Quaternion.identity, transform);

            // Get instantiated room component from instantiated prefab.
            var instantiatedRoom = roomGameObject.GetComponentInChildren<InstantiatedRoom>();

            instantiatedRoom.Room = room;

            // Initialise The Instantiated Room
            instantiatedRoom.Initialise(roomGameObject);

            // Save game object reference.
            room.InstantiatedRoom = instantiatedRoom;
        }
    }


    /// <summary>
    /// Get a room template by room template ID, returns null if ID doesn't exist
    /// </summary>
    public RoomTemplateSO GetRoomTemplate(string roomTemplateID) =>
        _roomTemplateDictionary.TryGetValue(roomTemplateID, out var roomTemplate) ? roomTemplate : null;

    /// <summary>
    /// Get room by roomID, if no room exists with that ID return null
    /// </summary>
    public Room GetRoomByRoomID(string roomID) =>
        MapBuilderRoomDictionary.TryGetValue(roomID, out var room) ? room : null;


    /// <summary>
    /// Clear dungeon room game objects and dungeon room dictionary
    /// </summary>
    private void ClearDungeon()
    {
        // Destroy instantiated dungeon game objects and clear dungeon manager room dictionary
        if (MapBuilderRoomDictionary.Count <= 0) return;
        foreach (var room in MapBuilderRoomDictionary
                     .Select(keyValuePair => keyValuePair.Value)
                     .Where(room => room.InstantiatedRoom != null))
        {
            Destroy(room.InstantiatedRoom.gameObject);
        }

        MapBuilderRoomDictionary.Clear();
    }
}