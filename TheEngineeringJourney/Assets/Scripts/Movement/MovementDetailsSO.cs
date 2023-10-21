using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementDetails_", menuName = "Scriptable Objects/Movement/MovementDetails")]
public class MovementDetailsSO : ScriptableObject
{
    #region Header MOVEMENT DETAILS
    [Space(10)]
    [Header("MOVEMENT DETAILS")]
    #endregion Header
    #region Tooltip
    [Tooltip("The minimum move speed. The GetMoveSpeed method calculates a random value between the minimum and maximum")]
    #endregion Tooltip
    public float minMoveSpeed = 8f;
    #region Tooltip
    [Tooltip("The maximum move speed. The GetMoveSpeed method calculates a random value between the minimum and maximum")]
    #endregion Tooltip
    public float maxMoveSpeed = 8f;
    // #region Tooltip
    // [Tooltip("If there is a roll movement- this is the roll speed")]
    // #endregion
    // public float rollSpeed; // for player
    // #region Tooltip
    // [Tooltip("If there is a roll movement - this is the roll distance")]
    // #endregion
    // public float rollDistance; // for player
    // #region Tooltip
    // [Tooltip("If there is a roll movement - this is the cooldown time in seconds between roll actions")]
    // #endregion
    // public float rollCooldownTime; // for player

    /// <summary>
    /// Get a random movement speed between the minimum and maximum values
    /// </summary>
    public float GetMovementSpeed()
    {
        return minMoveSpeed == maxMoveSpeed 
            ? minMoveSpeed 
            : Random.Range(minMoveSpeed, maxMoveSpeed);
    }
}
