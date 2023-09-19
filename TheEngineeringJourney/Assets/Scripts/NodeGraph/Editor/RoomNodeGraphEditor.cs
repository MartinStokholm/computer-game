using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using Color = UnityEngine.Color;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle _roomNodeStyle;
    private GUIStyle _roomNodeSelectedStyle;

    private Vector2 _graphOffSet;
    private Vector2 _graphDrag;
    
    private static RoomNodeGraphSO _currentRoomNodeGraph;
    private RoomNodeSO _currentRoomNode;
    private RoomNodeTypeListSO _roomNodeTypeList;
    
    // Node layout values
    private const float NodeWidth = 160f;
    private const float NodeHeight = 75f;
    private const int NodePadding = 25;
    private const int NodeBorder = 12;
    
    // Connecting line values
    private const float ConnectingLineWidth = 3f;
    private const float ConnectingLineArrowSize = 6f;
    
    //Grid Spacing
    private const float GridLarge = 100f;
    private const float GridSmall = 25f;
    
    [MenuItem("Room Node Graph Editor", menuItem = "Window/Map Editor/Room Node Graph Editor")]

    #region OnMethod

    // Define node layout style
    private void OnEnable()
    {
        // Subscribe to the inspector selection changed event
        Selection.selectionChanged += InspectorSelectionChanged;
        
        _roomNodeStyle = new GUIStyle
        {
            normal =
            {
                background = EditorGUIUtility.Load("node1") as Texture2D,
                textColor = Color.white
            },
            padding = new RectOffset(NodePadding, NodePadding, NodePadding, NodePadding),
            border = new RectOffset(NodeBorder, NodeBorder, NodeBorder, NodeBorder)
        };
        
        _roomNodeSelectedStyle = new GUIStyle
        {
            normal =
            {
                background = EditorGUIUtility.Load("node1 on") as Texture2D,
                textColor = Color.white
            },
            padding = new RectOffset(NodePadding, NodePadding, NodePadding, NodePadding),
            border = new RectOffset(NodeBorder, NodeBorder, NodeBorder, NodeBorder)
        };
        
        // Load Room node types
        _roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= InspectorSelectionChanged;
    }
    
    /// <summary>
    /// Selection changed in the inspector
    /// </summary>
    private static void InspectorSelectionChanged()
    {
        if (Selection.activeObject is not RoomNodeGraphSO roomNodeGraph) return;
        _currentRoomNodeGraph = roomNodeGraph;
        GUI.changed = true;
    }

    #endregion
    
    #region Editor
    
    private static void OpenWindow() => GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");

    /// <summary>
    /// Open the room node graph editor window if a room node graph scriptable object asset is double clicked in the inspector
    /// </summary>
    [OnOpenAsset(0)] // Callbacks see docs OnOpenAssetAttribute
    public static bool OnDoubleClickAsset(int instanceId, int line)
    {
        if (EditorUtility.InstanceIDToObject(instanceId) is RoomNodeGraphSO roomNodeGraph)
        {
            OpenWindow();
            _currentRoomNodeGraph = roomNodeGraph;
            return true;
        }

        return false;
    }
    
    #endregion
    
    #region GUIUpdates
    
    /// <summary>
    ///  Draw Editor GUI
    /// </summary>
    private void OnGUI()
    {
        // If a scriptable object of type RoomNodeGraphSO has been selected then process
        if (_currentRoomNodeGraph is not null)
        {
            DrawBackgroundGrid(GridSmall, 0.2f, Color.gray);
            DrawBackgroundGrid(GridLarge, 0.3f, Color.gray);
            
            DrawDraggedLine();
            ProcessEvents(Event.current);
            DrawRoomConnections();
            DrawRoomNodes();
        }
        
        if (GUI.changed)
            Repaint();
    }

    /// <summary>
    /// Draw a background grid for the editor
    /// </summary>
    private void DrawBackgroundGrid(float gridSize, float gridOpacity, Color gridColor)
    {
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);
        _graphOffSet += _graphDrag * 0.5f;

        var gridOffset = new Vector3(_graphOffSet.x % gridSize, _graphOffSet.y % gridSize, 0);
        
        var verticalLineCount = Mathf.CeilToInt((position.width + gridSize) / gridSize);
        for (var i = 0; i < verticalLineCount; i++)
        {
            Handles.DrawLine(new Vector3(gridSize * i, -gridSize, 0) + gridOffset,
                new Vector3(gridSize * i, position.height + gridSize, 0f) + gridOffset);
        }

        var horizontalLineCount = Mathf.CeilToInt((position.height + gridSize) / gridSize);
        for (var i = 0; i < horizontalLineCount; i++)
        {
            Handles.DrawLine(new Vector3(-gridSize, gridSize * i, 0) + gridOffset,
                new Vector3(position.width  + gridSize, gridSize * i, 0f) + gridOffset);
        }

        Handles.color = Color.white;
    }

    private static void DrawDraggedLine()
    {
        if (_currentRoomNodeGraph.linePosition != Vector2.zero)
        {
            Handles.DrawBezier(
                _currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center,
                _currentRoomNodeGraph.linePosition,
                _currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center,
                _currentRoomNodeGraph.linePosition,
                Color.white,
                null,
                ConnectingLineWidth);
        }
    }

    private void ProcessEvents(Event currentEvent)
    {
        // Reset graph drag
        _graphDrag = Vector2.zero;
        
        // Get Room Node That Mouse Is Over If It Is Null Or Not Currently Being Dragged
        if (_currentRoomNode is null || _currentRoomNode.isLeftClickDragging == false)
        {
            _currentRoomNode = IsMouseOverRoomNode(currentEvent);
        }

        // if mouse isn't over a room node or we are currently dragging a line from the room node then process graph events
        if (_currentRoomNode is null || _currentRoomNodeGraph.roomNodeToDrawLineFrom is not null)
        {
            ProcessRoomNodeGraphEvents(currentEvent);
        }
        else // else process room node events
        {
            _currentRoomNode.ProcessEvents(currentEvent);
        }
        
    }
    
    /// <summary>
    /// Draw connections in the graph window between nodes
    /// Find all room nodes, that haves children and use the dictionary to get the child and update them
    /// </summary>
    private static void DrawRoomConnections() {
        var connectedRoomNodes = _currentRoomNodeGraph.roomNodeList
            .Where(roomNodeSO => roomNodeSO.childRoomNodeIDList.Count > 0)
            .SelectMany(x => x.childRoomNodeIDList)
            .Select(childRoomNodeId => 
            (
                roomNodeSO: _currentRoomNodeGraph.roomNodeList.FindAll(x => x.childRoomNodeIDList.Contains(childRoomNodeId)),
                childRoomNodeId
            ))
            .ToList();

        connectedRoomNodes
            .ForEach(tuple =>
            {
                tuple.roomNodeSO
                    .ForEach(roomNode =>
                    {
                        DrawConnectionLine(roomNode, _currentRoomNodeGraph.RoomNodeDictionary[tuple.childRoomNodeId]);
                        GUI.changed = true;
                    });
            });
    }
    
    /// <summary>
    /// Draw RoomNodes In GraphWindow
    /// </summary>
    private void DrawRoomNodes()
    {
        _currentRoomNodeGraph.roomNodeList.ForEach(x =>
        {
            switch (x.isSelected)
            {
                case true:
                    x.Draw(_roomNodeSelectedStyle);
                    break;
                case false:
                    x.Draw(_roomNodeStyle);
                    break;
            }
        });
        GUI.changed = true;
    }

    #endregion
    
    #region ContextMenu

    /// <summary>
    /// Show the context menu
    /// </summary>
    private void ShowContextMenu(Vector2 mousePosition)
    {
        var menu = new GenericMenu();
        
        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Select All Room Nodes"), false, SelectedAllRoomNodes);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Select Room Nodes Links"), false, DeleteSelectedRoomNodeLinks);
        menu.AddItem(new GUIContent("Delete Select Room Nodes"), false, DeleteSelectedRoomNodes);
        menu.ShowAsContext();
    }

    /// <summary>
    /// Create a room node at the mouse position or the entrance room node
    /// </summary>
    private void CreateRoomNode(object mousePositionObject)
    {
        if (_currentRoomNodeGraph.roomNodeList.Count.Equals(0))
        {
            CreateRoomNode(new Vector2(200f, 200f), _roomNodeTypeList.list.Find(x => x.isEntrance));
        }
        
        CreateRoomNode(mousePositionObject, _roomNodeTypeList.list.Find(x => x.isNone));
    }
    
    /// <summary>
    /// Create a room node at the mouse position
    /// </summary>
    private static void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        var mousePosition = (Vector2)mousePositionObject;
        
        // Create RoomNode Scriptable Object Asset
        var roomNode = CreateInstance<RoomNodeSO>();
        
        // Add RoomNode to current RoomNodeGraphList
        _currentRoomNodeGraph.roomNodeList.Add(roomNode);
        
        // Set RoomNode values
        roomNode.Initialise(new Rect(
            mousePosition, 
            new Vector2(NodeWidth, NodeWidth)),
            _currentRoomNodeGraph, 
            roomNodeType);

        // Add RoomNode To RoomNodeGraph Scriptable object asset database
        AssetDatabase.AddObjectToAsset(roomNode, _currentRoomNodeGraph);
        AssetDatabase.SaveAssets();
        
        // Refresh graph node dictionary
        _currentRoomNodeGraph.OnValidate();
    }

    /// <summary>
    /// Remove relationship between child and parent
    /// Find all room nodes, that's selected and delete
    /// First remove child from parent and then parent from child
    /// </summary>
    private static void DeleteSelectedRoomNodeLinks()
    {
        var connectedRoomNodes = _currentRoomNodeGraph.roomNodeList
            .Where(roomNodeSO => roomNodeSO.isSelected && roomNodeSO.childRoomNodeIDList.Count > 0)
            .SelectMany(x => x.childRoomNodeIDList)
            .Select(childRoomNodeId => 
            (
                roomNodeSO: _currentRoomNodeGraph.roomNodeList.FindAll(x => x.childRoomNodeIDList.Contains(childRoomNodeId)),
                childRoomNodeId
            ))
            .ToList();

        connectedRoomNodes
            .ForEach(tuple =>
            {
                tuple.roomNodeSO
                    .ForEach(roomNode =>
                    {
                        roomNode.RemoveChildRoomNodeIDFromRoomNode(_currentRoomNodeGraph
                            .GetRoomNode(tuple.childRoomNodeId).id);
                        
                        _currentRoomNodeGraph.GetRoomNode(tuple.childRoomNodeId)
                            .RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    });
            });
        
        ClearAllSelectedRoomNodes();
    }

    private static void DeleteSelectedRoomNodes()
    {
        var roomNodeDeletionQueue = new Queue<RoomNodeSO>();
        
        var roomsToDelete = _currentRoomNodeGraph.roomNodeList
            .Where(roomNode => roomNode.isSelected && !roomNode.roomNodeType.isEntrance)
            .ToList();

        roomsToDelete.ForEach(roomNode =>
        {
            roomNodeDeletionQueue.Enqueue(roomNode);
        
            roomNode.childRoomNodeIDList
                .ForEach(childRoomNodeID => _currentRoomNodeGraph
                    .GetRoomNode(childRoomNodeID)?.RemoveParentRoomNodeIDFromRoomNode(roomNode.id));
            
            roomNode.parentRoomNodeIDList
                .ForEach(parentRoomNodeID => _currentRoomNodeGraph
                    .GetRoomNode(parentRoomNodeID)?.RemoveChildRoomNodeIDFromRoomNode(roomNode.id));
        });
  
        while (roomNodeDeletionQueue.Count > 0)
        {
            var RoomNodeToDelete = roomNodeDeletionQueue.Dequeue();
            _currentRoomNodeGraph.RoomNodeDictionary.Remove(RoomNodeToDelete.id);
            _currentRoomNodeGraph.roomNodeList.Remove(RoomNodeToDelete);
            
            DestroyImmediate(RoomNodeToDelete, true);
            
            AssetDatabase.SaveAssets();
        }
    }

    private static void ClearAllSelectedRoomNodes()
        =>_currentRoomNodeGraph.roomNodeList
            .Where(x => x.isSelected)
            .Select(x =>
            {
                GUI.changed = true;
                return x.isSelected = false;
            });
    

    /// <summary>
    /// Select all room nodes
    /// </summary>
    private static void SelectedAllRoomNodes()
    {
        // LINQ operations are usually evaluated lazily, ensure that the LINQ operation is executed immediately
        _ =_currentRoomNodeGraph.roomNodeList.Select(x => x.isSelected = true).ToList(); 
        GUI.changed = true;
    }


    #endregion
    
    #region LineHandlers

     private static void ClearLineDrag()
    {
        _currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        _currentRoomNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }



    /// <summary>
    /// Draw connection line between the parent and child room node
    /// </summary>
    private static void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)
    {
        var startPosition = parentRoomNode.rect.center;
        var endPosition = childRoomNode.rect.center;

        var midPosition = (endPosition + startPosition) / 2f;
        var direction = endPosition - startPosition;
        
        // Calculate normalised perpendicular positions from the mid point
        var perpendicular = new Vector2(-direction.y, direction.x).normalized;
        var arrowTailPoint1 = midPosition - perpendicular * ConnectingLineArrowSize;
        var arrowTailPoint2 = midPosition + perpendicular * ConnectingLineArrowSize;

        var arrowHeadPoint = midPosition + direction.normalized * ConnectingLineArrowSize;
        
        Handles.DrawBezier(
            arrowHeadPoint,
            arrowTailPoint1,
            arrowHeadPoint,
            arrowTailPoint1,
            Color.white,
            null,
            ConnectingLineWidth);
        
        Handles.DrawBezier(
            arrowHeadPoint,
            arrowTailPoint2,
            arrowHeadPoint,
            arrowTailPoint2,
            Color.white,
            null,
            ConnectingLineWidth);

        Handles.DrawBezier(
            startPosition,
            endPosition,
            startPosition,
            endPosition,
            Color.white,
            null,
            ConnectingLineWidth);
        
        GUI.changed = true;
    }

    #endregion

    #region Events

    /// <summary>
    /// Process Room Node Graph Events
    /// </summary>
    private void ProcessRoomNodeGraphEvents(Event currentEvent)
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

    /// <summary>
    /// Process mouse down events on the room node graph, this does not include nodes
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        if (IsRightClicked(currentEvent))
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
        else if (IsLeftClicked(currentEvent))
        {
            //ClearLineDrag();
            ClearAllSelectedRoomNodes();
        }
    }

    
    private static void ProcessMouseUpEvent(Event currentEvent)
    {
        if (currentEvent.button == 1 || _currentRoomNodeGraph.roomNodeToDrawLineFrom is null) return;
        
        // Get First room node
        SetNodeRelationship(IsMouseOverRoomNode(currentEvent));
        
        ClearLineDrag();
    }
    
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        if (IsRightClicked(currentEvent))
        {
            ProcessRightMouseDragEvent(currentEvent);
        }
        else if (IsLeftClicked(currentEvent))
        {
            ProcessLeftMouseDragEvent(currentEvent.delta);
        }
    }

    /// <summary>
    /// Draw the line
    /// </summary>
    private static void ProcessRightMouseDragEvent(Event currentEvent)
    {
        if (_currentRoomNodeGraph.roomNodeToDrawLineFrom is null) return;
        _currentRoomNodeGraph.linePosition += currentEvent.delta;
        GUI.changed = true;
    }
    
    private void ProcessLeftMouseDragEvent(Vector2 dragDelta)
    {
        _graphDrag = dragDelta;
        _currentRoomNodeGraph.roomNodeList.ForEach(x=> x.DragNode(dragDelta));
        GUI.changed = true;
    }
    
    private static void SetNodeRelationship(RoomNodeSO roomNode)
    {
        if (roomNode is null) return;
        // Maybe muse a pipe here instead. Looks really bad
        // Set the it as a child of the parent room node 
        if (_currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeID(roomNode.id))
        {
            // Set parent ID in child room node
            roomNode.AddParentRoomNodeID(_currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
        }
    }
    
    /// <summary>
    /// Check To See 
    /// </summary>
    /// <param name="currentEvent"></param>
    /// <returns></returns>
    private static RoomNodeSO IsMouseOverRoomNode(Event currentEvent) =>
        _currentRoomNodeGraph.roomNodeList
            .FirstOrDefault(x => x.rect.Contains(currentEvent.mousePosition));
    
    private static bool IsLeftClicked(Event currentEvent) => currentEvent.button == 0;
    private static bool IsRightClicked(Event currentEvent) => currentEvent.button == 1;

    #endregion
    
}
