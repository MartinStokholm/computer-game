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
        HelperUtilities.ValidateCheckEmptyString(this, nameof(PlayerCharacterName), PlayerCharacterName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(PlayerPrefab), PlayerPrefab);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(PlayerHealthAmount), PlayerHealthAmount, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(PlayerMiniMapIcon), PlayerMiniMapIcon);
        HelperUtilities.ValidateCheckNullValue(this, nameof(PlayerHandSprite), PlayerHandSprite);
        HelperUtilities.ValidateCheckNullValue(this, nameof(RuntimeAnimatorController), RuntimeAnimatorController);
    }
#endif
    #endregion

}