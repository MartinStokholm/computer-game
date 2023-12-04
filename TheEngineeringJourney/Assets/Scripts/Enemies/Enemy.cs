using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Enemy : MovingObjects
{
    [HideInInspector] public EnemyDetailsSO EnemyDetails;
    [HideInInspector] public SpriteRenderer[] spriteRendererArray;
    
    private MaterializeEffect MaterializeEffect;
    private CircleCollider2D circleCollider2D;
    private PolygonCollider2D polygonCollider2D;
    
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
        var hitPlayer = component as Player;
        
        hitPlayer.Health.LoseHealth(playerDamage);
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (_skipMove)
        {
            _skipMove = false;
            return;
        }
        
        base.AttemptMove<T>(xDir, yDir);
    }

    public void MoveEnemy()
    {
        var xDir = 0;
        var yDir = 0;

        if (Mathf.Abs(_target.position.x - transform.position.x) < float.Epsilon)
        {
            yDir = _target.position.y > transform.position.y ? 1 : -1;
        }
        else
        {
            xDir = _target.position.x > transform.position.x ? 1 : -1;
        }
        
        AttemptMove<Player>(xDir,yDir);
    }
    
    
    /// <summary>
    /// Initialise the enemy
    /// </summary>
    public void EnemyInitialization(EnemyDetailsSO enemyDetails, int enemySpawnNumber, MapLevelSO mapLevel)
    {
        this.EnemyDetails = enemyDetails;

        // SetEnemyMovementUpdateFrame(enemySpawnNumber);
        //
        // SetEnemyStartingHealth(dungeonLevel);
        //
        // SetEnemyStartingWeapon();
        //
        // SetEnemyAnimationSpeed();

        // Materialise enemy
        StartCoroutine(MaterializeEnemy());
    }
    
    private IEnumerator MaterializeEnemy()
    {
        // Disable collider, Movement AI and Weapon AI
        EnemyEnable(false);

        yield return StartCoroutine(MaterializeEffect.MaterializeRoutine(EnemyDetails.enemyMaterializeShader, EnemyDetails.enemyMaterializeColor, EnemyDetails.enemyMaterializeTime, spriteRendererArray, EnemyDetails.enemyStandardMaterial));

        // Enable collider, Movement AI and Weapon AI
        EnemyEnable(true);

    }
    
    private void EnemyEnable(bool isEnabled)
    {
        // Enable/Disable colliders
        // circleCollider2D.enabled = isEnabled;
        // polygonCollider2D.enabled = isEnabled;
        //
        // // Enable/Disable movement AI
        // enemyMovementAI.enabled = isEnabled;
        //
        // // Enable / Disable Fire Weapon
        // fireWeapon.enabled = isEnabled;
    }
}
