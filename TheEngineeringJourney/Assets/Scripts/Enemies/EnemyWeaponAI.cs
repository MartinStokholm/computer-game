using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyWeaponAI : MonoBehaviour
{
    private Enemy _enemy;
    private EnemyDetailsSO _enemyDetails;
    private void Awake()
    {
        // Load Components
       _enemy = GetComponent<Enemy>();
    }
    
    private void Start()
    {
        _enemyDetails = _enemy.EnemyDetails;
    }

    private void Update()
    {
        AimDirection();
    }

    private void AimDirection()
    {
        var playerDirectionVector = GameManager.Instance.Player.GetPlayerPosition() - transform.position;
        var enemyAngleDegrees = GameUtilities.GetAngleFromVector(playerDirectionVector);
        
        var enemyAimDirection = PlayerUtils.GetAimDirection(enemyAngleDegrees);
        
        _enemy.AimWeaponEvent.CallAimWeaponEvent(enemyAimDirection, enemyAngleDegrees, 0f, new Vector3(0,0,0));
    }
    
    private void FireWeapon()
    {
        
    }

    private bool IsPlayerInLineOfSight(Vector3 weaponDirection, float enemyAmmoRange)
    {
        return true;
    }
    
}
