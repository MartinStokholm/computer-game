using UnityEngine;
using System.Collections;

public abstract class MovingObjects : MonoBehaviour
{
    public float moveTime = 0.1f;
    public LayerMask blockingLayer;

    private BoxCollider2D _boxCollider2D;
    private Rigidbody2D _rigidbody2D;
    private float _inverseMoveTime;

    protected virtual void Start()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _inverseMoveTime = 1f / moveTime;
    }

    private IEnumerator SmoothMovement(Vector3 end)
    {
        var remainingDistance = (transform.position - end).sqrMagnitude;

        while (remainingDistance > float.Epsilon)
        {
            var newPos = Vector3.MoveTowards(_rigidbody2D.position, end, _inverseMoveTime * Time.deltaTime);
            _rigidbody2D.MovePosition(newPos);
            remainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }
    
    protected abstract void OnCantMove<T>(T component) where T : Component;
    
    private bool Move(int xDirection, int yDirection, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        var end = start + new Vector2(xDirection, yDirection);

        _boxCollider2D.enabled = false; 
        
        hit = Physics2D.Linecast(start,end,blockingLayer);

        if (hit.transform is not null) return false;
        
        StartCoroutine(SmoothMovement(end));
        return true;
    }

    protected virtual void AttemptMove<T>(int xDirection, int yDirection) where T : Component
    {
        var canMove = Move(xDirection, yDirection, out var hit);
        
        if (hit.transform is null) return;

        var hitComponent = hit.transform.GetComponent<T>();

        if (!canMove && hitComponent is not null)
        {
            OnCantMove(hitComponent);
        }
    }
}
