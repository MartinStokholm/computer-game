using UnityEngine;

public static class Settings 
{
    #region MAP BUILD SETTINGS
    public const int MaxMapRebuildAttemptsForRoomGraph = 1000;
    public const int MaxMapBuildAttempts = 10;
    #endregion
    
    #region ROOM SETTING

    // Amount of child corridors leading from a room
    public const int MaxChildCorridors = 3;
    
    #endregion

    #region ANIMATOR PARAMETERS

    public static int AimUp = Animator.StringToHash("AimUp");
    public static int AimDown = Animator.StringToHash("AimDown");
    public static int AimUpRight = Animator.StringToHash("AimUpRight");
    public static int AimUpLeft = Animator.StringToHash("AimUpLeft");
    public static int AimRight = Animator.StringToHash("AimRight");
    public static int AimLeft = Animator.StringToHash("AimLeft");
    public static int IsIdle = Animator.StringToHash("IsIdle");
    public static int IsMoving = Animator.StringToHash("IsMoving");

    #endregion
    
    #region GAMEOBJECT TAGS
    public const string PlayerTag = "Player";
    #endregion
    
    #region ASTAR PATHFINDING PARAMETERS
    public const int DefaultAStarMovementPenalty = 40;
    public const int PreferredPathAStarMovementPenalty = 1;
    public const int TargetFrameRateToSpreadPathfindingOver = 60;
    public const float PlayerMoveDistanceToRebuildPath = 3f;
    public const float EnemyPathRebuildCooldown = 2f;
    #endregion

    #region PlayerLevel
    public const int ExperienceToLevelUp = 100;
    #endregion
}
