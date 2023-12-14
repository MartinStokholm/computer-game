using UnityEngine;


[RequireComponent(typeof(WeaponAimEvent))]
[DisallowMultipleComponent]
public class WeaponAim : MonoBehaviour
{

    #region Tooltip
    [Tooltip("Populate with the Transform from the child WeaponRotationPoint gameobject")]
    #endregion
    [SerializeField] private Transform weaponRotationPointTransform;

    private WeaponAimEvent _weaponAimEvent;

    private void Awake()
    {
        // Load components
        _weaponAimEvent = GetComponent<WeaponAimEvent>();
    }

    private void OnEnable()
    {
        // Subscribe to aim weapon event
        _weaponAimEvent.OnWeaponAim += WeaponAimEventOnWeaponAim;
    }

    private void OnDisable()
    {
        // Unsubscribe from aim weapon event
        _weaponAimEvent.OnWeaponAim -= WeaponAimEventOnWeaponAim;
    }

    /// <summary>
    /// Aim weapon event handler
    /// </summary>
    private void WeaponAimEventOnWeaponAim(WeaponAimEvent weaponAimEvent, AimWeaponEventArgs aimWeaponEventArgs)
    {
        Aim(aimWeaponEventArgs.AimDirection, aimWeaponEventArgs.AimAngle);
    }

    /// <summary>
    /// Aim the weapon based on player direction this flips the weapon transform based 
    /// </summary>
    private void Aim(AimDirection aimDirection, float aimAngle)
    {
        // Set angle of the weapon transform
        weaponRotationPointTransform.eulerAngles = new Vector3(0f, 0f, aimAngle);

        // 
        switch (aimDirection)
        {
            case AimDirection.Left:
                weaponRotationPointTransform.localScale = new Vector3(1f, -1f, 0f);
                break;

            case AimDirection.Up:
            case AimDirection.Right:
            case AimDirection.Down:
                weaponRotationPointTransform.localScale = new Vector3(1f, 1f, 0f);
                break;
        }

    }


    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        EditorUtilities.ValidateCheckNullValue(this, nameof(weaponRotationPointTransform), weaponRotationPointTransform);
    }
#endif
    #endregion

}
