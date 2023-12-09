using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class GameResources : MonoBehaviour
{
    private static GameResources _instance;

    public static GameResources Instance => _instance ?? Resources.Load<GameResources>("GameResources");

    #region Header Map
    [FormerlySerializedAs("roomNodeTypeList")]
    [FormerlySerializedAs("roomNodeTypeListSo")]
    [Space(10)]
    [Header("Map")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with map RoomNodeTypeListSO")]
    #endregion
    public RoomNodeTypeListSO RoomNodeTypes;
    
    #region Header PLAYER
    [FormerlySerializedAs("currentPlayer")]
    [Space(10)]
    [Header("PLAYER")]
    #endregion Header PLAYER
    #region Tooltip
    [Tooltip("The current player scriptable object - this is used to reference the current player between scenes")]
    #endregion Tooltip
    public CurrentPlayerSO CurrentPlayer;


    #region Header MATERIALS
    [Space(10)]
    [Header("MATERIALS")]
    #endregion
    #region Tooltip
    [Tooltip("Dimmed Material")]
    #endregion
    public Material DimmedMaterial;

    
    #region Header MUSIC
    [Space(10)]
    [Header("MUSIC")]
    #endregion Header MUSIC
    #region Tooltip
    [Tooltip("Populate with the music master mixer group")]
    #endregion
    public AudioMixerGroup MusicMasterMixerGroup;
    #region Tooltip
    [Tooltip("Main menu music scriptable object")]
    #endregion Tooltip
    public MusicTrackSO MainMenuMusic;
    #region Tooltip
    [Tooltip("music on full snapshot")]
    #endregion Tooltip
    public AudioMixerSnapshot MusicOnFullSnapshot;
    #region Tooltip
    [Tooltip("music low snapshot")]
    #endregion Tooltip
    public AudioMixerSnapshot MusicLowSnapshot;
    #region Tooltip
    [Tooltip("music off snapshot")]
    #endregion Tooltip
    public AudioMixerSnapshot MusicOffSnapshot;

    #region Header SOUNDS
    [Space(10)]
    [Header("SOUNDS")]
    #endregion Header
    #region Tooltip
    [Tooltip("Populate with the sounds master mixer group")]
    #endregion
    public AudioMixerGroup SoundsMasterMixerGroup;
    #region Tooltip
    [Tooltip("Door open close sound effect")]
    #endregion Tooltip
    public SoundEffectSO DoorOpenCloseSoundEffect;
    #region Tooltip
    [Tooltip("Populate with the chest open sound effect")]
    #endregion
    public SoundEffectSO ChestOpen;
    #region Tooltip
    [Tooltip("Populate with the health pickup sound effect")]
    #endregion
    public SoundEffectSO HealthPickup;
    
    
       #region Validation
#if UNITY_EDITOR
    // Validate the scriptable object details entered
    private void OnValidate()
    {
        EditorUtilities.ValidateCheckNullValue(this, nameof(RoomNodeTypes), RoomNodeTypes);
        //EditorUtilities.ValidateCheckNullValue(this, nameof(playerSelectionPrefab), playerSelectionPrefab);
        //EditorUtilities.ValidateCheckEnumerableValues(this, nameof(PlayerDetailsSO), PlayerDetailsSO);
        EditorUtilities.ValidateCheckNullValue(this, nameof(CurrentPlayer), CurrentPlayer);
        EditorUtilities.ValidateCheckNullValue(this, nameof(MainMenuMusic), MainMenuMusic);
        EditorUtilities.ValidateCheckNullValue(this, nameof(SoundsMasterMixerGroup), SoundsMasterMixerGroup);
        // EditorUtilities.ValidateCheckNullValue(this, nameof(variableLitShader), variableLitShader);
        // EditorUtilities.ValidateCheckNullValue(this, nameof(materializeShader), materializeShader);
        // EditorUtilities.ValidateCheckEnumerableValues(this, nameof(enemyUnwalkableCollisionTilesArray), enemyUnwalkableCollisionTilesArray);
        // EditorUtilities.ValidateCheckNullValue(this, nameof(preferredEnemyPathTile), preferredEnemyPathTile);
        EditorUtilities.ValidateCheckNullValue(this, nameof(MusicMasterMixerGroup), MusicMasterMixerGroup);
        EditorUtilities.ValidateCheckNullValue(this, nameof(MusicOnFullSnapshot), MusicOnFullSnapshot);
        EditorUtilities.ValidateCheckNullValue(this, nameof(MusicLowSnapshot), MusicLowSnapshot);
        EditorUtilities.ValidateCheckNullValue(this, nameof(MusicOffSnapshot), MusicOffSnapshot);
    }

#endif
    #endregion
}
