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
    }
    
    private void OnDisable()
    {
        _enemy.MovementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
        _enemy.IdleEvent.OnIdle -= IdleEvent_OnIdle;
    }
    
    /// <summary>
    /// On movement event handler
    /// </summary>
    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent, MovementToPositionArgs movementToPositionArgs)
    {
        SetMovementAnimationParameters();
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
}
