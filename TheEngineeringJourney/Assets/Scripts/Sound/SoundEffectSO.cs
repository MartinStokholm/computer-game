using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SoundEffect_", menuName = "Scriptable Objects/Sounds/SoundEffect")]
public class SoundEffectSO : ScriptableObject
{
    #region Header SOUND EFFECT DETAILS
    [Space(10)]
    [Header("SOUND EFFECT DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("The name for the sound effect")]
    #endregion
    public string SoundEffectName;
    #region Tooltip
    #endregion
    public GameObject SoundPrefab;
    #region Tooltip
    #endregion
    public AudioClip SoundEffectClip;
    #region Tooltip
    [Tooltip("The minimum pitch variation for the sound effect.  A random pitch variation will be generated between the minimum and maximum values.  A random pitch variation makes sound effects sound more natural.")]
    #endregion
    [Range(0.1f, 1.5f)]
    public float SoundEffectPitchRandomVariationMin = 0.8f;
    #region Tooltip

    [Tooltip("The maximum pitch variation for the sound effect.  A random pitch variation will be generated between the minimum and maximum values.  A random pitch variation makes sound effects sound more natural.")]
    #endregion
    [Range(0.1f, 1.5f)]
    public float SoundEffectPitchRandomVariationMax = 1.2f;
    #region Tooltip
    [Tooltip("The sound effect volume.")]
    #endregion
    [Range(0f, 1f)]
    public float SoundEffectVolume = 1f;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        EditorUtilities.ValidateCheckEmptyString(this, nameof(SoundEffectName), SoundEffectName);
        EditorUtilities.ValidateCheckNullValue(this, nameof(SoundPrefab), SoundPrefab);
        EditorUtilities.ValidateCheckNullValue(this, nameof(SoundEffectClip), SoundEffectClip);
        EditorUtilities.ValidateCheckPositiveRange(this, nameof(SoundEffectPitchRandomVariationMin), SoundEffectPitchRandomVariationMin, nameof(SoundEffectPitchRandomVariationMax), SoundEffectPitchRandomVariationMax, false);
        EditorUtilities.ValidateCheckPositiveValue(this, nameof(SoundEffectVolume), (int)SoundEffectVolume, true);
    }
#endif
    #endregion
}
