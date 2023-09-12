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
        
        // Display a PopUp Using the RoomNodeType Name Values, that can be selected from (default to the currently set roomNodeType
        var selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);
        var selection = EditorGUILayout.Popup("", selected,GetRoomTypesToDisplay());

        roomNodeType = roomNodeTypeList.list[selection];
        
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
        if (childRoomNodeIDList.Contains(id)) return false;
        childRoomNodeIDList.Add(id);
        return true;
    }
    
    public bool AddParentRoomNodeID(string id)
    {
        if (parentRoomNodeIDList.Contains(id)) return false;
        parentRoomNodeIDList.Add(id);
        return true;
    }
#endif
    #endregion

}
