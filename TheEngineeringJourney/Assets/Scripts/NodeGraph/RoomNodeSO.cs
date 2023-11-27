using System;
using System.Collections.Generic;
using System.Linq;
using NodeGraph;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class RoomNodeSO : ScriptableObject
{
    [FormerlySerializedAs("id")] [HideInInspector] public string Id;
    [HideInInspector] public List<string> parentRoomNodeIDList = new();
    [FormerlySerializedAs("childRoomNodeIDList")] [HideInInspector] public List<string> childRoomNodeIDs = new();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;

    #region Editor 
#if UNITY_EDITOR
    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging;
    [HideInInspector] public bool isSelected;
    public void Initialise(Rect rect, RoomNodeGraphSO roomNodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        this.Id = Guid.NewGuid().ToString();
        this.name = "RoomNode";
        this.roomNodeGraph = roomNodeGraph;
        this.roomNodeType = roomNodeType;
        
        // Load RoomNode Type List
        roomNodeTypeList = GameResources.Instance.RoomNodeTypes;
    }

    public void Draw(GUIStyle nodeStyle)
    {
        // Draw Node Box Using Begin Area
        GUILayout.BeginArea(rect, nodeStyle);
        
        // Start Region To Detect Popup Selection Changes
        EditorGUI.BeginChangeCheck();
        
        // Display label that can't be changed
        if (parentRoomNodeIDList.Count > 0 || roomNodeType.isEntrance)
        {
            EditorGUILayout.LabelField(roomNodeType.RoomNodeTypeName);
        }
        else 
        {
            // Display a PopUp Using the RoomNodeType Name Values, that can be selected from (default to the currently set roomNodeType
            var selected = roomNodeTypeList.RoomNodeTypes.FindIndex(x => x == roomNodeType);
            var selection = EditorGUILayout.Popup("", selected,GetRoomTypesToDisplay());

            roomNodeType = roomNodeTypeList.RoomNodeTypes[selection];

            // If deleting a node changed the child to be invalid, remove relationship
            if (IsSelectedChanged(selected, selection) && childRoomNodeIDs.Count > 0)
            {
                roomNodeGraph.roomNodeList
                        .Where(roomNode => childRoomNodeIDs.Contains(roomNode.Id))
                        .ToList()
                        .ForEach(childRoomNode =>
                        {
                            RemoveChildRoomNodeIDFromRoomNode(childRoomNode.Id);
                            childRoomNode.RemoveParentRoomNodeIDFromRoomNode(Id);
                        });
            }

        }
        
        if(EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);
        
        GUILayout.EndArea();
    }

    private bool IsSelectedChanged(int selected, int selection) => 
        roomNodeTypeList.RoomNodeTypes[selected].isCorridor && !roomNodeTypeList.RoomNodeTypes[selection].isCorridor ||
               !roomNodeTypeList.RoomNodeTypes[selected].isCorridor && roomNodeTypeList.RoomNodeTypes[selection].isCorridor ||
               !roomNodeTypeList.RoomNodeTypes[selected].isBossRoom && roomNodeTypeList.RoomNodeTypes[selection].isBossRoom;

    /// <summary>
    /// Populate a string array with the RoomNodeTypes To Display That Can Be Selected
    /// </summary>
    private string[] GetRoomTypesToDisplay() =>
        roomNodeTypeList.RoomNodeTypes
            .Where(nodeType => nodeType.displayInNodeGraphEditor)
            .Select(nodeType => nodeType.RoomNodeTypeName)
            .ToArray();

    #region Events
    public void ProcessEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
        }
    }

    private void ProcessMouseDownEvent(Event currentEvent)
    {
        if (IsLeftClicked(currentEvent))
        {
            ProcessLeftClickDownEvent();
        } else if (IsRightClicked(currentEvent))
        {
            ProcessRightClickDownEvent(currentEvent);
        }
    }

    private void ProcessLeftClickDownEvent()
    {
        Selection.activeObject = this;
        
        // Toggle Mode Selected
        isSelected = !isSelected;
    }

    private void ProcessRightClickDownEvent(Event currentEvent)
    {
        roomNodeGraph.SetNodeDrawConnectionLineFrom(this, currentEvent.mousePosition);
    }
    
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        if (IsLeftClicked(currentEvent))
        {
            ProcessLeftClickUpEvent();
        }
    }

    private void ProcessLeftClickUpEvent()
    {
        if (isLeftClickDragging) isLeftClickDragging = false;
    }
    
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        if (IsLeftClicked(currentEvent))
        {
            ProcessLeftClickDragEvent(currentEvent);
        }
    }

    private void ProcessLeftClickDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;

        DragNode(currentEvent.delta);
        GUI.changed = true;
    }
    #endregion
    #region Buttons

    private static bool IsLeftClicked(Event currentEvent) => currentEvent.button == 0;
    private static bool IsRightClicked(Event currentEvent) => currentEvent.button == 1;

    #endregion


    public void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }

    public bool AddChildRoomNodeID(string childId)
    {
        if (!RoomNodeSOValidator.IsChildRoomValid(Id, childId, roomNodeGraph, childRoomNodeIDs, roomNodeType)) 
            return false;
        
        childRoomNodeIDs.Add(childId);
        return true;
    }
    
    public bool AddParentRoomNodeID(string parentId)
    {
        parentRoomNodeIDList.Add(parentId);
        return true;
    }

    public bool RemoveChildRoomNodeIDFromRoomNode(string childId)
    {
        if (!childRoomNodeIDs.Contains(childId)) return false;
        
        childRoomNodeIDs.Remove(childId);
        return true;
    }
    
    public bool RemoveParentRoomNodeIDFromRoomNode(string parentId)
    {
        if (!parentRoomNodeIDList.Contains(parentId)) return false;
        
        parentRoomNodeIDList.Remove(parentId);
        return true;
    }
#endif
    #endregion

}

public static class RoomNodeSOHelper
{
    public static bool IsEntrance(this RoomNodeSO roomNode) => roomNode.parentRoomNodeIDList.Count == 0;
    
    public static bool IsEntranceDebug(this RoomNodeSO entranceNode)
    {
        if (entranceNode is not null) return false;
        
        Debug.Log("Entrance Node missing. Please provide Room Node in Room Node graph scriptable object asset");
        return true;
    }
    
    public static bool IsQueueEmpty(this Queue<RoomNodeSO> queueRoomNode)
    {
        if (queueRoomNode.Count > 0) 
        {
            Debug.LogError("The Room Node Queue is empty. Please provide Room Node in Room Node graph scriptable object asset");    
            return false;
        }

        return true;
    }
}