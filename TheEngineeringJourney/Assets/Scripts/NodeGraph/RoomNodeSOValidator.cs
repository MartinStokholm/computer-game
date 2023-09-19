using System.Collections.Generic;
using System.Linq;

namespace NodeGraph
{
    public static class RoomNodeSOValidator
    {
        public static bool IsChildRoomValid(string currentId, string childId, RoomNodeGraphSO roomNodeGraph, ICollection<string>  childRoomNodeIDList, RoomNodeTypeSO roomNodeType)
        {
            var childNode = roomNodeGraph.GetRoomNode(childId);
        
            // The child is a type boss room and there already is a boss room return false
            if (IsAlreadyBossRoom(childNode, IsConnectedBoosNodeAlready(roomNodeGraph))) return false;
            if (IsRoomTypeNone(childNode)) return false;
            if (IsAlreadyChild(childRoomNodeIDList, childId)) return false;
            if (IsCurrentItSelf(currentId, childId)) return false;
            if (HasParent(childNode)) return false;
            if (IsChildACorridorAndCurrentACorridor(childNode, roomNodeType)) return false;
            if (IsChildARoomAndCurrentARoom(childNode, roomNodeType)) return false;
            if (HasRoomMaximumCorridors(childNode, childRoomNodeIDList)) return false;
            if (IsEntrance(childNode)) return false;
            if (IsCorridorAlreadyConnectedToRoom(childRoomNodeIDList, childNode)) return false;
        
            return true;
        }

        #region ChildValidator

        private static bool IsEntrance(RoomNodeSO childNode) => childNode.roomNodeType.isEntrance;

        private static bool IsCurrentItSelf(string id, string childId) => id == childId;
        
        private static bool IsRoomTypeNone(RoomNodeSO childNode) => childNode.roomNodeType.isNone;
        private static bool HasParent(RoomNodeSO childNode) => childNode.parentRoomNodeIDList.Count > 0;
        
        private static bool IsCorridorAlreadyConnectedToRoom(ICollection<string> childRoomNodeIDList, RoomNodeSO childNode) 
            => !childNode.roomNodeType.isCorridor && childRoomNodeIDList.Count > 0;
        
        private static bool IsAlreadyChild(ICollection<string> childRoomNodeIDList, string childId) =>
            childRoomNodeIDList.Contains(childId);
        
        private static bool IsChildACorridorAndCurrentACorridor(RoomNodeSO childNode, RoomNodeTypeSO roomNodeType) 
            => childNode.roomNodeType.isCorridor && roomNodeType.isCorridor;
        private static bool IsChildARoomAndCurrentARoom(RoomNodeSO childNode, RoomNodeTypeSO roomNodeType) 
            => !childNode.roomNodeType.isCorridor && !roomNodeType.isCorridor;
        private static bool HasRoomMaximumCorridors(RoomNodeSO childNode, ICollection<string> childRoomNodeIDList) =>
            childNode.roomNodeType.isCorridor && childRoomNodeIDList.Count >= Settings.MaxChildCorridors;
        
        private static bool IsConnectedBoosNodeAlready(RoomNodeGraphSO roomNodeGraph) 
            => roomNodeGraph.roomNodeList
                .Any(x => x.roomNodeType.isBossRoom && x.parentRoomNodeIDList.Count > 0);
        
        private static bool IsAlreadyBossRoom(RoomNodeSO childNode, bool isConnectedBoosNodeAlready) =>
            childNode.roomNodeType.isBossRoom && isConnectedBoosNodeAlready;
        #endregion
       
    }
}