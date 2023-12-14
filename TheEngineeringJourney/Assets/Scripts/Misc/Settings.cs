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

    #region AUDIO
    public const float MusicFadeOutTime = 0.5f;  // Default Music Fade Out Transition
    public const float MusicFadeInTime = 0.5f;  // Default Music Fade In Transition
    #endregion
    
    #region ANIMATOR PARAMETERS

    public static int AimUp = Animator.StringToHash("AimUp");
    public static int AimDown = Animator.StringToHash("AimDown");
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
    
    #region FIRING CONTROL
    public const float UseAimAngleDistance = 3.5f; // if the target distance is less than this then the aim angle will be used (calculated from player), else the weapon aim angle will be used (calculated from the weapon). 
    #endregion
    
    #region ENEMY PARAMETERS
    public const int DefaultEnemyHealth = 20;
    #endregion
    
    #region CONTACT DAMAGE PARAMETERS
    public const float ContactDamageCollisionResetDelay = 0.5f;
    #endregion
}
