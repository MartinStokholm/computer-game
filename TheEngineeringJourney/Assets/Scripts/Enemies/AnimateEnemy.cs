using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class AnimateEnemy : MonoBehaviour
{
    private Enemy _enemy;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        _enemy.MovementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;
        _enemy.IdleEvent.OnIdle += IdleEvent_OnIdle;
        _enemy.WeaponAimEvent.OnWeaponAim += WeaponAimEventOnWeaponAim;
    }
    
    private void OnDisable()
    {
        _enemy.MovementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
        _enemy.IdleEvent.OnIdle -= IdleEvent_OnIdle;
        _enemy.WeaponAimEvent.OnWeaponAim -= WeaponAimEventOnWeaponAim;
    }
    
    /// <summary>
    /// On movement event handler
    /// </summary>
    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent, MovementToPositionArgs movementToPositionArgs)
    {
        SetMovementAnimationParameters();
    }
    
    /// <summary>
    /// On weapon aim event handler
    /// </summary>
    private void WeaponAimEventOnWeaponAim(WeaponAimEvent weaponAimEvent, AimWeaponEventArgs aimWeaponEventArgs)
    {
        InitialiseAimAnimationParameters();
        SetAimWeaponAnimationParameters(aimWeaponEventArgs.AimDirection);
    }
    
    /// <summary>
    /// Set movement animation parameters
    /// </summary>
    private void SetMovementAnimationParameters()
    {
        _enemy.Animator.SetBool(Settings.IsIdle, false);
        _enemy.Animator.SetBool(Settings.IsMoving, true);
    }
    
    /// <summary>
    /// On idle event handler
    /// </summary>
    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        SetIdleAnimationParameters();
    }
    
    /// <summary>
    /// Set idle animation parameters
    /// </summary>
    private void SetIdleAnimationParameters()
    {
        _enemy.Animator.SetBool(Settings.IsIdle, true);
        _enemy.Animator.SetBool(Settings.IsMoving, false);
    }
    
    /// <summary>
    /// Initialise aim animation parameters
    /// </summary>
    private void InitialiseAimAnimationParameters()
    {
        _enemy.Animator.SetBool(Settings.AimUp, false);
        _enemy.Animator.SetBool(Settings.AimRight, false);
        _enemy.Animator.SetBool(Settings.AimLeft, false);
        _enemy.Animator.SetBool(Settings.AimDown, false);
    }
    
    private void SetAimWeaponAnimationParameters(AimDirection aimDirection)
    {
        // Set aim direction
        switch (aimDirection)
        {
            case AimDirection.Up:
                _enemy.Animator.SetBool(Settings.AimUp, true);
                break;

            case AimDirection.Right:
                _enemy.Animator.SetBool(Settings.AimRight, true);
                break;

            case AimDirection.Left:
                _enemy.Animator.SetBool(Settings.AimLeft, true);
                break;

            case AimDirection.Down:
                _enemy.Animator.SetBool(Settings.AimDown, true);
                break;
        }
    }

}
