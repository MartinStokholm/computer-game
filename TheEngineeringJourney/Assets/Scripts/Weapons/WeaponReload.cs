using System.Collections;
using UnityEngine;

[RequireComponent(typeof(WeaponReloadEvent))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(SetActiveWeaponEvent))]

[DisallowMultipleComponent]
public class WeaponReload : MonoBehaviour
{
    private WeaponReloadEvent _weaponReloadEvent;
    private WeaponReloadedEvent _weaponReloadedEvent;
    private SetActiveWeaponEvent _setActiveWeaponEvent;
    private Coroutine _reloadWeaponCoroutine;

    private void Awake()
    {
        _weaponReloadEvent = GetComponent<WeaponReloadEvent>();
        _weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
        _setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
    }

    private void OnEnable()
    {
        _weaponReloadEvent.OnReloadWeapon += WeaponReloadEventOnWeaponReload;
        _setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
    }

    private void OnDisable()
    {
        _weaponReloadEvent.OnReloadWeapon -= WeaponReloadEventOnWeaponReload;
        _setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
    }

    /// <summary>
    /// Handle reload weapon event
    /// </summary>
    private void WeaponReloadEventOnWeaponReload(WeaponReloadEvent weaponReloadEvent, ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        StartReloadWeapon(reloadWeaponEventArgs);
    }

    /// <summary>
    /// Start reloading the weapon
    /// </summary>
    private void StartReloadWeapon(ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        if (_reloadWeaponCoroutine != null)
        {
            StopCoroutine(_reloadWeaponCoroutine);
        }

        _reloadWeaponCoroutine = StartCoroutine(ReloadWeaponRoutine(reloadWeaponEventArgs.Weapon, reloadWeaponEventArgs.TopUpAmmoPercent));
    }

    /// <summary>
    /// Reload weapon coroutine
    /// </summary>
    private IEnumerator ReloadWeaponRoutine(Weapon weapon, int topUpAmmoPercent)
    {
        // Play reload sound if there is one
        if (weapon.WeaponDetails.WeaponReloadingSoundEffect is not null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(weapon.WeaponDetails.WeaponReloadingSoundEffect);

        }

        // Set weapon as reloading
        weapon.IsWeaponReloading = true;

        // Update reload progress timer
        while (weapon.WeaponReloadTimer < weapon.WeaponDetails.WeaponReloadTime)
        {
            weapon.WeaponReloadTimer += Time.deltaTime;
            yield return null;
        }

        // If total ammo is to be increased then update
        if (topUpAmmoPercent != 0)
        {
            var ammoIncrease = Mathf.RoundToInt((weapon.WeaponDetails.WeaponAmmoCapacity * topUpAmmoPercent) / 100f);

            var totalAmmo = weapon.WeaponRemainingAmmo + ammoIncrease;

            weapon.WeaponRemainingAmmo = totalAmmo > weapon.WeaponDetails.WeaponAmmoCapacity 
                ? weapon.WeaponDetails.WeaponAmmoCapacity 
                : totalAmmo;
        }

        // If weapon has infinite ammo then just refill the clip
        if (weapon.WeaponDetails.HasInfiniteAmmo)
        {
            weapon.WeaponClipRemainingAmmo = weapon.WeaponDetails.WeaponClipAmmoCapacity;
        }
        // else if not infinite ammo then if remaining ammo is greater than the amount required to
        // refill the clip, then fully refill the clip
        else if (weapon.WeaponRemainingAmmo >= weapon.WeaponDetails.WeaponClipAmmoCapacity)
        {
            weapon.WeaponClipRemainingAmmo = weapon.WeaponDetails.WeaponClipAmmoCapacity;
        }
        // else set the clip to the remaining ammo
        else
        {
            weapon.WeaponClipRemainingAmmo = weapon.WeaponRemainingAmmo;
        }

        // Reset weapon reload timer
        weapon.WeaponReloadTimer = 0f;

        // Set weapon as not reloading
        weapon.IsWeaponReloading = false;

        // Call weapon reloaded event
        _weaponReloadedEvent.CallWeaponReloadedEvent(weapon);

    }

    /// <summary>
    /// Set active weapon event handler
    /// </summary>
    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        if (!setActiveWeaponEventArgs.Weapon.IsWeaponReloading) return;
        if (_reloadWeaponCoroutine is not null)
        {
            StopCoroutine(_reloadWeaponCoroutine);
        }

        _reloadWeaponCoroutine = StartCoroutine(ReloadWeaponRoutine(setActiveWeaponEventArgs.Weapon, 0));
    }
}
