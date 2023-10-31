using UnityEngine;

public class Enemey : MovingObjects
{
    public int playerDamage;

    private Animator _animator;
    private Transform _target; 
    private bool _skipMove;
    
    protected override void Start()
    {
        _animator = GetComponent<Animator>();
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;
        
        hitPlayer.Health.LoseHealth(playerDamage);
    }

    protected override void AttemptMove<T>(int xdir, int ydir)
    {
        if (_skipMove)
        {
            _skipMove = false;
            return;
        }
        
        base.AttemptMove<T>(xdir, ydir);
    }

    public void MoveEnemy()
    {
        var xdir = 0;
        var ydir = 0;

        if (Mathf.Abs(_target.position.x - transform.position.x) < float.Epsilon)
        {
            ydir = _target.position.y > transform.position.y ? 1 : -1;
        }
        else
        {
            xdir = _target.position.x > transform.position.x ? 1 : -1;
        }
        
        AttemptMove<Player>(xdir,ydir);
    }
}
