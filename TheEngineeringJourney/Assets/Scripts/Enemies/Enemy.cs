using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(EnemyWeaponAI))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AnimateEnemy))]
[RequireComponent(typeof(EnemyMovementAI))]
[DisallowMultipleComponent]
#endregion REQUIRE COMPONENTS

public class Enemy : MonoBehaviour
{
    public EnemyDetailsSO EnemyDetails;
    [HideInInspector] public SpriteRenderer[] spriteRendererArray;
    public Health Health;
    [HideInInspector] public AimWeaponEvent AimWeaponEvent;
    [HideInInspector] public MovementByVelocityEvent MovementByVelocityEvent;
    [HideInInspector] public EnemyWeaponAI EnemyWeaponAI;
    [HideInInspector] public MovementToPositionEvent MovementToPositionEvent;
    [HideInInspector] public IdleEvent IdleEvent;
    [HideInInspector] public Animator Animator;

    private HealthEvent HealthEvent;
    private MaterializeEffect MaterializeEffect;
    private CircleCollider2D circleCollider2D;
    private PolygonCollider2D polygonCollider2D;
    private EnemyMovementAI _enemyMovementAI;

    private void Awake()
    {
        // Load components
        HealthEvent = GetComponent<HealthEvent>();
        Health = GetComponent<Health>();
        MovementToPositionEvent = GetComponent<MovementToPositionEvent>();
        AimWeaponEvent = GetComponent<AimWeaponEvent>();
        EnemyWeaponAI = GetComponent<EnemyWeaponAI>();
        IdleEvent = GetComponent<IdleEvent>();
        _enemyMovementAI = GetComponent<EnemyMovementAI>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        spriteRendererArray = GetComponentsInChildren<SpriteRenderer>();
        Animator = GetComponent<Animator>();
    }
    
    private void OnEnable()
    {
        HealthEvent.OnHealthChanged += HealthEvent_OnHealthLost;
    }

    private void OnDisable()
    {
        HealthEvent.OnHealthChanged -= HealthEvent_OnHealthLost;
    }
    
    
    /// <summary>
    /// Handle health lost event
    /// </summary>
    private void HealthEvent_OnHealthLost(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        if (healthEventArgs.HealthAmount > 0) return;
        
        var destroyedEvent = GetComponent<DestroyedEvent>();
        destroyedEvent.CallDestroyedEvent(false, Health.GetStartingHealth());
    }

    /// <summary>
    /// Initialise the enemy
    /// </summary>
    public void EnemyInitialization(EnemyDetailsSO enemyDetails, int enemySpawnNumber, MapLevelSO mapLevel)
    {
        EnemyDetails = enemyDetails;

        // SetEnemyMovementUpdateFrame(enemySpawnNumber);
        //
        SetEnemyStartingHealth(mapLevel);
        //
        // SetEnemyStartingWeapon();
        //
        //SetEnemyAnimationSpeed();

        // Materialise enemy
        //StartCoroutine(MaterializeEnemy());
    }
    
    // private IEnumerator MaterializeEnemy()
    // {
    //     // Disable collider, Movement AI and Weapon AI
    //     EnemyEnable(false);
    //
    //     //yield return StartCoroutine(MaterializeEffect.MaterializeRoutine(EnemyDetails.enemyMaterializeShader, EnemyDetails.enemyMaterializeColor, EnemyDetails.enemyMaterializeTime, spriteRendererArray, EnemyDetails.enemyStandardMaterial));
    //
    //     // Enable collider, Movement AI and Weapon AI
    //     EnemyEnable(true);
    //
    // }
    
    /// <summary>
    /// Set the starting health for the enemy
    /// </summary>
    private void SetEnemyStartingHealth(MapLevelSO mapLevel)
    {
        var matchingHealthDetails = EnemyDetails.EnemyHealthDetailsArray
            .FirstOrDefault(e => e.MapLevel == mapLevel);

        Health.SetStartingHealth(matchingHealthDetails?.EnemyHealthAmount ?? Settings.DefaultEnemyHealth);
    }
    
    private void EnemyEnable(bool isEnabled)
    {
        // Enable/Disable colliders
        circleCollider2D.enabled = isEnabled;
        polygonCollider2D.enabled = isEnabled;
        //
        // // Enable/Disable movement AI
        _enemyMovementAI.enabled = isEnabled;
        //
        // // Enable / Disable Fire Weapon
        // fireWeapon.enabled = isEnabled;
    }
}
