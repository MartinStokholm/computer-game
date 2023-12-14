using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyMovementAI : MovingObjects
{
    [HideInInspector] public int UpdateFrameNumber = 1; //This is set by the enemy spawner.
    #region Tooltip
    [Tooltip("MovementDetailsSO scriptable object containing movement details")]
    #endregion Tooltip
    [SerializeField] private MovementDetailsSO _movementDetails;
    private WaitForFixedUpdate _waitForFixedUpdate;
    private Enemy _enemy;
    //private Stack<Vector3> movementSteps = new Stack<Vector3>();
    private Coroutine moveEnemyRoutine; 
    private float _movementSpeed;
    private Vector3 _playerPosition;
    public int playerDamage;
    private Transform _target; 
    private bool _skipMove;
    private bool _chasePlayer = false;
    private float _currentEnemyPathRebuildCooldown;
    
    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _movementSpeed = _movementDetails.GetMovementSpeed();
    }

    protected override void Start()
    {
        _waitForFixedUpdate = new WaitForFixedUpdate();
        _playerPosition = GetPlayerPosition();
        _target = GameObject.FindGameObjectWithTag(Settings.PlayerTag).transform;
        base.Start();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        // Movement cooldown timer
        _currentEnemyPathRebuildCooldown -= Time.deltaTime;
        
        if (!_chasePlayer || Vector3.Distance(transform.position, GetPlayerPosition()) < _enemy.EnemyDetails.ChaseDistance)
        {
            _chasePlayer = true;
        }
        
        if (!_chasePlayer) return;
        
        // Only process A Star path rebuild on certain frames to spread the load between enemies
        if (IsAStarPathRebuildRequired()) return;

        if (!IsMovementCooldownTimerReached() && !IsPlayerMovedMoreThanRequiredDistance()) return;
        
        _currentEnemyPathRebuildCooldown = Settings.EnemyPathRebuildCooldown;

        _playerPosition = GetPlayerPosition();

        MoveEnemy();

        // if (movementSteps == null) return;
        //
        // if (moveEnemyRoutine != null)
        // {
        //     // Trigger idle event
        //     _enemy.IdleEvent.CallIdleEvent();
        //     StopCoroutine(moveEnemyRoutine);
        // }
        //
        // // Move enemy along the path using a coroutine
        // moveEnemyRoutine = StartCoroutine(MoveEnemyRoutine(movementSteps));
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
        _skipMove = true;
    }

    public void MoveEnemy()
    {
        var xDir = (_target.position.x > transform.position.x) ? 1 : (_target.position.x < transform.position.x) ? -1 : 0;
        var yDir = (_target.position.y > transform.position.y) ? 1 : (_target.position.y < transform.position.y) ? -1 : 0;
        
        AttemptMove<Player>(xDir,yDir);
    }

    // private void CreatePath()
    // {
    //     var currentRoom = GameManager.Instance.CurrentRoom;
    //
    //     var grid = currentRoom.InstantiatedRoom.Grid;
    //
    //     // Get players position on the grid
    //     var playerGridPosition = GetNearestNonObstaclePlayerPosition(currentRoom);
    //
    //
    //     // Get enemy position on the grid
    //     Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);
    //
    //     // Build a path for the enemy to move on
    //     movementSteps = AStar.BuildPath(currentRoom, enemyGridPosition, playerGridPosition);
    //
    //     // Take off first step on path - this is the grid square the enemy is already on
    //     if (movementSteps != null)
    //     {
    //         movementSteps.Pop();
    //     }
    //     else
    //     {
    //         // Trigger idle event - no path
    //         _enemy.IdleEvent.CallIdleEvent();
    //     }
    // }

    // private IEnumerator MoveEnemyRoutine(Stack<Vector3> movementSteps)
    // {
    //     yield break;
    // }


    private bool IsAStarPathRebuildRequired() => Time.frameCount % Settings.TargetFrameRateToSpreadPathfindingOver != UpdateFrameNumber;
    private bool IsMovementCooldownTimerReached() => _currentEnemyPathRebuildCooldown <= 0f;

    private bool IsPlayerMovedMoreThanRequiredDistance() =>
        Vector3.Distance(transform.position, GetPlayerPosition()) <
        _enemy.EnemyDetails.ChaseDistance;
    
    private Vector3 GetPlayerPosition() => GameManager.Instance.Player.GetPlayerPosition();
    
    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        EditorUtilities.ValidateCheckNullValue(this, nameof(_movementDetails), _movementDetails);
    }

#endif

    #endregion Validation
}
