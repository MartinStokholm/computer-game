using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle _roomNodeStyle;
    private static RoomNodeGraphSO _currentRoomNodeGraph;
    private RoomNodeSO _currentRoomNode;
    private RoomNodeTypeListSO _roomNodeTypeList;
    
    // Node layout values
    private const float NodeWidth = 160f;
    private const float NodeHeight = 75f;
    private const int NodePadding = 25;
    private const int NodeBorder = 12;
    
    // Connecting line values
    private const float connectingLineWidth = 3f;
    
    [MenuItem("Room Node Graph Editor", menuItem = "Window/Map Editor/Room Node Graph Editor")]
    private static void OpenWindow() => GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");

    // Define node layout style
    private void OnEnable()
    {
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
        
        // Load Room node types
        _roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }


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

    /// <summary>
    ///  Draw Editor GUI
    /// </summary>
    private void OnGUI()
    {
        // If a scriptable object of type RoomNodeGraphSO has been selected then process
        if (_currentRoomNodeGraph is not null)
        {
            DrawDraggedLine();
            ProcessEvents(Event.current);
            DrawRoomConnections();
            DrawRoomNodes();
        }
        
        if (GUI.changed)
            Repaint();
    }

    private void DrawDraggedLine()
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
                connectingLineWidth);
        }
    }

    private void ProcessEvents(Event currentEvent)
    {
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
    /// Check To See 
    /// </summary>
    /// <param name="currentEvent"></param>
    /// <returns></returns>
    private static RoomNodeSO IsMouseOverRoomNode(Event currentEvent) =>
        _currentRoomNodeGraph.roomNodeList
            .FirstOrDefault(x => x.rect.Contains(currentEvent.mousePosition));

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
        if (currentEvent.button != 1) return;
        ShowContextMenu(currentEvent.mousePosition);
    }

    /// <summary>
    /// Show the context menu
    /// </summary>
    private void ShowContextMenu(Vector2 mousePosition)
    {
        var menu = new GenericMenu();
        
        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        menu.ShowAsContext();
    }

    /// <summary>
    /// Create a room node at the mouse position
    /// </summary>
    private void CreateRoomNode(object mousePositionObject)
    {
        var mousePosition = (Vector2)mousePositionObject;
        
        // Create RoomNode Scriptable Object Asset
        var roomNode = CreateInstance<RoomNodeSO>();
        
        // Add RoomNode tp current RoomNodeGraphList
        _currentRoomNodeGraph.roomNodeList.Add(roomNode);
        
        // Set RoomNode values
        roomNode.Initialise(new Rect(mousePosition, new Vector2(NodeWidth, NodeWidth)), _currentRoomNodeGraph, _roomNodeTypeList.list.Find(x => x.isNone));

        // Add RoomNode To RoomNodeGraph Scriptable object asset database
        AssetDatabase.AddObjectToAsset(roomNode, _currentRoomNodeGraph);
        AssetDatabase.SaveAssets();
        
        // Refresh graph node dictionary
        _currentRoomNodeGraph.OnValidate();
    }

    private static void ProcessMouseUpEvent(Event currentEvent)
    {
        if (currentEvent.button == 1 || _currentRoomNodeGraph.roomNodeToDrawLineFrom is null) return;
        
        // Get First room node
        SetNodeRelationship(IsMouseOverRoomNode(currentEvent));
        
        ClearLineDrag();
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

    private static void ClearLineDrag()
    {
        _currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        _currentRoomNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }

    private void DrawRoomConnections()
    {
        // _currentRoomNodeGraph.roomNodeList
        //     .Where(x => x.childRoomNodeIDList.Count > 0)
        //     .Select(x => _currentRoomNode.roomNodeGraph.ContainsKey(x.childRoomNodeIDList);

        // .Where(nodeType => nodeType.displayInNodeGraphEditor)
        // .Select(nodeType => nodeType.roomNodeTypeName)
        // .ToArray();
    }

    private static void ProcessMouseDragEvent(Event currentEvent)
    {
        if (currentEvent.button != 1) return;
        ProcessRightMouseDragEvent(currentEvent);
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

    /// <summary>
    /// Draw RoomNodes In GraphWindow
    /// </summary>
    private void DrawRoomNodes()
    {
        _currentRoomNodeGraph.roomNodeList.ForEach(x => x.Draw(_roomNodeStyle));
        GUI.changed = true;
    }
}
