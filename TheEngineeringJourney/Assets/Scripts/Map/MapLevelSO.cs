using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MapLevel_", menuName = "Scriptable Objects/Map/Map Level")]
public class MapLevelSO : ScriptableObject
{
    #region Header BASIC LEVEL DETAILS

    [Space(10)]
    [Header("BASIC LEVEL DETAILS")]

    #endregion
    #region Tooltip
    [Tooltip("The Name For The Level")]

    #endregion
    public string LevelName;

    #region Header ROOM TEMPLATES FOR LEVEL

    [Space(10)] [Header("ROOM TEMPLATES FOR LEVEL")]

    #endregion
    #region Tooltip

    [Tooltip("Add the room templates you want in the level. Make sure to include templates for all types of rooms mentioned in the Room Node Graphs for the level.")]

    #endregion
    public List<RoomTemplateSO> RoomTemplates;
    
    #region Header ROOM NODE GRAPHS FOR LEVEL

    [Space(10)]
    [Header("ROOM NODE GRAPHS FOR LEVEL")]

    #endregion

    #region Tooltip

    [Tooltip("Fill this list with the room node graphs that will be randomly chosen for the level")]

    #endregion Tooltip

    public List<RoomNodeGraphSO> RoomNodeGraphs;

    #region Validation

#if UNITY_EDITOR
    
    /// <summary>
    ///  Ensure that room templates are assigned for every node type in the chosen node graphs
    /// </summary>
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(LevelName), LevelName);
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(RoomTemplates), RoomTemplates)) return;
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(RoomNodeGraphs), RoomNodeGraphs)) return;
        
        // RoomTemplates.IsTypeOfCorridorOrEntrance(name).ForEach( x => Debug.Log(x));
        // First check that north/south corridor, east/west corridor and entrance types have been specified
        bool isEWCorridor = false;
        bool isNSCorridor = false;
        bool isEntrance = false;

        // Loop through all room templates to check that this node type has been specified
        foreach (RoomTemplateSO roomTemplateSO in RoomTemplates)
        {
            if (roomTemplateSO == null)
                return;

            if (roomTemplateSO.RoomNodeType.isCorridorEW)
                isEWCorridor = true;

            if (roomTemplateSO.RoomNodeType.isCorridorNS)
                isNSCorridor = true;

            if (roomTemplateSO.RoomNodeType.isEntrance)
                isEntrance = true;
        }

        if (isEWCorridor == false)
        {
            Debug.Log("In " + this.name.ToString() + " : No E/W Corridor Room Type Specified");
        }

        if (isNSCorridor == false)
        {
            Debug.Log("In " + this.name.ToString() + " : No N/S Corridor Room Type Specified");
        }

        if (isEntrance == false)
        {
            Debug.Log("In " + this.name.ToString() + " : No Entrance Corridor Room Type Specified");
        }


        GetTupleOfInvalidGraphsAndNodes(RoomNodeGraphs, RoomTemplates)
            .InvalidGraphsAndNodes(name)
            .ForEach(Debug.Log);
    }



    private static IEnumerable<(RoomNodeGraphSO roomNodeGraph, RoomNodeSO roomNodeSO)> GetTupleOfInvalidGraphsAndNodes(
        IEnumerable<RoomNodeGraphSO> roomNodeGraphs, IEnumerable<RoomTemplateSO> roomTemplate) =>
        roomNodeGraphs
            .SelectMany(roomNodeGraph => roomNodeGraph?.roomNodeList
                .Select(roomNodeSO => (roomNodeGraph, roomNodeSO)))
            .Where(tuple => IsRoomNodeTypeAlreadyChecked(tuple.roomNodeSO) &&
                            !IsMatchingRoomTemplate(tuple.roomNodeSO, roomTemplate));

    private static bool IsRoomNodeTypeAlreadyChecked(RoomNodeSO roomNodeSO) =>
        !roomNodeSO.roomNodeType.isEntrance &&
        !roomNodeSO.roomNodeType.isCorridorEW &&
        !roomNodeSO.roomNodeType.isCorridorNS &&
        !roomNodeSO.roomNodeType.isCorridor &&
        !roomNodeSO.roomNodeType.isNone;

    private static bool IsMatchingRoomTemplate(RoomNodeSO roomNodeSO, IEnumerable<RoomTemplateSO> roomTemplateList) =>
        roomTemplateList.Any(template => template?.RoomNodeType == roomNodeSO.roomNodeType);
    
    
#endif

    #endregion
}



#region Extension

#if UNITY_EDITOR
public static class MapLevelExtensions
{
    public static List<string> IsTypeOfCorridorOrEntrance(this IReadOnlyCollection<RoomTemplateSO> roomTemplates, string name)
    {
        var errors = new List<string>();

        if (roomTemplates.Any(x => x?.RoomNodeType.isCorridorEW == false))
        {
            errors.Add($"In {name}: No E/W Corridor Room Type Specified");
        }

        if (roomTemplates.Any(x => x?.RoomNodeType.isCorridorNS == false))
        {
            errors.Add($"In {name}: No N/S Corridor Room Type Specified");
        }

        if (roomTemplates.Any(x => x?.RoomNodeType.isEntrance == false))
        {
            errors.Add($"In {name}: No Entrance Corridor Room Type Specified");
        }

        return errors;
    }

    public static List<string> InvalidGraphsAndNodes(this IEnumerable<(RoomNodeGraphSO roomNodeGraph, RoomNodeSO roomNodeSO)> tuples, string name) => 
        tuples
            .Select(tuple =>
                $"In {name}: No room template {tuple.roomNodeSO.roomNodeType.name} found for node graph {tuple.roomNodeGraph.name}")
            .ToList();
}
#endif

#endregion