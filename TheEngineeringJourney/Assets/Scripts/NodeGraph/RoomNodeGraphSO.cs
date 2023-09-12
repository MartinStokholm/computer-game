using System;
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
        // RoomNodeDictionary = roomNodeList.ToDictionary(node => node.id, node => node);
        RoomNodeDictionary.Clear();
        
        roomNodeList.ForEach(node => RoomNodeDictionary[node.id] = node);
        foreach (var node in roomNodeList)
        {
            RoomNodeDictionary[node.id] = node;
        }
    }
    
    #region  Editor Code

#if  UNITY_EDITOR
    
    [HideInInspector] public RoomNodeSO roomNodeToDrawLineFrom = null;
    [HideInInspector] public Vector2 linePosition;

    public void OnValidate()
    {
        LoadRoomNodeDictionary();
    }

    public void SetNodeDrawConnectionLinFrom(RoomNodeSO node, Vector2 position)
    {
        roomNodeToDrawLineFrom = node;
        linePosition = position;
    }
#endif

    #endregion
}
