using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "EnemyDetails_", menuName = "Scriptable Objects/Enemy/EnemyDetails")]
public class EnemyDetailsSO : ScriptableObject
{
    #region Header BASE ENEMY DETAILS
    [FormerlySerializedAs("enemyName")]
    [Space(10)]
    [Header("BASE ENEMY DETAILS")]
    #endregion

    #region Tooltip
    [Tooltip("Enemy name")]
    #endregion
    public string EnemyName;

    #region Tooltip
    [FormerlySerializedAs("EnemyPrefab")] [Tooltip("The prefab for the enemy")]
    #endregion
    public GameObject enemyPrefab;

    #region Tooltip
    [FormerlySerializedAs("chaseDistance")] [Tooltip("Distance to the player before enemy starts chasing")]
    #endregion
    public float ChaseDistance = 50f;

    #region Header ENEMY MATERIAL
    [Space(10)]
    [Header("ENEMY MATERIAL")]
    #endregion
    #region Tooltip
    [Tooltip("This is the standard lit shader material for the enemy (used after the enemy materializes")]
    #endregion
    public Material enemyStandardMaterial;

    #region Header ENEMY MATERIALIZE SETTINGS
    [Space(10)]
    [Header("ENEMY MATERIALIZE SETTINGS")]
    #endregion
    #region Tooltip
    [Tooltip("The time in seconds that it takes the enemy to materialize")]
    #endregion
    public float enemyMaterializeTime;
    #region Tooltip
    [Tooltip("The shader to be used when the enemy materializes")]
    #endregion
    public Shader enemyMaterializeShader;
    [ColorUsage(true, true)]
    #region Tooltip
    [Tooltip("The colour to use when the enemy materializes.  This is an HDR color so intensity can be set to cause glowing / bloom")]
    #endregion
    public Color enemyMaterializeColor;
    
    #region Header ENEMY WEAPON SETTINGS
    [Space(10)]
    [Header("ENEMY WEAPON SETTINGS")]
    #endregion
    #region Tooltip
    [Tooltip("The weapon for the enemy - none if the enemy doesn't have a weapon")]
    #endregion
    public WeaponDetailsSO EnemyWeapon;
    #region Tooltip
    [Tooltip("The minimum time delay interval in seconds between bursts of enemy shooting.  This value should be greater than 0. A random value will be selected between the minimum value and the maximum value")]
    #endregion
    public float FiringIntervalMin = 0.1f;
    #region Tooltip
    [Tooltip("The maximum time delay interval in seconds between bursts of enemy shooting.  A random value will be selected between the minimum value and the maximum value")]
    #endregion
    public float FiringIntervalMax = 1f;
    #region Tooltip
    [Tooltip("The minimum firing duration that the enemy shoots for during a firing burst.  This value should be greater than zero.  A random value will be selected between the minimum value and the maximum value.")]
    #endregion
    public float FiringDurationMin = 1f;
    #region Tooltip
    [Tooltip("The maximum firing duration that the enemy shoots for during a firing burst.  A random value will be selected between the minimum value and the maximum value.")]
    #endregion
    public float FiringDurationMax = 2f;
    #region Tooltip
    [Tooltip("Select this if line of sight is required of the player before the enemy fires.  If line of sight isn't selected the enemy will fire regardless of obstacles whenever the player is 'in range'")]
    #endregion
    public bool FiringLineOfSightRequired;
    
    #region Header ENEMY HEALTH
    [Space(10)]
    [Header("ENEMY HEALTH")]
    #endregion
    #region Tooltip
    [Tooltip("The health of the enemy for each level")]
    #endregion
    public EnemyHealthDetails[] EnemyHealthDetailsArray;
    #region Tooltip
    [Tooltip("Select if has immunity period immediately after being hit.  If so specify the immunity time in seconds in the other field")]
    #endregion
    public bool IsImmuneAfterHit = false;
    #region Tooltip
    [Tooltip("Immunity time in seconds after being hit")]
    #endregion
    public float HitImmunityTime;
    #region Tooltip
    [Tooltip("Select to display a health bar for the enemy")]
    #endregion
    public bool IsHealthBarDisplayed = false;



    #region Validation
#if UNITY_EDITOR
    // Validate the scriptable object details entered
    private void OnValidate()
    {
        EditorUtilities.ValidateCheckEmptyString(this, nameof(EnemyName), EnemyName);
        EditorUtilities.ValidateCheckNullValue(this, nameof(enemyPrefab), enemyPrefab);
        EditorUtilities.ValidateCheckPositiveValue(this, nameof(ChaseDistance), (int)ChaseDistance, false); 
        EditorUtilities.ValidateCheckNullValue(this, nameof(enemyStandardMaterial), enemyStandardMaterial);
        EditorUtilities.ValidateCheckPositiveValue(this, nameof(enemyMaterializeTime), enemyMaterializeTime, true);
        EditorUtilities.ValidateCheckNullValue(this, nameof(enemyMaterializeShader), enemyMaterializeShader);
        EditorUtilities.ValidateCheckPositiveRange(
            this, 
            nameof(FiringIntervalMin), 
            FiringIntervalMin, 
            nameof(FiringIntervalMax), 
            FiringIntervalMax, 
            false);
        EditorUtilities.ValidateCheckPositiveRange(
            this, 
            nameof(FiringDurationMin), 
            FiringDurationMin, 
            nameof(FiringDurationMax), 
            FiringDurationMax, 
            false);
        EditorUtilities.ValidateCheckEnumerableValues(this, nameof(EnemyHealthDetailsArray), EnemyHealthDetailsArray);
        if (IsImmuneAfterHit)
        {
             EditorUtilities.ValidateCheckPositiveValue(this, nameof(HitImmunityTime), (int)HitImmunityTime, false);
        }
    }

#endif
    #endregion

}

public static class EnemyDetailsSOHelper
{
    /// <summary>
    /// Calculate a random weapon shoot interval between the min and max values
    /// </summary>
    private static void WeaponShootInterval(this EnemyDetailsSO enemyDetailsSo) =>
        Random.Range(enemyDetailsSo.FiringIntervalMin, enemyDetailsSo.FiringIntervalMax);

}