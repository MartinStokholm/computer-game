using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class AnimatePlayer : MonoBehaviour
{
    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        _player.MovementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;
        _player.MovementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;
        _player.IdleEvent.OnIdle += IdleEvent_OnIdle;
        _player.AimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
        
    }
    
    private void OnDisable()
    {
        _player.MovementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;
        _player.MovementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
        _player.IdleEvent.OnIdle -= IdleEvent_OnIdle; 
        _player.AimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
    }

    private void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent @event, MovementByVelocityArgs args)
    {
        SetMovementAnimationParameters();
    }

    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent @event, MovementToPositionArgs args)
    {
        InitializeAimAnimationParameters();
        SetMovementAnimationParameters();
    }

    private void IdleEvent_OnIdle(IdleEvent @event)
    {
        SetIdleAnimationParameters();
    }

    private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArgs aimWeaponEventArgs)
    {
        InitializeAimAnimationParameters();
        SetAimWeaponAnimationParameters(aimWeaponEventArgs.AimDirection);
    }
    
    /// <summary>
    /// Set movement animation parameters
    /// </summary>
    private void SetMovementAnimationParameters()
    {
        _player.Animator.SetBool(Settings.IsMoving, true);
        _player.Animator.SetBool(Settings.IsIdle, false);
    }
    
    /// <summary>
    /// Set movement animation parameters
    /// </summary>
    private void SetIdleAnimationParameters()
    {
        _player.Animator.SetBool(Settings.IsMoving, false);
        _player.Animator.SetBool(Settings.IsIdle, true);
    }
    

    #region Weapons
    /// <summary>
    /// Initialise aim animation parameters
    /// </summary>
    private void InitializeAimAnimationParameters()
    {
        _player.Animator.SetBool(Settings.AimUp, false);
        _player.Animator.SetBool(Settings.AimRight, false);
        _player.Animator.SetBool(Settings.AimLeft, false);
        _player.Animator.SetBool(Settings.AimDown, false);
    }
    /// <summary>
    /// Set aim animation parameters
    /// </summary>
    private void SetAimWeaponAnimationParameters(AimDirection aimDirection)
    {
        // Set aim direction
        switch (aimDirection)
        {
            case AimDirection.Up:
                _player.Animator.SetBool(Settings.AimUp, true);
                break;

            case AimDirection.Right:
                _player.Animator.SetBool(Settings.AimRight, true);
                break;

            case AimDirection.Left:
                _player.Animator.SetBool(Settings.AimLeft, true);
                break;

            case AimDirection.Down:
                _player.Animator.SetBool(Settings.AimDown, true);
                break;
        }
    }
    

    #endregion
    
}
