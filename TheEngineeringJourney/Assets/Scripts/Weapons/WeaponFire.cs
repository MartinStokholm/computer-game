using System.Collections;
using UnityEngine;

[RequireComponent(typeof(WeaponActive))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(WeaponReloadEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[DisallowMultipleComponent]
public class WeaponFire : MonoBehaviour
{
    private float _firePreChargeTimer;
    private float _fireRateCoolDownTimer;
    private WeaponActive _activeWeapon;
    private FireWeaponEvent _fireWeaponEvent;
    private WeaponReloadEvent _weaponReloadEvent;
    private WeaponFiredEvent _weaponFiredEvent;

    private void Awake()
    {
        // Load components.
        _activeWeapon = GetComponent<WeaponActive>();
        _fireWeaponEvent = GetComponent<FireWeaponEvent>();
        _weaponReloadEvent = GetComponent<WeaponReloadEvent>();
        _weaponFiredEvent = GetComponent<WeaponFiredEvent>();
    }

    private void OnEnable()
    {
        _fireWeaponEvent.OnFireWeapon += FireWeaponEvent_OnFireWeapon;
    }

    private void OnDisable()
    {
        _fireWeaponEvent.OnFireWeapon -= FireWeaponEvent_OnFireWeapon;
    }

    private void Update()
    {
        _fireRateCoolDownTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Handle fire weapon event.
    /// </summary>
    private void FireWeaponEvent_OnFireWeapon(FireWeaponEvent fireWeaponEvent, FireWeaponEventArgs fireWeaponEventArgs)
    {
        FireWeapon(fireWeaponEventArgs);
    }

    /// <summary>
    /// Fire weapon.
    /// </summary>
    private void FireWeapon(FireWeaponEventArgs fireWeaponEventArgs)
    {
        // Handle weapon precharge timer.
        WeaponPreCharge(fireWeaponEventArgs);

        // Weapon fire.
        if (!fireWeaponEventArgs.Fire) return;
        // Test if weapon is ready to fire.
        if (!IsWeaponReadyToFire()) return;
        
        FireAmmo(fireWeaponEventArgs.AimAngle, fireWeaponEventArgs.WeaponAimAngle, fireWeaponEventArgs.WeaponAimDirectionVector);
        ResetCoolDownTimer();
        ResetPrechargeTimer();
    }

    /// <summary>
    /// Handle weapon precharge.
    /// </summary>
    private void WeaponPreCharge(FireWeaponEventArgs fireWeaponEventArgs)
    {
        // Weapon precharge.
        if (fireWeaponEventArgs.FirePreviousFrame)
        {
            // Decrease precharge timer if fire button held previous frame.
            _firePreChargeTimer -= Time.deltaTime;
        }
        else
        {
            // else reset the precharge timer.
            ResetPrechargeTimer();
        }
    }

    /// <summary>
    /// Returns true if the weapon is ready to fire, else returns false.
    /// </summary>
    private bool IsWeaponReadyToFire()
    {
        // if there is no ammo and weapon doesn't have infinite ammo then return false.
        if (_activeWeapon.CurrentWeapon.WeaponRemainingAmmo <= 0 && !_activeWeapon.CurrentWeapon.WeaponDetails.HasInfiniteAmmo)
            return false;

        // if the weapon is reloading then return false.
        if (_activeWeapon.CurrentWeapon.IsWeaponReloading)
            return false;

        // If the weapon isn't precharged or is cooling down then return false.
        if (_firePreChargeTimer > 0f || _fireRateCoolDownTimer > 0f)
            return false;

        // if no ammo in the clip and the weapon doesn't have infinite clip capacity then return false.
        if (!_activeWeapon.CurrentWeapon.WeaponDetails.HasInfiniteClipCapacity && _activeWeapon.CurrentWeapon.WeaponClipRemainingAmmo <= 0)
        {
            // trigger a reload weapon event.
            _weaponReloadEvent.CallReloadWeaponEvent(_activeWeapon.CurrentWeapon, 0);

            return false;
        }

        // weapon is ready to fire - return true
        return true;
    }

    /// <summary>
    /// Set up ammo using an ammo gameobject and component from the object pool.
    /// </summary>
    private void FireAmmo(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        var currentAmmo = _activeWeapon.GetCurrentAmmo();
        if (currentAmmo is not null)
        {
            // Fire ammo routine.
            StartCoroutine(FireAmmoRoutine(currentAmmo, aimAngle, weaponAimAngle, weaponAimDirectionVector));
        }
    }

    /// <summary>
    /// Coroutine to spawn multiple ammo per shot if specified in the ammo details
    /// </summary>
    private IEnumerator FireAmmoRoutine(AmmoDetailsSO currentAmmo, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        var ammoCounter = 0;

        // Get random ammo per shot
        var ammoPerShot = Random.Range(currentAmmo.AmmoSpawnAmountMin, currentAmmo.AmmoSpawnAmountMax + 1);

        // Get random interval between ammo

        var ammoSpawnInterval = ammoPerShot > 1 
            ? Random.Range(currentAmmo.AmmoSpawnIntervalMin, currentAmmo.AmmoSpawnIntervalMax) 
            : 0f;

        // Loop for number of ammo per shot
        while (ammoCounter < ammoPerShot)
        {
            ammoCounter++;

            // Get ammo prefab from array
            var ammoPrefab = currentAmmo.AmmoPrefabArray[Random.Range(0, currentAmmo.AmmoPrefabArray.Length)];

            // Get random speed value
            var ammoSpeed = Random.Range(currentAmmo.AmmoSpeedMin, currentAmmo.AmmoSpeedMax);

            // Get Gameobject with IFireable component
            var ammo = (IFireAble)PoolManager.Instance.ReuseComponent(ammoPrefab, _activeWeapon.GetShootPosition(), Quaternion.identity);
            Debug.Log($"ammo {ammo.GetGameObject()} and {ammo}");
            
            // Initialise Ammo
            ammo.InitialiseAmmo(currentAmmo, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector);

            // Wait for ammo per shot timegap
            yield return new WaitForSeconds(ammoSpawnInterval);
        }

        // Reduce ammo clip count if not infinite clip capacity
        if (!_activeWeapon.CurrentWeapon.WeaponDetails.HasInfiniteClipCapacity)
        {
            _activeWeapon.CurrentWeapon.WeaponClipRemainingAmmo--;
            _activeWeapon.CurrentWeapon.WeaponRemainingAmmo--;
        }

        // Call weapon fired event
        _weaponFiredEvent.CallWeaponFiredEvent(_activeWeapon.GetCurrentWeapon());

        // Display weapon shoot effect
        WeaponShootEffect(aimAngle);

        // Weapon fired sound effect
        WeaponSoundEffect();
    }
    

    /// <summary>
    /// Display the weapon shoot effect
    /// </summary>
    private void WeaponShootEffect(float aimAngle)
    {
        // Process if there is a shoot effect & prefab
        if (_activeWeapon.CurrentWeapon.WeaponDetails.WeaponShootEffect == null ||
            _activeWeapon.CurrentWeapon.WeaponDetails.WeaponShootEffect.WeaponShootEffectPrefab is null) return;
        
        // Get weapon shoot effect gameobject from the pool with particle system component
        var weaponShootEffect = (WeaponShootEffect)PoolManager.Instance.ReuseComponent(_activeWeapon.GetCurrentWeapon().WeaponDetails.WeaponShootEffect.WeaponShootEffectPrefab, _activeWeapon.GetShootEffectPosition(), Quaternion.identity);
        //Debug.Log($"Shoot {weaponShootEffect}");
        // Set shoot effect
        weaponShootEffect.SetShootEffect(_activeWeapon.CurrentWeapon.WeaponDetails.WeaponShootEffect, aimAngle);
        
        // Set gameobject active (the particle system is set to automatically disable the
        // gameobject once finished)
        weaponShootEffect.gameObject.SetActive(true);
    }

    /// <summary>
    /// Play weapon shooting sound effect
    /// </summary>
    private void WeaponSoundEffect()
    {
        if (_activeWeapon.CurrentWeapon.WeaponDetails.WeaponFiringSoundEffect is not null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(_activeWeapon.GetCurrentWeapon().WeaponDetails.WeaponFiringSoundEffect);
        }
        else
        {
            Debug.Log("WeaponSoundEffect missing");
        }
    }
    
    /// <summary>
    /// Reset cooldown timer
    /// </summary>
    private void ResetCoolDownTimer() => 
        _fireRateCoolDownTimer = _activeWeapon.CurrentWeapon.WeaponDetails.WeaponFireRate;

    /// <summary>
    /// Reset precharge timers
    /// </summary>
    private void ResetPrechargeTimer() =>
        _firePreChargeTimer = _activeWeapon.CurrentWeapon.WeaponDetails.WeaponPrechargeTime;

}