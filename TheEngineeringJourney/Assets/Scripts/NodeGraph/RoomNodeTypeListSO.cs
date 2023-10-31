using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "RoomNodeTypeListSO", menuName = "Scriptable Objects/Map/Room Node Type List")]
public class RoomNodeTypeListSO : ScriptableObject
{
    #region Header ROOM NODE TYPE LIST
    [FormerlySerializedAs("RoomNodeTypeSOs")]
    [FormerlySerializedAs("list")]
    [Space(10)]
    [Header("ROOM NODE TYPE LIST")]
    #endregion
    
    #region Tooltip
    [Tooltip("This list should populated with all the RoomNodeTypeSO for the game - it is used instead of an enum")]
    
    #endregion
    public List<RoomNodeTypeSO> RoomNodeTypes;
    
    #region Header ROOM NODE TYPE LIST
    # if UNITY_EDITOR
    private void OnValidate()
    {
        EditorUtilities.ValidateCheckEnumerableValues(this, nameof(RoomNodeTypes), RoomNodeTypes);
    }
#endif
    #endregion
}
