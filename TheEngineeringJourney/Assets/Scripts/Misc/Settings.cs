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

    public static int AimUp = Animator.StringToHash("aimUp");
    public static int AimDown = Animator.StringToHash("aimDown");
    public static int AimUpRight = Animator.StringToHash("aimUpRight");
    public static int AimUpLeft = Animator.StringToHash("aimUpLeft");
    public static int AimRight = Animator.StringToHash("aimRight");
    public static int AimLeft = Animator.StringToHash("aimLeft");
    public static int IsIdle = Animator.StringToHash("isIdle");
    public static int IsMoving = Animator.StringToHash("isMoving");

    #endregion
}
