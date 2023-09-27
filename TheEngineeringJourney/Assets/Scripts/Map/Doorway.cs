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
