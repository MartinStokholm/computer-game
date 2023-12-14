
using UnityEngine;

public class AmmoPattern : MonoBehaviour, IFireAble
{
    #region Tooltip
    [Tooltip("Populate the array with the child ammo gameobjects")]
    #endregion
    [SerializeField] private Ammo[] AmmoArray;

    private float AmmoRange;
    private float AmmoSpeed;
    private Vector3 FireDirectionVector;
    private float FireDirectionAngle;
    private AmmoDetailsSO AmmoDetails;
    private float AmmoChargeTimer;

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void InitialiseAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, bool overrideAmmoMovement)
    {
        AmmoDetails = ammoDetails;
        AmmoSpeed = ammoSpeed;

        // Set fire direction
        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

        // Set ammo range
        AmmoRange = ammoDetails.AmmoRange;

        // Activate ammo pattern gameobject
        gameObject.SetActive(true);

        // Loop through all child ammo and initialise it
        foreach (var ammo in AmmoArray)
        {
            ammo.InitialiseAmmo(ammoDetails, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector, true);
        }

        // Set ammo charge timer - this will hold the ammo briefly
        AmmoChargeTimer = ammoDetails.AmmoChargeTime > 0f 
            ? ammoDetails.AmmoChargeTime 
            : 0f;
    }

    private void Update()
    {
        // Ammo charge effect
        if (AmmoChargeTimer > 0f)
        {
            AmmoChargeTimer -= Time.deltaTime;
            return;
        }

        // Calculate distance vector to move ammo
        var distanceVector = FireDirectionVector * (AmmoSpeed * Time.deltaTime);

        transform.position += distanceVector;

        // Rotate ammo
        transform.Rotate(new Vector3(0f, 0f, AmmoDetails.AmmoRotationSpeed * Time.deltaTime));

        // Disable after max range reached
        AmmoRange -= distanceVector.magnitude;

        if (AmmoRange < 0f)
        {
            DisableAmmo();
        }
    }

    /// <summary>
    /// Set ammo fire direction based on the input angle and direction adjusted by the
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

        // Set ammo fire direction
        FireDirectionVector = PlayerUtils.GetDirectionVectorFromAngle(FireDirectionAngle);
    }

    /// <summary>
    /// Disable the ammo - thus returning it to the object pool
    /// </summary>
    private void DisableAmmo()
    {
        // Disable the ammo pattern game object
        gameObject.SetActive(false);
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        EditorUtilities.ValidateCheckEnumerableValues(this, nameof(AmmoArray), AmmoArray);
    }
#endif
    #endregion
}