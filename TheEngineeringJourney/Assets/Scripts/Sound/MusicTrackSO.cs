using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "MusicTrack_", menuName = "Scriptable Objects/Sounds/MusicTrack")]
public class MusicTrackSO : ScriptableObject
{
    #region Header MUSIC TRACK DETAILS
    [FormerlySerializedAs("musicName")]
    [Space(10)]
    [Header("MUSIC TRACK DETAILS")]
    #endregion

    #region Tooltip
    [Tooltip("The name for the music track")]
    #endregion
    public string MusicName;

    #region Tooltip
    [FormerlySerializedAs("musicClip")] [Tooltip("The audio clip for the music track")]
    #endregion
    public AudioClip MusicClip;

    #region Tooltip
    [FormerlySerializedAs("musicVolume")]
    [Tooltip("The volume for the music track")]
    #endregion
    [Range(0, 1)]
    public float MusicVolume = 1f;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        EditorUtilities.ValidateCheckEmptyString(this, nameof(MusicName), MusicName);
        EditorUtilities.ValidateCheckNullValue(this, nameof(MusicClip), MusicClip);
        EditorUtilities.ValidateCheckPositiveValue(this, nameof(MusicVolume), (int)MusicVolume, true);
    }
#endif
    #endregion
}
