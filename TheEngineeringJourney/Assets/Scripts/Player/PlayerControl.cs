using System;
using System.Collections;
using System.Collections.Generic;
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
        MovementInput();
    }
    
    /// <summary>
    /// Player movement input
    /// </summary>
    private void MovementInput()
    {
        // Get movement input
        var horizontalMovement = Input.GetAxisRaw("Horizontal");
        var verticalMovement = Input.GetAxisRaw("Vertical");
        var rightMouseButtonDown = Input.GetMouseButtonDown(1);

        // Create a direction vector based on the input
        var direction = new Vector2(horizontalMovement, verticalMovement);

        // Adjust distance for diagonal movement (pythagoras approximation)
        if (horizontalMovement != 0f && verticalMovement != 0f)
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
    
    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(MovementDetailsSO), _movementDetail);
    }

#endif

    #endregion Validation
}
