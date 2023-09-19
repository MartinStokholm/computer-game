using UnityEngine;
using UnityEngine.Serialization;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance => instance ?? Resources.Load<GameResources>("GameResources");

    #region Header Map
    [FormerlySerializedAs("roomNodeTypeListSo")]
    [Space(10)]
    [Header("Map")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with map RoomNodeTypeListSO")]
    #endregion
    public RoomNodeTypeListSO roomNodeTypeList;
}
