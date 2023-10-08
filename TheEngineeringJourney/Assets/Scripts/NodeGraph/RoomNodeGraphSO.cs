using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeGraph", menuName = "Scriptable Objects/Map/Room Node Graph")]
public class RoomNodeGraphSO : ScriptableObject
{
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
    [HideInInspector] public List<RoomNodeSO> roomNodeList = new();
    [HideInInspector] public Dictionary<string, RoomNodeSO> RoomNodeDictionary = new();

    private void Awake()
    {
        LoadRoomNodeDictionary();
    }

    private void LoadRoomNodeDictionary()
    {
        RoomNodeDictionary = roomNodeList.ToDictionary(node => node.Id, node => node);
    }
    
    /// <summary>
    /// Get room node by roomNodeType
    /// </summary>
    public RoomNodeSO GetRoomNode(RoomNodeTypeSO roomNodeType) => 
        roomNodeList.FirstOrDefault(node => node.roomNodeType == roomNodeType);

    public RoomNodeSO GetRoomNode(string roomNodeID) =>
        RoomNodeDictionary.TryGetValue(roomNodeID, out var roomNode) ? roomNode : null;
    
    /// <summary>
    /// Get child room nodes for supplied parent room node
    /// </summary>
    public IEnumerable<RoomNodeSO> GetChildRoomNodes(RoomNodeSO parentRoomNode) =>
        parentRoomNode.childRoomNodeIDs.Select(GetRoomNode);


    #region  Editor Code

#if  UNITY_EDITOR
    
    [HideInInspector] public RoomNodeSO roomNodeToDrawLineFrom = null;
    [HideInInspector] public Vector2 linePosition;

    public void OnValidate()
    {
        LoadRoomNodeDictionary();
    }

    public void SetNodeDrawConnectionLineFrom(RoomNodeSO node, Vector2 position)
    {
        roomNodeToDrawLineFrom = node;
        linePosition = position;
    }
#endif

    #endregion
}
