using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(DealContactDamage))]
[RequireComponent(typeof(ReceiveContactDamage))]
[RequireComponent(typeof(DestroyedEvent))]
[RequireComponent(typeof(Destroyed))]
[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[RequireComponent(typeof(MovementByVelocity))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(WeaponAimEvent))]
[RequireComponent(typeof(WeaponAim))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(WeaponFire))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(WeaponActive))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(WeaponReloadEvent))]
[RequireComponent(typeof(WeaponReload))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(AnimatePlayer))]
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
#endregion REQUIRE COMPONENTS

public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerDetailsSO PlayerDetails;
    [HideInInspector] public HealthEvent HealthEvent;
    [HideInInspector] public Health Health;
    [HideInInspector] public MovementByVelocityEvent MovementByVelocityEvent;
    [HideInInspector] public MovementToPositionEvent MovementToPositionEvent;
    [HideInInspector] public IdleEvent IdleEvent;
    [HideInInspector] public SpriteRenderer SpriteRenderer;
    [HideInInspector] public Animator Animator;
    [HideInInspector] public DestroyedEvent DestroyedEvent;
    
    [HideInInspector] public PlayerControl playerControl;
    [FormerlySerializedAs("AimWeaponEvent")] [HideInInspector] public WeaponAimEvent weaponAimEvent;
    [FormerlySerializedAs("fireWeaponEvent")] [HideInInspector] public FireWeaponEvent WeaponFireEvent;
    [HideInInspector] public SetActiveWeaponEvent setActiveWeaponEvent;
    [HideInInspector] public WeaponActive weaponActive;
    [HideInInspector] public WeaponFiredEvent weaponFiredEvent;
    [HideInInspector] public WeaponReloadEvent reloadWeaponEvent;
    [HideInInspector] public WeaponReloadedEvent weaponReloadedEvent;
    
    public List<Weapon> Weapons = new List<Weapon>();

    private void Awake()
    {
        HealthEvent = GetComponent<HealthEvent>();
        Health = GetComponent<Health>();
        MovementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        MovementToPositionEvent = GetComponent<MovementToPositionEvent>();
        weaponAimEvent = GetComponent<WeaponAimEvent>();
        IdleEvent = GetComponent<IdleEvent>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();
        DestroyedEvent = GetComponent<DestroyedEvent>();
        playerControl = GetComponent<PlayerControl>();
        weaponAimEvent = GetComponent<WeaponAimEvent>();
        WeaponFireEvent = GetComponent<FireWeaponEvent>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
        weaponActive = GetComponent<WeaponActive>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        reloadWeaponEvent = GetComponent<WeaponReloadEvent>();
        weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
    }
    

    /// <summary>
    /// Initialize the player
    /// </summary>
    public void Initialize(PlayerDetailsSO playerDetails)
    {
        PlayerDetails = playerDetails;
        
        CreatePlayerStartingWeapons();
        // Set player starting health
        SetPlayerHealth(playerDetails);
    }
    
    private void OnEnable()
    {
        HealthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }


    private void OnDisable()
    {
        HealthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }
    
    /// <summary>
    /// Handle health changed event
    /// </summary>
    private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        Debug.Log("Deal dmg to player" + healthEventArgs.HealthAmount);
        if (!(healthEventArgs.HealthAmount <= 0f)) return;

        SetPlayerHealth(PlayerDetails);
        
        DestroyedEvent.CallDestroyedEvent(true, 0);
    }
    
    /// <summary>
    /// Set player health from playerDetails SO
    /// </summary>
    private void SetPlayerHealth(PlayerDetailsSO playerDetails)
    {
        Debug.Log("SetStartingHealth" + playerDetails.PlayerHealthAmount);
        Health.SetStartingHealth(playerDetails.PlayerHealthAmount);
    }

    /// <summary>
    /// Gets the player position
    /// </summary>
    public Vector3 GetPlayerPosition() => transform.position;
    
    /// <summary>
    /// Set the player starting weapon
    /// </summary>
    private void CreatePlayerStartingWeapons()
    {
        //Clear list
        Weapons.Clear();
        
        // Populate weapon list from starting weapons
        foreach (var weaponDetails in PlayerDetails.StartingWeaponList)
        {
            // Add weapon to player
            AddWeaponToPlayer(weaponDetails);
        }
    }
    
    
    /// <summary>
    /// Add a weapon to the player weapon dictionary
    /// </summary>
    public Weapon AddWeaponToPlayer(WeaponDetailsSO weaponDetails)
    {
        Debug.Log($"AddWeaponToPlayer: {weaponDetails.WeaponShootPosition}");
        var weapon = new Weapon() { 
            WeaponDetails = weaponDetails, 
            WeaponReloadTimer = 0f, 
            WeaponClipRemainingAmmo = weaponDetails.WeaponClipAmmoCapacity, 
            WeaponRemainingAmmo = weaponDetails.WeaponAmmoCapacity, 
            IsWeaponReloading = false };

        // Add the weapon to the list
        Weapons.Add(weapon);

        // Set weapon position in list
        weapon.WeaponListPosition = Weapons.Count;

        // Set the added weapon as active
        setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);

        return weapon;
    }


    /// <summary>
    /// Returns true if the weapon is held by the player - otherwise returns false
    /// </summary>
    public bool IsWeaponHeldByPlayer(WeaponDetailsSO weaponDetails)
    {

        foreach (var weapon in Weapons)
        {
            if (weapon.WeaponDetails == weaponDetails) return true;
        }

        return false;
    }
    
}