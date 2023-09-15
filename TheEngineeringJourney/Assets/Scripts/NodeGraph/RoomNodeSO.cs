using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentRoomNodeIDList = new();
    [HideInInspector] public List<string> childRoomNodeIDList = new();
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
        this.id = Guid.NewGuid().ToString();
        this.name = "RoomNode";
        this.roomNodeGraph = roomNodeGraph;
        this.roomNodeType = roomNodeType;
        
        // Load RoomNode Type List
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
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
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
        }
        else 
        {
            // Display a PopUp Using the RoomNodeType Name Values, that can be selected from (default to the currently set roomNodeType
            var selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);
            var selection = EditorGUILayout.Popup("", selected,GetRoomTypesToDisplay());

            roomNodeType = roomNodeTypeList.list[selection];
        }
        
        if(EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);
        
        GUILayout.EndArea();
    }

    /// <summary>
    /// Populate a string array with the RoomNodeTypes To Display That Can Be Selected
    /// </summary>
    private string[] GetRoomTypesToDisplay() =>
        roomNodeTypeList.list
            .Where(nodeType => nodeType.displayInNodeGraphEditor)
            .Select(nodeType => nodeType.roomNodeTypeName)
            .ToArray();

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
        roomNodeGraph.SetNodeDrawConnectionLinFrom(this, currentEvent.mousePosition);
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

    private static bool IsLeftClicked(Event currentEvent) => currentEvent.button == 0;
    private static bool IsRightClicked(Event currentEvent) => currentEvent.button == 1;

    private void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }

    public bool AddChildRoomNodeID(string id)
    {
        if (!IsChildRoomValid(id)) return false;
        
        childRoomNodeIDList.Add(id);
        return true;
    }

    private bool IsChildRoomValid(string childId)
    {
        var isConnectedBoosNodeAlready = roomNodeGraph.roomNodeList
            .Any(x => x.roomNodeType.isBossRoom && x.parentRoomNodeIDList.Count > 0);
        
        var childNode = roomNodeGraph.GetRoomNode(childId);
        
        // The child is a type boss room and there already is a boss room return false
        if (childNode.roomNodeType.isBossRoom && isConnectedBoosNodeAlready) return false;
        
        // Can't connect child that does a type of none
        if (childNode.roomNodeType.isNone) return false;

        // Room Already added as a child
        if (childRoomNodeIDList.Contains(childId)) return false;

        // Don't connect a child to it self
        if (id == childId) return false;

        // Child node already has parent
        if (childNode.parentRoomNodeIDList.Count > 0) return false;

        // Child is a corridor and this node is a corridor
        if (childNode.roomNodeType.isCorridor && roomNodeType.isCorridor) return false;
        
        // Child is a room and this node is a room
        if (!childNode.roomNodeType.isCorridor && !roomNodeType.isCorridor) return false;
        
        // Adding a corridor checking for the maximum permitted children for the room 
        if (childNode.roomNodeType.isCorridor 
            && childRoomNodeIDList.Count >=  Settings.MaxChildCorridors) return false;

        // Child room can't be a entrance - is top level parent node
        if (childNode.roomNodeType.isEntrance) return false;
        
        // Adding a room to a corridor check if this corridor node doesn't have a room already 
        if (!childNode.roomNodeType.isCorridor && childRoomNodeIDList.Count > 0) return false;
        
        return true;
    }
    
    public bool AddParentRoomNodeID(string id)
    {
        parentRoomNodeIDList.Add(id);
        return true;
    }
#endif
    #endregion

}
