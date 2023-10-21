using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[DisallowMultipleComponent]
public class MovementByVelocity : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    private MovementByVelocityEvent _movementByVelocityEvent;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
    }

    private void OnEnable()
    {
        _movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;
    }
    
    private void OnDisable()
    {
        _movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;
    }

    /// <summary>
    /// Move the rigidbody component
    /// </summary>
    private void MovementByVelocityEvent_OnMovementByVelocity(
        MovementByVelocityEvent @event,
        MovementByVelocityArgs args)
    {
        _rigidbody2D.velocity = args.MovementDirection * args.MovementSpeed;
    }
}
