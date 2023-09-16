using System;
using System.Collections.Generic;
using System.Linq;
using NodeGraph;
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

            // If deleting a node changed the child to be invalid, remove relationship
            if (IsSelectedChanged(selected, selection) && childRoomNodeIDList.Count > 0)
            {
                roomNodeGraph.roomNodeList
                        .Where(roomNode => childRoomNodeIDList.Contains(roomNode.id))
                        .ToList()
                        .ForEach(childRoomNode =>
                        {
                            RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);
                            childRoomNode.RemoveParentRoomNodeIDFromRoomNode(id);
                        });
            }

        }
        
        if(EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);
        
        GUILayout.EndArea();
    }

    private bool IsSelectedChanged(int selected, int selection)
    {
        return roomNodeTypeList.list[selected].isCorridor && !roomNodeTypeList.list[selection].isCorridor ||
               !roomNodeTypeList.list[selected].isCorridor && roomNodeTypeList.list[selection].isCorridor ||
               !roomNodeTypeList.list[selected].isBossRoom && roomNodeTypeList.list[selection].isBossRoom;
    }

    /// <summary>
    /// Populate a string array with the RoomNodeTypes To Display That Can Be Selected
    /// </summary>
    private string[] GetRoomTypesToDisplay() =>
        roomNodeTypeList.list
            .Where(nodeType => nodeType.displayInNodeGraphEditor)
            .Select(nodeType => nodeType.roomNodeTypeName)
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
    #endregion
    #region Buttons

    private static bool IsLeftClicked(Event currentEvent) => currentEvent.button == 0;
    private static bool IsRightClicked(Event currentEvent) => currentEvent.button == 1;

    #endregion


    private void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }

    public bool AddChildRoomNodeID(string childId)
    {
        if (!RoomNodeSOValidator.IsChildRoomValid(id, childId, roomNodeGraph, childRoomNodeIDList, roomNodeType)) 
            return false;
        
        childRoomNodeIDList.Add(childId);
        return true;
    }
    
    public bool AddParentRoomNodeID(string parentId)
    {
        parentRoomNodeIDList.Add(parentId);
        return true;
    }

    public bool RemoveChildRoomNodeIDFromRoomNode(string childId)
    {
        if (!childRoomNodeIDList.Contains(childId)) return false;
        
        childRoomNodeIDList.Remove(childId);
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
