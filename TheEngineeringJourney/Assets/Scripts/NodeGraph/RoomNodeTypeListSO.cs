using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeTypeListSO", menuName = "Scriptable Objects/Map/Room Node Type List")]
public class RoomNodeTypeListSO : ScriptableObject
{
    #region Header ROOM NODE TYPE LIST
    [Space(10)]
    [Header("ROOM NODE TYPE LIST")]
    #endregion
    
    #region Tooltip
    [Tooltip("This list should populated with all the RoomNodeTypeSO for the game - it is used instead of an enum")]
    
    #endregion
    public List<RoomNodeTypeSO> list;
    
    #region Header ROOM NODE TYPE LIST
    # if UNITY_EDITOR
    private void OnValidate()
    {

        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(list), list);
    }
#endif
    #endregion
}
