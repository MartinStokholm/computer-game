using UnityEngine;
using UnityEngine.Serialization;

public class GameResources : MonoBehaviour
{
    private static GameResources _instance;

    public static GameResources Instance => _instance ?? Resources.Load<GameResources>("GameResources");

    #region Header Map
    [FormerlySerializedAs("roomNodeTypeList")]
    [FormerlySerializedAs("roomNodeTypeListSo")]
    [Space(10)]
    [Header("Map")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with map RoomNodeTypeListSO")]
    #endregion
    public RoomNodeTypeListSO RoomNodeTypes;
    
    #region Header PLAYER
    [FormerlySerializedAs("currentPlayer")]
    [Space(10)]
    [Header("PLAYER")]
    #endregion Header PLAYER
    #region Tooltip
    [Tooltip("The current player scriptable object - this is used to reference the current player between scenes")]
    #endregion Tooltip
    public CurrentPlayerSO CurrentPlayer;


    #region Header MATERIALS
    [Space(10)]
    [Header("MATERIALS")]
    #endregion
    #region Tooltip
    [Tooltip("Dimmed Material")]
    #endregion
    public Material DimmedMaterial;

}
