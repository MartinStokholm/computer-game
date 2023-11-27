using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "RoomNodeType_", menuName = "Scriptable Objects/Map/Room Node Type")]
public class RoomNodeTypeSO : ScriptableObject
{
    [FormerlySerializedAs("roomNodeTypeName")] public string RoomNodeTypeName;

    #region Header

    [Header("Only flag the RoomNodeTypes that should be visible in the editor")]

    #endregion
    public bool displayInNodeGraphEditor = true;
    
    #region Header

    [Header("One Type Should Be A corridor")]

    #endregion
    public bool isCorridor;
    
    #region Header

    [Header("One Type Should Be A corridorNS")]

    #endregion
    public bool isCorridorNS;
    
    #region Header

    [Header("One Type Should Be A corridorEW")]

    #endregion
    public bool isCorridorEW;
    
    #region Header

    [Header("One Type Should Be A isEntrance")]

    #endregion
    public bool isEntrance;
    
    #region Header

    [Header("One Type Should Be A Boss Room")]

    #endregion
    public bool isBossRoom;
    
    #region Header

    [Header("One Type Should Be None (Unassigned")]

    #endregion
    public bool isNone;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        EditorUtilities.ValidateCheckEmptyString(this, nameof(RoomNodeTypeName), RoomNodeTypeName);
    }
#endif
    #endregion
}

public static class RoomNodeTypeSOHelper
{
    /// <summary>
    /// Get a random room template from the room template list that matches the roomType and return it
    /// (return null if no matching room templates found).
    /// </summary>
    public static RoomTemplateSO GetRandomRoomTemplate(this RoomNodeTypeSO roomNodeType, IEnumerable<RoomTemplateSO> roomTemplateList)
    {
        // Loop through room template list
        var matchingRoomTemplateList = roomTemplateList
            .Where(x => x.RoomNodeType == roomNodeType)
            .ToList();
        
        // Return null if list is zero or Select random room template from list and return
        return matchingRoomTemplateList.Count is not 0
            ? matchingRoomTemplateList[Random.Range(0, matchingRoomTemplateList.Count)]
            : null;
    }
}