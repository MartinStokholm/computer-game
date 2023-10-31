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
            Vector3 newPos = Vector3.MoveTowards(_rigidbody2D.position, end, _inverseMoveTime * Time.deltaTime);
            _rigidbody2D.MovePosition(newPos);
            remainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }
    
    protected abstract void OnCantMove<T>(T component) where T : Component;
    
    private bool Move(int xdir, int ydir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xdir, ydir);

        _boxCollider2D.enabled = false; 
        
        hit = Physics2D.Linecast(start,end,blockingLayer);

        if (hit.transform != null) return false;
        StartCoroutine(SmoothMovement(end));
        return true;

    }

    protected virtual void AttemptMove<T>(int xdir, int ydir) where T : Component
    {
        RaycastHit2D hit; 
        var canMove = Move(xdir, ydir, out hit);
        
        if (hit.transform == null) return;

        T hitComponent = hit.transform.GetComponent<T>();

        if (!canMove && hitComponent != null)
        {
            OnCantMove(hitComponent);
        }
    }
}
