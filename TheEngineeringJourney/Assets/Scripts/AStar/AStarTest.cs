using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarTest : MonoBehaviour
{
    private InstantiatedRoom instantiatedRoom;
    private Grid grid;
    private Tilemap frontTilemap;
    private Tilemap pathTilemap;
    private Vector3Int startGridPosition;
    private Vector3Int endGridPosition;
    private TileBase startPathTile;
    private TileBase finishPathTile;

    private Vector3Int noValue = new Vector3Int(9999, 9999, 9999);
    private Stack<Vector3> pathStack;

    private void OnEnable()
    {
        // Subscribe to the onRoomChanged event
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        // Unsubscribe to the onRoomChanged event
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void Start()
    {
        startPathTile = GameResources.Instance.PreferredEnemyPathTile;
        finishPathTile = GameResources.Instance.EnemyUnwalkableCollisionTilesArray[0];
    }


    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        pathStack = null;
        instantiatedRoom = roomChangedEventArgs.Room.InstantiatedRoom;
        frontTilemap = instantiatedRoom.transform.Find("Grid/Tilemap4_Front").GetComponent<Tilemap>();
        grid = instantiatedRoom.transform.GetComponentInChildren<Grid>();
        startGridPosition = noValue;
        endGridPosition = noValue;

        SetUpPathTilemap();
    }

    /// <summary>
    /// Use a clone of the front tilemap for the path tilemap.  If not created then create one, else use the exisitng one.
    /// </summary>
    private void SetUpPathTilemap()
    {
        Transform tilemapCloneTransform = instantiatedRoom.transform.Find("Grid/Tilemap4_Front(Clone)");

        // If the front tilemap hasn't been cloned then clone it
        if (tilemapCloneTransform == null)
        {
            pathTilemap = Instantiate(frontTilemap, grid.transform);
            pathTilemap.GetComponent<TilemapRenderer>().sortingOrder = 2;
            pathTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.LitMaterial;
            pathTilemap.gameObject.tag = "Untagged";
        }
        // else use it
        else
        {
            pathTilemap = instantiatedRoom.transform.Find("Grid/Tilemap4_Front(Clone)").GetComponent<Tilemap>();
            pathTilemap.ClearAllTiles();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        //Debug.Log("Astar test Update:" + instantiatedRoom + startPathTile + finishPathTile + grid + pathTilemap);
        if (instantiatedRoom == null || startPathTile == null || finishPathTile == null || grid == null || pathTilemap == null) return;

        if (Input.GetKeyDown(KeyCode.K))
        {
            ClearPath();
            SetStartPosition();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            ClearPath();
            SetEndPosition();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            DisplayPath();
        }
    }


    /// <summary>
    /// Set the start position and the start tile on the front tilemap
    /// </summary>
    private void SetStartPosition()
    {
        if (startGridPosition == noValue)
        {
            startGridPosition = grid.WorldToCell(GameUtilities.GetMouseWorldPosition());

            if (!IsPositionWithinBounds(startGridPosition))
            {
                startGridPosition = noValue;
                return;
            }

            pathTilemap.SetTile(startGridPosition, startPathTile);
        }
        else
        {
            pathTilemap.SetTile(startGridPosition, null);
            startGridPosition = noValue;
        }
    }


    /// <summary>
    /// Set the end position and the end tile on the front tilemap
    /// </summary>
    private void SetEndPosition()
    {
        if (endGridPosition == noValue)
        {
            endGridPosition = grid.WorldToCell(GameUtilities.GetMouseWorldPosition());

            if (!IsPositionWithinBounds(endGridPosition))
            {
                endGridPosition = noValue;
                return;
            }

            pathTilemap.SetTile(endGridPosition, finishPathTile);
        }
        else
        {
            pathTilemap.SetTile(endGridPosition, null);
            endGridPosition = noValue;
        }
    }


    /// <summary>
    /// Check if the position is within the lower and upper bounds of the room
    /// </summary>
    private bool IsPositionWithinBounds(Vector3Int position)
    {
        // If  position is beyond grid then return false
        if (position.x < instantiatedRoom.Room.TemplateLowerBounds.x || position.x > instantiatedRoom.Room.TemplateUpperBounds.x
            || position.y < instantiatedRoom.Room.TemplateLowerBounds.y || position.y > instantiatedRoom.Room.TemplateUpperBounds.y)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    /// <summary>
    /// Clear the path and reset the start and finish positions
    /// </summary>
    private void ClearPath()
    {
        // Clear Path
        if (pathStack == null) return;

        foreach (Vector3 worldPosition in pathStack)
        {
            pathTilemap.SetTile(grid.WorldToCell(worldPosition), null);
        }

        pathStack = null;

        //Clear Start and Finish Squares
        endGridPosition = noValue;
        startGridPosition = noValue;
    }

    /// <summary>
    /// Build and display the AStar path between the start and finish positions
    /// </summary>
    private void DisplayPath()
    {
        if (startGridPosition == noValue || endGridPosition == noValue) return;

        pathStack = AStar.BuildPath(instantiatedRoom.Room, startGridPosition, endGridPosition);
        Debug.Log("DisplayPath" + pathStack);
        
        if (pathStack == null) return;

        foreach (Vector3 worldPosition in pathStack)
        {
            pathTilemap.SetTile(grid.WorldToCell(worldPosition), startPathTile);
        }
    }

}