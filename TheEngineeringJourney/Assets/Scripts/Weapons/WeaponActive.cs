using System.Collections.Generic;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

[RequireComponent(typeof(SetActiveWeaponEvent))]
[DisallowMultipleComponent]
public class WeaponActive : MonoBehaviour
{
    #region Tooltip
    [Tooltip("Populate with the SpriteRenderer on the child Weapon gameobject")]
    #endregion
    [SerializeField] private SpriteRenderer WeaponSpriteRenderer;
    #region Tooltip
    [Tooltip("Populate with the PolygonCollider2D on the child Weapon gameobject")]
    #endregion
    [SerializeField] private PolygonCollider2D WeaponPolygonCollider2D;
    #region Tooltip
    [Tooltip("Populate with the Transform on the WeaponShootPosition gameobject")]
    #endregion
    [SerializeField] private Transform WeaponShootPositionTransform;
    #region Tooltip
    [Tooltip("Populate with the Transform on the WeaponEffectPosition gameobject")]
    #endregion
    [SerializeField] private Transform WeaponEffectPositionTransform;

    private SetActiveWeaponEvent SetWeaponEvent;
    public Weapon CurrentWeapon { get; private set; }


    private void Awake()
    {
        // Load components
        SetWeaponEvent = GetComponent<SetActiveWeaponEvent>();
    }

    private void OnEnable()
    {
        SetWeaponEvent.OnSetActiveWeapon += SetWeaponEvent_OnSetActiveWeapon;
    }

    private void OnDisable()
    {
        SetWeaponEvent.OnSetActiveWeapon -= SetWeaponEvent_OnSetActiveWeapon;
    }

    private void SetWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        SetWeapon(setActiveWeaponEventArgs.Weapon);
    }

    private void SetWeapon(Weapon weapon)
    {
        CurrentWeapon = weapon;

        // Set current weapon sprite
        WeaponSpriteRenderer.sprite = CurrentWeapon.WeaponDetails.WeaponSprite;

        // If the weapon has a polygon collider and a sprite then set it to the weapon sprite physics shape
        if (WeaponPolygonCollider2D is not null && WeaponSpriteRenderer.sprite is not null)
        {
            // Get sprite physics shape - this returns the sprite physics shape points as a list of Vector2s
            var spritePhysicsShapePointsList = new List<UnityEngine.Vector2>();
            WeaponSpriteRenderer.sprite.GetPhysicsShape(0, spritePhysicsShapePointsList);

            // Set polygon collider on weapon to pick up physics shap for sprite - set collider points to sprite physics shape points
            WeaponPolygonCollider2D.points = spritePhysicsShapePointsList.ToArray();

        }

        // Set weapon shoot position
        WeaponShootPositionTransform.localPosition = CurrentWeapon.WeaponDetails.WeaponShootPosition;
    }

    public AmmoDetailsSO GetCurrentAmmo()
    {
        return CurrentWeapon.WeaponDetails.WeaponCurrentAmmo;
    }

    public Weapon GetCurrentWeapon()
    {
        return CurrentWeapon;
    }

    public UnityEngine.Vector3 GetShootPosition()
    {
        return WeaponShootPositionTransform.position;
    }

    public UnityEngine.Vector3 GetShootEffectPosition()
    {
        return WeaponEffectPositionTransform.position;
    }

    public void RemoveCurrentWeapon()
    {
        CurrentWeapon = null;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        EditorUtilities.ValidateCheckNullValue(this, nameof(WeaponSpriteRenderer), WeaponSpriteRenderer);
        EditorUtilities.ValidateCheckNullValue(this, nameof(WeaponPolygonCollider2D), WeaponPolygonCollider2D);
        EditorUtilities.ValidateCheckNullValue(this, nameof(WeaponShootPositionTransform), WeaponShootPositionTransform);
        EditorUtilities.ValidateCheckNullValue(this, nameof(WeaponEffectPositionTransform), WeaponEffectPositionTransform);
    }
#endif
    #endregion

}