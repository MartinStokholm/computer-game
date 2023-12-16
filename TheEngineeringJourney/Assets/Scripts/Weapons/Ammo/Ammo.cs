using UnityEngine;

[DisallowMultipleComponent]
public class Ammo : MonoBehaviour, IFireAble
{
    #region Tooltip
    [Tooltip("Populate with child TrailRenderer component")]
    #endregion Tooltip
    [SerializeField] private TrailRenderer trailRenderer;

    private float AmmoRange = 0f; // the range of each ammo
    private float AmmoSpeed;
    private Vector3 FireDirectionVector;
    private float FireDirectionAngle;
    private SpriteRenderer SpriteRenderer;
    private AmmoDetailsSO AmmoDetails;
    private float AmmoChargeTimer;
    private bool IsAmmoMaterialSet = false;
    private bool OverrideAmmoMovement;
    private bool IsColliding = false;

    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Ammo charge effect
        if (AmmoChargeTimer > 0f)
        {
            AmmoChargeTimer -= Time.deltaTime;
            return;
        }
        
        if (!IsAmmoMaterialSet)
        {
            SetAmmoMaterial(AmmoDetails.AmmoMaterial);
            IsAmmoMaterialSet = true;
        }

        // Don't move ammo if movement has been overriden - e.g. this ammo is part of an ammo pattern
        if (OverrideAmmoMovement) return;
        
        // Calculate distance vector to move ammo
        var distanceVector = FireDirectionVector * (AmmoSpeed * Time.deltaTime);
        transform.position += distanceVector;

        // Disable after max range reached
        AmmoRange -= distanceVector.magnitude;

        if (!(AmmoRange < 0f)) return;
        // if (AmmoDetails.isPlayerAmmo)
        // {
        //     // no multiplier
        //     StaticEventHandler.CallMultiplierEvent(false);
        // }

        DisableAmmo();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If already colliding with something return
        Debug.Log($"OnTriggerEnter2D enemyHit  IsColliding! {IsColliding}");
        Debug.Log($"OnTriggerEnter2D enemyHit  to me! {collision.gameObject}");
        Debug.Log($"OnTriggerEnter2D enemyHit  IsColliding! {IsColliding}");
        
        if (IsColliding) return;

        // Deal Damage To Collision Object
        DealDamage(collision);

        // Show ammo hit effect
        AmmoHitEffect();

        DisableAmmo();
    }

    private void DealDamage(Collider2D collision)
    {
        var health = collision.GetComponent<Health>();
        Debug.Log($"DealDamage enemy healt! {health}");
        //var enemyHit = false;

        if (health is not null)
        {
            // Set isColliding to prevent ammo dealing damage multiple times
            IsColliding = true;
            Debug.Log($"DealDamage to: {collision.gameObject}, and take {AmmoDetails.AmmoDamage}");
            health.TakeDamage(AmmoDetails.AmmoDamage);

            // Enemy hit
            // if (health._enemy != null)
            // {
            //     Debug.Log($"enemyHit  to me! {collision.gameObject}");
            //     enemyHit = true;
            // }
        }

        // If player ammo then update multiplier
        // if (!AmmoDetails.isPlayerAmmo) return;
        // StaticEventHandler.CallMultiplierEvent(enemyHit);
    }


    /// <summary>
    /// Initialise the ammo being fired - using the ammodetails, the aimangle, weaponAngle, and
    /// weaponAimDirectionVector. If this ammo is part of a pattern the ammo movement can be
    /// overriden by setting overrideAmmoMovement to true
    /// </summary>
    public void InitialiseAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
    {
        #region Ammo

        this.AmmoDetails = ammoDetails;

        // Initialise isColliding
        IsColliding = false;

        // Set fire direction
        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

        // Set ammo sprite
        SpriteRenderer.sprite = ammoDetails.AmmoSprite;

        // set initial ammo material depending on whether there is an ammo charge period
        if (ammoDetails.AmmoChargeTime > 0f)
        {
            // Set ammo charge timer
            AmmoChargeTimer = ammoDetails.AmmoChargeTime;
            SetAmmoMaterial(ammoDetails.AmmoChargeMaterial);
            IsAmmoMaterialSet = false;
        }
        else
        {
            AmmoChargeTimer = 0f;
            SetAmmoMaterial(ammoDetails.AmmoMaterial);
            IsAmmoMaterialSet = true;
        }

        // Set ammo range
        AmmoRange = ammoDetails.AmmoRange;

        // Set ammo speed
        this.AmmoSpeed = ammoSpeed;

        // Override ammo movement
        this.OverrideAmmoMovement = overrideAmmoMovement;

        // Activate ammo gameobject
        gameObject.SetActive(true);

        #endregion Ammo


        #region Trail

        if (ammoDetails.IsAmmoTrail)
        {
            trailRenderer.gameObject.SetActive(true);
            trailRenderer.emitting = true;
            trailRenderer.material = ammoDetails.AmmoTrailMaterial;
            trailRenderer.startWidth = ammoDetails.AmmoTrailStartWidth;
            trailRenderer.endWidth = ammoDetails.AmmoTrailEndWidth;
            trailRenderer.time = ammoDetails.AmmoTrailTime;
        }
        else
        {
            trailRenderer.emitting = false;
            trailRenderer.gameObject.SetActive(false);
        }

        #endregion Trail

    }

    /// <summary>
    /// Set ammo fire direction and angle based on the input angle and direction adjusted by the
    /// random spread
    /// </summary>
    private void SetFireDirection(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        // calculate random spread angle between min and max
        var randomSpread = Random.Range(ammoDetails.AmmoSpreadMin, ammoDetails.AmmoSpreadMax);

        // Get a random spread toggle of 1 or -1
        var spreadToggle = Random.Range(0, 2) * 2 - 1;

        FireDirectionAngle = weaponAimDirectionVector.magnitude < Settings.UseAimAngleDistance 
            ? aimAngle 
            : weaponAimAngle;

        // Adjust ammo fire angle angle by random spread
        FireDirectionAngle += spreadToggle * randomSpread;

        // Set ammo rotation
        transform.eulerAngles = new Vector3(0f, 0f, FireDirectionAngle);

        // Set ammo fire direction
        FireDirectionVector = PlayerUtils.GetDirectionVectorFromAngle(FireDirectionAngle);
    }

    /// <summary>
    /// Disable the ammo - thus returning it to the object pool
    /// </summary>
    private void DisableAmmo()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Display the ammo hit effect
    /// </summary>
    private void AmmoHitEffect()
    {
        // Process if a hit effect has been specified
        if (AmmoDetails.AmmoHitEffect == null || AmmoDetails.AmmoHitEffect.AmmoHitEffectPrefab == null) return;
        Debug.Log($"AmmoDetails.AmmoHitEffect: {AmmoDetails.AmmoHitEffect}");
        Debug.Log($"AmmoDetails.AmmoHitEffectAmmoHitEffectPrefab: {AmmoDetails.AmmoHitEffect.AmmoHitEffectPrefab}");
        
        // Get ammo hit effect gameobject from the pool (with particle system component)
        var ammoHitEffect = (AmmoHitEffect)PoolManager.Instance.ReuseComponent(AmmoDetails.AmmoHitEffect.AmmoHitEffectPrefab, transform.position, Quaternion.identity);

        // Set Hit Effect
        ammoHitEffect.SetHitEffect(AmmoDetails.AmmoHitEffect);

        // Set gameobject active (the particle system is set to automatically disable the
        // gameobject once finished)
        ammoHitEffect.gameObject.SetActive(true);
    }


    public void SetAmmoMaterial(Material material)
    {
        SpriteRenderer.material = material;
    }


    public GameObject GetGameObject()
    {
        return gameObject;
    }

    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        EditorUtilities.ValidateCheckNullValue(this, nameof(trailRenderer), trailRenderer);
    }

#endif
    #endregion Validation

}