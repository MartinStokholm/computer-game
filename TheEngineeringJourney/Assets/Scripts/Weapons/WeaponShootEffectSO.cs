using UnityEngine;

[CreateAssetMenu(fileName = "WeaponShootEffect_", menuName = "Scriptable Objects/Weapons/Weapon Shoot Effect")]
public class WeaponShootEffectSO : ScriptableObject
{
    #region Header WEAPON SHOOT EFFECT DETAILS
    [Space(10)]
    [Header("WEAPON SHOOT EFFECT DETAILS")]
    #endregion Header WEAPON SHOOT EFFECT DETAILS

    #region Tooltip
    [Tooltip("The color gradient for the shoot effect.  This gradient show the color of particles during their lifetime - from left to right ")]
    #endregion Tooltip
    public Gradient ColorGradient;

    #region Tooltip
    [Tooltip("The length of time the particle system is emitting particles")]
    #endregion Tooltip
    public float Duration = 0.50f;

    #region Tooltip
    [Tooltip("The start particle size for the particle effect")]
    #endregion Tooltip
    public float StartParticleSize = 0.25f;

    #region Tooltip
    [Tooltip("The start particle speed for the particle effect")]
    #endregion Tooltip
    public float StartParticleSpeed = 3f;

    #region Tooltip
    [Tooltip("The particle lifetime for the particle effect")]
    #endregion Tooltip
    public float StartLifetime = 0.5f;

    #region Tooltip
    [Tooltip("The maximum number of particles to be emitted")]
    #endregion Tooltip
    public int MaxParticleNumber = 100;

    #region Tooltip
    [Tooltip("The number of particles emitted per second. If zero it will just be the burst number")]
    #endregion Tooltip
    public int EmissionRate = 100;

    #region Tooltip
    [Tooltip("How many particles should be emitted in the particle effect burst")]
    #endregion Tooltip
    public int BurstParticleNumber = 20;

    #region Tooltip
    [Tooltip("The gravity on the particles - a small negative number will make them float up")]
    #endregion
    public float EffectGravity = -0.01f;

    #region Tooltip
    [Tooltip("The sprite for the particle effect.  If none is specified then the default particle sprite will be used")]
    #endregion Tooltip
    public Sprite Sprite;

    #region Tooltip
    [Tooltip("The min velocity for the particle over its lifetime. A random value between min and max will be generated.")]
    #endregion Tooltip
    public Vector3 VelocityOverLifetimeMin;

    #region Tooltip
    [Tooltip("The max velocity for the particle over its lifetime. A random value between min and max will be generated.")]
    #endregion Tooltip
    public Vector3 VelocityOverLifetimeMax;

    #region Tooltip
    [Tooltip("weaponShootEffectPrefab contains the particle system for the shoot effect - and is configured by the weaponShootEffect SO")]
    #endregion Tooltip
    public GameObject WeaponShootEffectPrefab;


    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        EditorUtilities.ValidateCheckPositiveValue(this, nameof(Duration), Duration, false);
        EditorUtilities.ValidateCheckPositiveValue(this, nameof(StartParticleSize), StartParticleSize, false);
        EditorUtilities.ValidateCheckPositiveValue(this, nameof(StartParticleSpeed), StartParticleSpeed, false);
        EditorUtilities.ValidateCheckPositiveValue(this, nameof(StartLifetime), StartLifetime, false);
        EditorUtilities.ValidateCheckPositiveValue(this, nameof(MaxParticleNumber), MaxParticleNumber, false);
        EditorUtilities.ValidateCheckPositiveValue(this, nameof(EmissionRate), EmissionRate, true);
        EditorUtilities.ValidateCheckPositiveValue(this, nameof(BurstParticleNumber), BurstParticleNumber, true);
        EditorUtilities.ValidateCheckNullValue(this, nameof(WeaponShootEffectPrefab), WeaponShootEffectPrefab);
    }

#endif

    #endregion Validation
}