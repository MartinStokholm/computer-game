using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour
{
    [HideInInspector] public Room Room;
    [HideInInspector] public Grid Grid;
    [HideInInspector] public Tilemap GroundTilemap;
    [HideInInspector] public Tilemap Decoration1Tilemap;
    [HideInInspector] public Tilemap Decoration2Tilemap;
    [HideInInspector] public Tilemap FrontTilemap;
    [HideInInspector] public Tilemap CollisionTilemap;
    [HideInInspector] public Tilemap MinimapTilemap;
    [HideInInspector] public Bounds RoomColliderBounds;

    private BoxCollider2D boxCollider2D;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();

        // Save room collider bounds
        RoomColliderBounds = boxCollider2D.bounds;

    }
    
    /// <summary>
    /// Initialise The Instantiated Room
    /// </summary>
    public void Initialise(GameObject roomGameobject)
    {
        PopulateTilemapMemberVariables(roomGameobject);

        BlockOffUnusedDoorWays();

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
        var startPosition = doorway.DoorwayStartCopyPosition;

        // loop through all tiles to copy
        for (var xPos = 0; xPos < doorway.DoorwayCopyTileWidth; xPos++)
        {
            for (var yPos = 0; yPos < doorway.DoorwayCopyTileHeight; yPos++)
            {
                // Get rotation of tile being copied
                var transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));

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
        var startPosition = doorway.DoorwayStartCopyPosition;

        // loop through all tiles to copy
        for (var yPos = 0; yPos < doorway.DoorwayCopyTileHeight; yPos++)
        {

            for (var xPos = 0; xPos < doorway.DoorwayCopyTileWidth; xPos++)
            {
                // Get rotation of tile being copied
                var transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));

                // Copy tile
                tilemap.SetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                // Set rotation of tile copied
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), transformMatrix);

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

}
