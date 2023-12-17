using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyWeaponAI : MonoBehaviour
{
    #region Tooltip
    [Tooltip("Select the layers that the enemy bullets will hit")]
    #endregion Tooltip
    [SerializeField] private LayerMask layerMask;
    #region Tooltip
    [FormerlySerializedAs("weaponShootPosition")]
    [Tooltip("Populate this with the WeaponShootPosition child gameobject transform")]
    #endregion Tooltip
    [SerializeField] private Transform WeaponShootPosition;
    private Enemy enemy;
    private EnemyDetailsSO enemyDetails;
    private float firingIntervalTimer;
    private float firingDurationTimer;

    private void Awake()
    {
        // Load Components
        enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        enemyDetails = enemy.EnemyDetails;

        firingIntervalTimer = WeaponShootInterval();
        firingDurationTimer = WeaponShootDuration();
    }


    private void Update()
    {
        // Update timers
        firingIntervalTimer -= Time.deltaTime;

        // Interval Timer
        if (firingIntervalTimer < 0f)
        {
            if (firingDurationTimer >= 0)
            {
                firingDurationTimer -= Time.deltaTime;

                FireWeapon();
            }
            else
            {
                // Reset timers
                firingIntervalTimer = WeaponShootInterval();
                firingDurationTimer = WeaponShootDuration();
            }
        }
    }

    /// <summary>
    /// Calculate a random weapon shoot duration between the min and max values
    /// </summary>
    private float WeaponShootDuration()
    {
        // Calculate a random weapon shoot duration
        return Random.Range(enemyDetails.FiringDurationMin, enemyDetails.FiringDurationMax);
    }

    /// <summary>
    /// Calculate a random weapon shoot interval between the min and max values
    /// </summary>
    private float WeaponShootInterval()
    {
        // Calculate a random weapon shoot interval
        return Random.Range(enemyDetails.FiringIntervalMin, enemyDetails.FiringIntervalMax);
    }

    /// <summary>
    /// Fire the weapon
    /// </summary>
    private void FireWeapon()
    {
        // Player distance
        Vector3 playerDirectionVector = GameManager.Instance.Player.GetPlayerPosition() - transform.position;

        // Calculate direction vector of player from weapon shoot position
        Vector3 weaponDirection = (GameManager.Instance.Player.GetPlayerPosition() - WeaponShootPosition.position);

        // Get weapon to player angle
        float weaponAngleDegrees = GameUtilities.GetAngleFromVector(weaponDirection);

        // Get enemy to player angle
        float enemyAngleDegrees = GameUtilities.GetAngleFromVector(playerDirectionVector);

        // Set enemy aim direction
        AimDirection enemyAimDirection = PlayerUtils.GetAimDirection(enemyAngleDegrees);
        
        // Trigger weapon aim event
        enemy.WeaponAimEvent.CallAimWeaponEvent(enemyAimDirection, enemyAngleDegrees, weaponAngleDegrees, weaponDirection);

        // Only fire if enemy has a weapon
        if (enemyDetails.EnemyWeapon != null)
        {
            // Get ammo range
            float enemyAmmoRange = enemyDetails.EnemyWeapon.WeaponCurrentAmmo.AmmoRange;

            // Is the player in range
            if (playerDirectionVector.magnitude <= enemyAmmoRange)
            {
                // Does this enemy require line of sight to the player before firing?
                if (enemyDetails.FiringLineOfSightRequired && !IsPlayerInLineOfSight(weaponDirection, enemyAmmoRange)) return;

                // Trigger fire weapon event
                enemy.WeaponFireEvent.CallFireWeaponEvent(true, true, enemyAimDirection, enemyAngleDegrees, weaponAngleDegrees, weaponDirection);
            }
        }

    }

    private bool IsPlayerInLineOfSight(Vector3 weaponDirection, float enemyAmmoRange)
    {
        RaycastHit2D raycastHit2D = Physics2D.Raycast(WeaponShootPosition.position, (Vector2)weaponDirection, enemyAmmoRange, layerMask);

        if (raycastHit2D && raycastHit2D.transform.CompareTag(Settings.PlayerTag))
        {
            return true;
        }

        return false;
    }

    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        EditorUtilities.ValidateCheckNullValue(this, nameof(WeaponShootPosition), WeaponShootPosition);
    }

#endif
    #endregion Validation
}