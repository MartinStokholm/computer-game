using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour
{
    public Room Room;
    [HideInInspector] public Grid Grid;
    [HideInInspector] public Tilemap GroundTilemap;
    [HideInInspector] public Tilemap Decoration1Tilemap;
    [HideInInspector] public Tilemap Decoration2Tilemap;
    [HideInInspector] public Tilemap FrontTilemap;
    [HideInInspector] public Tilemap CollisionTilemap;
    [HideInInspector] public Tilemap MinimapTilemap;
    [HideInInspector] public Bounds RoomColliderBounds;
    [HideInInspector] public int[,] AStarMovementPenalty;  // use this 2d array to store movement penalties from the tilemaps to be used in AStar pathfinding
    [HideInInspector] public int[,] AStarItemObstacles; // use to store position of moveable items that are obstacles
    [HideInInspector] public List<MoveItem> moveableItemsList = new List<MoveItem>();
    
    #region Header OBJECT REFERENCES

    [Space(10)]
    [Header("OBJECT REFERENCES")]

    #endregion Header OBJECT REFERENCES

    #region Tooltip

    [Tooltip("Populate with the environment child placeholder gameobject ")]

    #endregion Tooltip

    [SerializeField] private GameObject environmentGameObject;
    private BoxCollider2D boxCollider2D;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();

        // Save room collider bounds
        RoomColliderBounds = boxCollider2D.bounds;

    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(Settings.PlayerTag) || Room == GameManager.Instance.CurrentRoom) return;
        
        // Set room as visited
        Room.IsPreviouslyVisited = true;

        // Call room changed event
        StaticEventHandler.CallRoomChangedEvent(Room);
    }
    
    /// <summary>
    /// Initialise The Instantiated Room
    /// </summary>
    public void Initialise(GameObject roomGameobject)
    {
        PopulateTilemapMemberVariables(roomGameobject);

        BlockOffUnusedDoorWays();
        
        AddObstaclesAndPreferredPaths();
        
        CreateItemObstaclesArray();

        AddDoorsToRooms();

        DisableCollisionTilemapRenderer();
    }

    /// <summary>
    /// Populate the tilemap and grid memeber variables.
    /// </summary>
    private void PopulateTilemapMemberVariables(GameObject roomGameobject)
    {
        // Get the grid component.
        Grid = roomGameobject.GetComponentInChildren<Grid>();

        // Get tilemaps in children.
        Tilemap[] tilemaps = roomGameobject.GetComponentsInChildren<Tilemap>();

        foreach (var tilemap in tilemaps)
        {
            switch (tilemap.gameObject.tag)
            {
                case "groundTilemap":
                    GroundTilemap = tilemap;
                    break;
                case "decoration1Tilemap":
                    Decoration1Tilemap = tilemap;
                    break;
                case "decoration2Tilemap":
                    Decoration2Tilemap = tilemap;
                    break;
                case "frontTilemap":
                    FrontTilemap = tilemap;
                    break;
                case "collisionTilemap":
                    CollisionTilemap = tilemap;
                    break;
                case "minimapTilemap":
                    MinimapTilemap = tilemap;
                    break;
            }
        }
    }
    
    /// <summary>
    /// Block Off Unused Doorways In The Room
    /// </summary>
    private void BlockOffUnusedDoorWays()
    {
        // Loop through all doorways
        foreach (Doorway doorway in Room.DoorWayList)
        {
            if (doorway.IsConnected)
                continue;

            // Block unconnected doorways using tiles on tilemaps
            if (CollisionTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(CollisionTilemap, doorway);
            }

            if (MinimapTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(MinimapTilemap, doorway);
            }

            if (GroundTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(GroundTilemap, doorway);
            }

            if (Decoration1Tilemap != null)
            {
                BlockADoorwayOnTilemapLayer(Decoration1Tilemap, doorway);
            }

            if (Decoration2Tilemap != null)
            {
                BlockADoorwayOnTilemapLayer(Decoration2Tilemap, doorway);
            }

            if (FrontTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(FrontTilemap, doorway);
            }
        }
    }
    
    /// <summary>
    /// Block a doorway on a tilemap layer
    /// </summary>
    private void BlockADoorwayOnTilemapLayer(Tilemap tilemap, Doorway doorway)
    {
        switch (doorway.Orientation)
        {
            case Orientation.North:
            case Orientation.South:
                BlockDoorwayHorizontally(tilemap, doorway);
                break;

            case Orientation.East:
            case Orientation.West:
                BlockDoorwayVertically(tilemap, doorway);
                break;

            case Orientation.None:
                break;
        }

    }
    
    /// <summary>
    /// Block doorway horizontally - for North and South doorways
    /// </summary>
    private void BlockDoorwayHorizontally(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.DoorwayStartCopyPosition;

        // loop through all tiles to copy
        for (int xPos = 0; xPos < doorway.DoorwayCopyTileWidth; xPos++)
        {
            for (int yPos = 0; yPos < doorway.DoorwayCopyTileHeight; yPos++)
            {
                // Get rotation of tile being copied
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));

                // Copy tile
                tilemap.SetTile(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0), tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                // Set rotation of tile copied
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0), transformMatrix);
            }
        }
    }

    /// <summary>
    /// Block doorway vertically - for East and West doorways
    /// </summary>
    private void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.DoorwayStartCopyPosition;

        // loop through all tiles to copy
        for (int yPos = 0; yPos < doorway.DoorwayCopyTileHeight; yPos++)
        {

            for (int xPos = 0; xPos < doorway.DoorwayCopyTileWidth; xPos++)
            {
                // Get rotation of tile being copied
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));

                // Copy tile
                tilemap.SetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                // Set rotation of tile copied
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), transformMatrix);

            }

        }
    }

    /// <summary>
    /// Update obstacles used by AStar pathfinmding.
    /// </summary>
    private void AddObstaclesAndPreferredPaths()
    {
        // this array will be populated with wall obstacles 
        AStarMovementPenalty = new int[Room.TemplateUpperBounds.x - Room.TemplateLowerBounds.x + 1, Room.TemplateUpperBounds.y - Room.TemplateLowerBounds.y + 1];


        // Loop thorugh all grid squares
        for (int x = 0; x < (Room.TemplateUpperBounds.x - Room.TemplateLowerBounds.x + 1); x++)
        {
            for (int y = 0; y < (Room.TemplateUpperBounds.y - Room.TemplateLowerBounds.y + 1); y++)
            {
                // Set default movement penalty for grid sqaures
                AStarMovementPenalty[x, y] = Settings.DefaultAStarMovementPenalty;

                // Add obstacles for collision tiles the enemy can't walk on
                TileBase tile = CollisionTilemap.GetTile(new Vector3Int(x + Room.TemplateLowerBounds.x, y + Room.TemplateLowerBounds.y, 0));

                foreach (TileBase collisionTile in GameResources.Instance.EnemyUnwalkableCollisionTilesArray)
                {
                    if (tile == collisionTile)
                    {
                        AStarMovementPenalty[x, y] = 0;
                        break;
                    }
                }

                // Add preferred path for enemies (1 is the preferred path value, default value for
                // a grid location is specified in the Settings).
                if (tile == GameResources.Instance.PreferredEnemyPathTile)
                {
                    AStarMovementPenalty[x, y] = Settings.PreferredPathAStarMovementPenalty;
                }

            }
        }

    }


    /// <summary>
    /// Add opening doors if this is not a corridor room
    /// </summary>
    private void AddDoorsToRooms()
    {
        // if the room is a corridor then return
        if (Room.RoomNodeType.isCorridorEW || Room.RoomNodeType.isCorridorNS) return;

        // Instantiate door prefabs at doorway positions
        foreach (Doorway doorway in Room.DoorWayList)
        {

            // if the doorway prefab isn't null and the doorway is connected
            if (doorway.DoorPrefab != null && doorway.IsConnected)
            {
                float tileDistance = Settings.TileSizePixels / Settings.PixelsPerUnit;

                GameObject door = null;

                if (doorway.Orientation == Orientation.North)
                {
                    // create door with parent as the room
                    door = Instantiate(doorway.DoorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.Position.x + tileDistance / 2f, doorway.Position.y + tileDistance, 0f);
                }
                else if (doorway.Orientation == Orientation.South)
                {
                    // create door with parent as the room
                    door = Instantiate(doorway.DoorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.Position.x + tileDistance / 2f, doorway.Position.y, 0f);
                }
                else if (doorway.Orientation == Orientation.East)
                {
                    // create door with parent as the room
                    door = Instantiate(doorway.DoorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.Position.x + tileDistance, doorway.Position.y + tileDistance * 1.25f, 0f);
                }
                else if (doorway.Orientation == Orientation.West)
                {
                    // create door with parent as the room
                    door = Instantiate(doorway.DoorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.Position.x, doorway.Position.y + tileDistance * 1.25f, 0f);
                }

                // Get door component
                Door doorComponent = door.GetComponent<Door>();

                // Set if door is part of a boss room
                if (Room.RoomNodeType.isBossRoom)
                {
                    doorComponent.isBossRoomDoor = true;

                    // lock the door to prevent access to the room
                    doorComponent.LockDoor();

                    // Instantiate skull icon for minimap by door
                    //GameObject skullIcon = Instantiate(GameResources.Instance.MinimapSkullPrefab, gameObject.transform);
                    //skullIcon.transform.localPosition = door.transform.localPosition;

                }
            }

        }

    }


    /// <summary>
    /// Disable collision tilemap renderer
    /// </summary>
    private void DisableCollisionTilemapRenderer()
    {
        // Disable collision tilemap renderer
        CollisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }

    /// <summary>
    /// Disable the room trigger collider that is used to trigger when the player enters a room
    /// </summary>
    public void DisableRoomCollider()
    {
        boxCollider2D.enabled = false;
    }

    /// <summary>
    /// Enable the room trigger collider that is used to trigger when the player enters a room
    /// </summary>
    public void EnableRoomCollider()
    {
        boxCollider2D.enabled = true;
    }

    public void ActivateEnvironmentGameObjects()
    {
        if (environmentGameObject != null)
            environmentGameObject.SetActive(true);
    }

    public void DeactivateEnvironmentGameObjects()
    {
        if (environmentGameObject != null)
            environmentGameObject.SetActive(false);
    }


    /// <summary>
    /// Lock the room doors
    /// </summary>
    public void LockDoors()
    {
        Door[] doorArray = GetComponentsInChildren<Door>();

        // Trigger lock doors
        foreach (Door door in doorArray)
        {
            door.LockDoor();
        }

        // Disable room trigger collider
        DisableRoomCollider();
    }

    /// <summary>
    /// Unlock the room doors
    /// </summary>
    public void UnlockDoors(float doorUnlockDelay)
    {
        StartCoroutine(UnlockDoorsRoutine(doorUnlockDelay));
    }

    /// <summary>
    /// Unlock the room doors routine
    /// </summary>
    private IEnumerator UnlockDoorsRoutine(float doorUnlockDelay)
    {
        if (doorUnlockDelay > 0f)
            yield return new WaitForSeconds(doorUnlockDelay);

        Door[] doorArray = GetComponentsInChildren<Door>();

        // Trigger open doors
        foreach (Door door in doorArray)
        {
            door.UnlockDoor();
        }

        // Enable room trigger collider
        EnableRoomCollider();
    }

    /// <summary>
    /// Create Item Obstacles Array
    /// </summary>
    private void CreateItemObstaclesArray()
    {
        // this array will be populated during gameplay with any moveable obstacles
        AStarItemObstacles = new int[Room.TemplateUpperBounds.x - Room.TemplateLowerBounds.x + 1, Room.TemplateUpperBounds.y - Room.TemplateLowerBounds.y + 1];
    }

    /// <summary>
    /// Initialize Item Obstacles Array With Default AStar Movement Penalty Values
    /// </summary>
    private void InitializeItemObstaclesArray()
    {
        for (int x = 0; x < (Room.TemplateUpperBounds.x - Room.TemplateLowerBounds.x + 1); x++)
        {
            for (int y = 0; y < (Room.TemplateUpperBounds.y - Room.TemplateLowerBounds.y + 1); y++)
            {
                // Set default movement penalty for grid sqaures
                AStarItemObstacles[x, y] = Settings.DefaultAStarMovementPenalty;
            }
        }
    }

    /// <summary>
    /// Update the array of moveable obstacles
    /// </summary>
    public void UpdateMoveableObstacles()
    {
        InitializeItemObstaclesArray();

        foreach (MoveItem moveItem in moveableItemsList)
        {
            var colliderBoundsMin = Grid.WorldToCell(moveItem.boxCollider2D.bounds.min);
            var colliderBoundsMax = Grid.WorldToCell(moveItem.boxCollider2D.bounds.max);

            // Loop through and add moveable item collider bounds to obstacle array
            for (int i = colliderBoundsMin.x; i <= colliderBoundsMax.x; i++)
            {
                for (int j = colliderBoundsMin.y; j <= colliderBoundsMax.y; j++)
                {
                    AStarItemObstacles[i - Room.TemplateLowerBounds.x, j - Room.TemplateLowerBounds.y] = 0;
                }
            }
        }
    }

}
