using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDetails_", menuName = "Scriptable Objects/Player/Player Details")]
public class PlayerDetailsSO : ScriptableObject
{
    #region Header PLAYER BASE DETAILS
    [Space(10)]
    [Header("PLAYER BASE DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("Player character name.")]
    #endregion
    public string PlayerCharacterName;

    #region Tooltip
    [Tooltip("Prefab game object for the player")]
    #endregion
    public GameObject PlayerPrefab;

    #region Tooltip
    [Tooltip("Player runtime animator controller")]
    #endregion
    public RuntimeAnimatorController RuntimeAnimatorController;

    #region Header HEALTH
    [Space(10)]
    [Header("HEALTH")]
    #endregion
    #region Tooltip
    [Tooltip("Player starting health amount")]
    #endregion
    public int PlayerHealthAmount;

    #region Header OTHER
    [Space(10)]
    [Header("OTHER")]
    #endregion
    #region Tooltip
    [Tooltip("Player icon sprite to be used in the minimap")]
    #endregion
    public Sprite PlayerMiniMapIcon;

    #region Tooltip
    [Tooltip("Player hand sprite")] [CanBeNull]

    #endregion
    public Sprite PlayerHandSprite;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        EditorUtilities.ValidateCheckEmptyString(this, nameof(PlayerCharacterName), PlayerCharacterName);
        EditorUtilities.ValidateCheckNullValue(this, nameof(PlayerPrefab), PlayerPrefab);
        EditorUtilities.ValidateCheckPositiveValue(this, nameof(PlayerHealthAmount), PlayerHealthAmount, false);
        EditorUtilities.ValidateCheckNullValue(this, nameof(PlayerMiniMapIcon), PlayerMiniMapIcon);
        EditorUtilities.ValidateCheckNullValue(this, nameof(PlayerHandSprite), PlayerHandSprite);
        EditorUtilities.ValidateCheckNullValue(this, nameof(RuntimeAnimatorController), RuntimeAnimatorController);
    }
#endif
    #endregion

}