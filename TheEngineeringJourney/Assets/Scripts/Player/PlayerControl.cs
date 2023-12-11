using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    #region Tooltip
    [Tooltip("MovementDetailsSO containing movement details as speed")]
    #endregion
    [SerializeField] private MovementDetailsSO _movementDetail;
    
    #region Tooltip

    [Tooltip("The player WeaponShootPosition game object in the hierarchy")]

    #endregion Tooltip

    [SerializeField] private Transform weaponShootPosition;
    
    private Player _player;
    private float _moveSpeed;
    private Coroutine _playerRollCoroutine;
    private WaitForFixedUpdate _waitForFixedUpdate;
    //private bool _isPlayerRolling = false;
    //private float _playerRollCooldownTimer = 0f;

    private void Awake()
    { 
        _player = GetComponent<Player>();
        
        _moveSpeed = _movementDetail.GetMovementSpeed();
    }

    /// <summary>
    /// Create wait for fixed update for use in coroutine
    /// </summary>
    private void Start()
    {
        _waitForFixedUpdate = new WaitForFixedUpdate();
    }

    private void Update()
    {

        // Don't move when dialogue system is play???
        if (DialogueManager.Instance.IsDialoguePlaying) return;
        MovementInput();
        
        WeaponInput();
    }
    
    /// <summary>
    /// Player movement input
    /// </summary>
    private void MovementInput()
    {
        // Get movement input
        var direction = InputManager.Instance.GetMoveDirection();
        var rightMouseButtonDown = Input.GetMouseButtonDown(1);
        

        // Adjust distance for diagonal movement (pythagoras approximation)
        if (direction.x != 0f && direction.y != 0f)
        {
            direction *= 0.7f;
        }

        // If there is movement either move or roll
        if (direction != Vector2.zero)
        {
            if (!rightMouseButtonDown)
            {
                // trigger movement event
                _player.MovementByVelocityEvent.CallMovementByVelocityEvent(direction, _moveSpeed);
            }
            // else player roll if not cooling down
            // else if (_playerRollCooldownTimer <= 0f)
            // {
            //     PlayerRoll((Vector3)direction);
            // }

        }
        // else trigger idle event
        else
        {
            _player.IdleEvent.CallIdleEvent();
        }
    }

    private void WeaponInput()
    {
        var mouseWorldPosition = GameUtilities.GetMouseWorldPosition();
        var weaponDirection = (mouseWorldPosition);

        // Calculate direction vector of mouse cursor from player transform position
        var playerDirection = (mouseWorldPosition - transform.position);

        // Get weapon to cursor angle
        var weaponAngleDegrees = GameUtilities.GetAngleFromVector(weaponDirection);

        // Get player to cursor angle
        var playerAngleDegrees = GameUtilities.GetAngleFromVector(playerDirection);

        // Set player aim direction
        var playerAimDirection = PlayerUtils.GetAimDirection(playerAngleDegrees);
        
        
        // Trigger weapon aim event
        _player.AimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
    }


    
    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        EditorUtilities.ValidateCheckNullValue(this, nameof(MovementDetailsSO), _movementDetail);
    }

#endif

    #endregion Validation
}
