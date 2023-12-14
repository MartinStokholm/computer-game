
using UnityEngine;

[CreateAssetMenu(fileName = "AmmoDetails_", menuName = "Scriptable Objects/Weapons/Ammo Details")]
public class AmmoDetailsSO : ScriptableObject
{
    #region Header BASIC AMMO DETAILS

    [Space(10)]
    [Header("BASIC AMMO DETAILS")]

    #endregion

    #region Tooltip

    [Tooltip("Name for the ammo")]

    #endregion

    public string AmmoName;

    public bool isPlayerAmmo;

    #region Header AMMO SPRITE, PREFAB & MATERIALS

    [Space(10)]
    [Header("AMMO SPRITE, PREFAB & MATERIALS")]

    #endregion

    #region Tooltip

    [Tooltip("Sprite to be used for the ammo")]

    #endregion

    public Sprite AmmoSprite;

    #region Tooltip

    [Tooltip(
        "Populate with the prefab to be used for the ammo.  If multiple prefabs are specified then a random prefab from the array will be selecetd.  The prefab can be an ammo pattern - as long as it conforms to the IFireable interface.")]

    #endregion

    public GameObject[] AmmoPrefabArray;

    #region Tooltip

    [Tooltip("The material to be used for the ammo")]

    #endregion

    public Material AmmoMaterial;

    #region Tooltip

    [Tooltip(
        "If the ammo should 'charge' briefly before moving then set the time in seconds that the ammo is held charging after firing before release")]

    #endregion

    public float AmmoChargeTime = 0.1f;

    #region Tooltip

    [Tooltip(
        "If the ammo has a charge time then specify what material should be used to render the ammo while charging")]

    #endregion

    public Material AmmoChargeMaterial;

    #region Header AMMO HIT EFFECT

    [Space(10)]
    [Header("AMMO HIT EFFECT")]

    #endregion

    #region Tooltip

    [Tooltip("The scriptable object that defines the parameters for the hit effect prefab")]

    #endregion

    public AmmoHitEffectSO AmmoHitEffect;

    #region Header AMMO BASE PARAMETERS

    [Space(10)]
    [Header("AMMO BASE PARAMETERS")]

    #endregion

    #region Tooltip

    [Tooltip("The damage each ammo deals")]

    #endregion

    public int AmmoDamage = 1;

    #region Tooltip

    [Tooltip("The minimum speed of the ammo - the speed will be a random value between the min and max")]

    #endregion

    public float AmmoSpeedMin = 20f;

    #region Tooltip

    [Tooltip("The maximum speed of the ammo - the speed will be a random value between the min and max")]

    #endregion

    public float AmmoSpeedMax = 20f;

    #region Tooltip

    [Tooltip("The range of the ammo (or ammo pattern) in unity units")]

    #endregion

    public float AmmoRange = 20f;

    #region Tooltip

    [Tooltip("The rotation speed in degrees per second of the ammo pattern")]

    #endregion

    public float AmmoRotationSpeed = 1f;

    #region Header AMMO SPREAD DETAILS

    [Space(10)]
    [Header("AMMO SPREAD DETAILS")]

    #endregion

    #region Tooltip

    [Tooltip(
        "This is the  minimum spread angle of the ammo.  A higher spread means less accuracy. A random spread is calculated between the min and max values.")]

    #endregion

    public float AmmoSpreadMin = 0f;

    #region Tooltip

    [Tooltip(
        " This is the  maximum spread angle of the ammo.  A higher spread means less accuracy. A random spread is calculated between the min and max values. ")]

    #endregion

    public float AmmoSpreadMax = 0f;

    #region Header AMMO SPAWN DETAILS

    [Space(10)]
    [Header("AMMO SPAWN DETAILS")]

    #endregion

    #region Tooltip

    [Tooltip(
        "This is the minimum number of ammo that are spawned per shot. A random number of ammo are spawned between the minimum and maximum values. ")]

    #endregion

    public int AmmoSpawnAmountMin = 1;

    #region Tooltip

    [Tooltip(
        "This is the maximum number of ammo that are spawned per shot. A random number of ammo are spawned between the minimum and maximum values. ")]

    #endregion

    public int AmmoSpawnAmountMax = 1;

    #region Tooltip

    [Tooltip(
        "Minimum spawn interval time. The time interval in seconds between spawned ammo is a random value between the minimum and maximum values specified.")]

    #endregion

    public float AmmoSpawnIntervalMin = 0f;

    #region Tooltip

    [Tooltip(
        "Maximum spawn interval time. The time interval in seconds between spawned ammo is a random value between the minimum and maximum values specified.")]

    #endregion

    public float AmmoSpawnIntervalMax = 0f;


    #region Header AMMO TRAIL DETAILS

    [Space(10)]
    [Header("AMMO TRAIL DETAILS")]

    #endregion

    #region Tooltip

    [Tooltip(
        "Selected if an ammo trail is required, otherwise deselect.  If selected then the rest of the ammo trail values should be populated.")]

    #endregion

    public bool IsAmmoTrail = false;

    #region Tooltip

    [Tooltip("Ammo trail lifetime in seconds.")]

    #endregion

    public float AmmoTrailTime = 3f;

    #region Tooltip

    [Tooltip("Ammo trail material.")]

    #endregion

    public Material AmmoTrailMaterial;

    #region Tooltip

    [Tooltip("The starting width for the ammo trail.")]

    #endregion

    [Range(0f, 1f)]
    public float AmmoTrailStartWidth;

    #region Tooltip

    [Tooltip("The ending width for the ammo trail")]

    #endregion

    [Range(0f, 1f)]
    public float AmmoTrailEndWidth;

    #region Validation

#if UNITY_EDITOR
    // Validate the scriptable object details entered
    private void OnValidate()
    {
        EditorUtilities.ValidateCheckEmptyString(this, nameof(AmmoName), AmmoName);
        EditorUtilities.ValidateCheckNullValue(this, nameof(AmmoSprite), AmmoSprite);
        EditorUtilities.ValidateCheckEnumerableValues(this, nameof(AmmoPrefabArray), AmmoPrefabArray);
        EditorUtilities.ValidateCheckNullValue(this, nameof(AmmoMaterial), AmmoMaterial);
        if (AmmoChargeTime > 0)
            EditorUtilities.ValidateCheckNullValue(this, nameof(AmmoChargeMaterial), AmmoChargeMaterial);
        EditorUtilities.ValidateCheckPositiveValue(this, nameof(AmmoDamage), AmmoDamage, false);
        EditorUtilities.ValidateCheckPositiveRange(this, nameof(AmmoSpeedMin), AmmoSpeedMin, nameof(AmmoSpeedMax),
            AmmoSpeedMax, false);
        EditorUtilities.ValidateCheckPositiveValue(this, nameof(AmmoRange), (int)AmmoRange, false);
        EditorUtilities.ValidateCheckPositiveRange(this, nameof(AmmoSpreadMin), AmmoSpreadMin, nameof(AmmoSpreadMax),
            AmmoSpreadMax, true);
        EditorUtilities.ValidateCheckPositiveRange(this, nameof(AmmoSpawnAmountMin), AmmoSpawnAmountMin,
            nameof(AmmoSpawnAmountMax), AmmoSpawnAmountMax, false);
        EditorUtilities.ValidateCheckPositiveRange(this, nameof(AmmoSpawnIntervalMin), AmmoSpawnIntervalMin,
            nameof(AmmoSpawnIntervalMax), AmmoSpawnIntervalMax, true);

        if (!IsAmmoTrail) return;
        EditorUtilities.ValidateCheckPositiveValue(this, nameof(AmmoTrailTime), (int)AmmoTrailTime, false);
        EditorUtilities.ValidateCheckNullValue(this, nameof(AmmoTrailMaterial), AmmoTrailMaterial);
        EditorUtilities.ValidateCheckPositiveValue(this, nameof(AmmoTrailStartWidth), (int)AmmoTrailStartWidth, false);
        EditorUtilities.ValidateCheckPositiveValue(this, nameof(AmmoTrailEndWidth), (int)AmmoTrailEndWidth, false);
    }

#endif

    #endregion
}