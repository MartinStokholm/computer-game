using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AnimateEnemy))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(DealContactDamage))]
[RequireComponent(typeof(Destroyed))]
[RequireComponent(typeof(DestroyedEvent))]
[RequireComponent(typeof(EnemyWeaponAI))]
[RequireComponent(typeof(EnemyMovementAI))]
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(WeaponActive))]
[RequireComponent(typeof(WeaponAimEvent))]
[RequireComponent(typeof(WeaponAim))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(WeaponReloadEvent))]
[RequireComponent(typeof(WeaponReload))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(WeaponFire))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(WeaponReload))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SortingGroup))]

[DisallowMultipleComponent]
#endregion REQUIRE COMPONENTS
public class Enemy : MonoBehaviour
{
    [HideInInspector] public EnemyDetailsSO EnemyDetails;
    [HideInInspector] public SpriteRenderer[] spriteRendererArray;
    [HideInInspector] public Health Health;
    [HideInInspector] public MovementByVelocityEvent MovementByVelocityEvent;
    [HideInInspector] public EnemyWeaponAI EnemyWeaponAI;
    [HideInInspector] public MovementToPositionEvent MovementToPositionEvent;
    [HideInInspector] public IdleEvent IdleEvent;
    [HideInInspector] public Animator Animator;
    [HideInInspector] public WeaponAimEvent WeaponAimEvent;
    [HideInInspector] public FireWeaponEvent WeaponFireEvent;

    private HealthEvent HealthEvent;
    private MaterializeEffect MaterializeEffect;
    private CircleCollider2D circleCollider2D;
    private PolygonCollider2D polygonCollider2D;
    private EnemyMovementAI _enemyMovementAI;
    private SetActiveWeaponEvent _setActiveWeaponEvent;
    private WeaponFire _weaponFire;

    private void Awake()
    {
        HealthEvent = GetComponent<HealthEvent>();
        Health = GetComponent<Health>();
        
        IdleEvent = GetComponent<IdleEvent>();
        _enemyMovementAI = GetComponent<EnemyMovementAI>();
        MovementToPositionEvent = GetComponent<MovementToPositionEvent>();

        circleCollider2D = GetComponent<CircleCollider2D>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        spriteRendererArray = GetComponentsInChildren<SpriteRenderer>();
        Animator = GetComponent<Animator>();
        
        EnemyWeaponAI = GetComponent<EnemyWeaponAI>();
        WeaponAimEvent = GetComponent<WeaponAimEvent>();
        WeaponFireEvent = GetComponent<FireWeaponEvent>();
        _weaponFire = GetComponent<WeaponFire>();
        _setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
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
        Debug.Log($"Health lost for enemy {healthEventArgs.HealthAmount } of {Health._currentHealth}" );
        
        if (healthEventArgs.HealthAmount > 0) return;
        
        Debug.Log("DESTROY ENEMY");
        GameManager.Instance.MiscEvents.EnemyKilled();
        
        GetComponent<DestroyedEvent>().CallDestroyedEvent(false, Health.StartingHealth);
    }

    /// <summary>
    /// Initialise the enemy
    /// </summary>
    public void EnemyInitialization(EnemyDetailsSO enemyDetails, int enemySpawnNumber, MapLevelSO mapLevel)
    {
        EnemyDetails = enemyDetails;

        SetEnemyMovementUpdateFrame(enemySpawnNumber);
        //
        SetEnemyStartingHealth(mapLevel);
        //
        SetEnemyStartingWeapon();
        //
        SetEnemyAnimationSpeed();

        // Materialise enemy
        //EnemyEnable(true);
        StartCoroutine(MaterializeEnemy());
    }
    
    /// <summary>
    /// Set enemy movement update frame
    /// </summary>
    private void SetEnemyMovementUpdateFrame(int enemySpawnNumber)
    {
        // Set frame number that enemy should process it's updates
        _enemyMovementAI.SetUpdateFrameNumber(enemySpawnNumber % Settings.TargetFrameRateToSpreadPathfindingOver);
    }
    
    /// <summary>
    /// Set enemy animator speed to match movement speed
    /// </summary>
    private void SetEnemyAnimationSpeed()
    {
        // Set animator speed to match movement speed
        Animator.speed = _enemyMovementAI.moveSpeed / Settings.BaseSpeedForEnemyAnimations;
    }
    
    private IEnumerator MaterializeEnemy()
    {
        // Disable collider, Movement AI and Weapon AI
        //EnemyEnable(false);
        // Debug.Log("MaterializeEnemy" );
        // Debug.Log("MaterializeEnemy EnemyDetails.enemyMaterializeShader: " + EnemyDetails.enemyMaterializeShader);
        // Debug.Log("MaterializeEnemy EnemyDetails.enemyMaterializeColor: " + EnemyDetails.enemyMaterializeColor);
        // Debug.Log("MaterializeEnemy EnemyDetails.enemyMaterializeTime: " + EnemyDetails.enemyMaterializeTime);
        // Debug.Log("MaterializeEnemy spriteRendererArray: " + spriteRendererArray.Length + spriteRendererArray.FirstOrDefault());
        // Debug.Log("MaterializeEnemy EnemyDetails.enemyStandardMaterial: " + EnemyDetails.enemyStandardMaterial);
        
        //Test(EnemyDetails.enemyMaterializeShader, EnemyDetails.enemyMaterializeColor, EnemyDetails.enemyMaterializeTime, spriteRendererArray, EnemyDetails.enemyStandardMaterial);
        yield return StartCoroutine(
            MaterializeEffect.MaterializeRoutine(
                EnemyDetails.enemyMaterializeShader, 
                EnemyDetails.enemyMaterializeColor, 
                EnemyDetails.enemyMaterializeTime, 
                spriteRendererArray, 
                EnemyDetails.enemyStandardMaterial));
    
        // Enable collider, Movement AI and Weapon AI
        EnemyEnable(true);
    
    }

    private void Test(Shader materializeShader, Color materializeColor, float materializeTime, SpriteRenderer[] spriteRendererArray, Material normalMaterial)
    {
        Debug.LogError($"MaterializeRoutine materializeShader: ");
        var materializeMaterial = new Material(materializeShader);

        materializeMaterial.SetColor("_EmissionColor", materializeColor);
        Debug.LogError("MaterializeRoutine");
        Debug.LogError($"spriteRendererArray {spriteRendererArray.Length}");
        // Set materialize material in sprite renderers
        foreach (var spriteRenderer in spriteRendererArray)
        {
            spriteRenderer.material = materializeMaterial;
            Debug.LogError($"MaterializeRoutine loop: {materializeMaterial}");
        }
        
        var dissolveAmount = 0f;
        
        //materialize enemy
        while (dissolveAmount < 1f)
        {
            dissolveAmount += Time.deltaTime / materializeTime;
            Debug.LogError($"dissolveAmount: {dissolveAmount}");
            materializeMaterial.SetFloat("_DissolveAmount", dissolveAmount);
            
        
        }


        //Set standard material in sprite renderers
        foreach (var spriteRenderer in spriteRendererArray)
        {
            spriteRenderer.material = normalMaterial;
            Debug.LogError($"spriteRenderer.material: {spriteRenderer.material}");
        }
    }
    
    /// <summary>
    /// Set enemy starting weapon as per the weapon details SO
    /// </summary>
    private void SetEnemyStartingWeapon()
    {
        // Process if enemy has a weapon
        if (EnemyDetails.EnemyWeapon is null) return;
        
        var weapon = new Weapon()
        {
            WeaponDetails = EnemyDetails.EnemyWeapon, 
            WeaponReloadTimer = 0f, 
            WeaponClipRemainingAmmo = EnemyDetails.EnemyWeapon.WeaponClipAmmoCapacity, 
            WeaponRemainingAmmo = EnemyDetails.EnemyWeapon.WeaponAmmoCapacity, 
            IsWeaponReloading = false
        };
        
        _setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);
    }
    
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

        // // Enable/Disable movement AI
        _enemyMovementAI.enabled = isEnabled;

        // // Enable / Disable Fire Weapon
        _weaponFire.enabled = isEnabled;
    }
}
