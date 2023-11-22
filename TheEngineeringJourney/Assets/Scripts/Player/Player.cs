using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(Health))]
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
    [HideInInspector] public Health Health;
    [HideInInspector] public MovementByVelocityEvent MovementByVelocityEvent;
    [HideInInspector] public MovementToPositionEvent MovementToPositionEvent;
    [HideInInspector] public IdleEvent IdleEvent;
    [FormerlySerializedAs("aimWeaponEvent")] [HideInInspector] public AimWeaponEvent AimWeaponEvent;
    [HideInInspector] public SpriteRenderer SpriteRenderer;
    [HideInInspector] public Animator Animator;

    private void Awake()
    {
        Health = GetComponent<Health>();
        MovementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        MovementToPositionEvent = GetComponent<MovementToPositionEvent>();
        IdleEvent = GetComponent<IdleEvent>();
        AimWeaponEvent = GetComponent<AimWeaponEvent>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Initialize the player
    /// </summary>
    public void Initialize(PlayerDetailsSO playerDetails)
    {
        PlayerDetails = playerDetails;

        // Set player starting health
        SetPlayerHealth(playerDetails);
    }
    
    /// <summary>
    /// Set player health from playerDetails SO
    /// </summary>
    private void SetPlayerHealth(PlayerDetailsSO playerDetails)
    {
        Health.SetStartingHealth(playerDetails.PlayerHealthAmount);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("IsTrigger");
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("IsTrigger collision");
    }
}