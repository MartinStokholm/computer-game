using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyMovementAI : MonoBehaviour
{

    #region Tooltip
    [Tooltip("MovementDetailsSO scriptable object containing movement details")]
    #endregion Tooltip
    [HideInInspector] public float MoveSpeed;
    
    [SerializeField] private MovementDetailsSO _movementDetails;
    
    private Enemy _enemy;
    private Stack<Vector3> movementSteps = new();
    private Vector3 _playerReferencePosition;
    private Coroutine moveEnemyRoutine;
    private float _currentEnemyPathRebuildCooldown;
    private WaitForFixedUpdate _waitForFixedUpdate;
    [HideInInspector] public float _movementSpeed;
    private bool _chasePlayer = false;
    [HideInInspector] public int UpdateFrameNumber = 1; //This is set by the enemy spawner.
    private List<Vector2Int> surroundingPositionList = new();

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _movementSpeed = _movementDetails.GetMovementSpeed();
    }

    protected void Start()
    {
        _waitForFixedUpdate = new WaitForFixedUpdate();
        _playerReferencePosition = GameManager.Instance.Player.GetPlayerPosition();
    }

    private void Update()
    {
        MoveEnemy();
    }

    private void MoveEnemy()
    {
        // Movement cooldown timer
        _currentEnemyPathRebuildCooldown -= Time.deltaTime;

        // Check distance to player to see if enemy should start chasing
        if (!_chasePlayer && Vector3.Distance(transform.position, GameManager.Instance.Player.GetPlayerPosition()) < _enemy.EnemyDetails.ChaseDistance)
        {
            Debug.Log("MoveEnemy _chasePlayer: " + _chasePlayer);
            _chasePlayer = true;
        }

        // If not close enough to chase player then return
        if (!_chasePlayer)
            return;

        Debug.Log("MoveEnemy A Star path rebuild on certain frames to spread the load between enemies: " + (Time.frameCount % Settings.TargetFrameRateToSpreadPathfindingOver != UpdateFrameNumber));
        // Only process A Star path rebuild on certain frames to spread the load between enemies
        if (Time.frameCount % Settings.TargetFrameRateToSpreadPathfindingOver != UpdateFrameNumber) return;

        // if the movement cooldown timer reached or player has moved more than required distance
        // then rebuild the enemy path and move the enemy
        if (_currentEnemyPathRebuildCooldown <= 0f || (Vector3.Distance(_playerReferencePosition, GameManager.Instance.Player.GetPlayerPosition()) > Settings.PlayerMoveDistanceToRebuildPath))
        {
            // Reset path rebuild cooldown timer
            _currentEnemyPathRebuildCooldown = Settings.EnemyPathRebuildCooldown;

            // Reset player reference position
            _playerReferencePosition = GameManager.Instance.Player.GetPlayerPosition();

            // Move the enemy using AStar pathfinding - Trigger rebuild of path to player
            CreatePath();

            // If a path has been found move the enemy
            if (movementSteps != null)
            {
                if (moveEnemyRoutine != null)
                {
                    // Trigger idle event
                    _enemy.IdleEvent.CallIdleEvent();
                    StopCoroutine(moveEnemyRoutine);
                }

                // Move enemy along the path using a coroutine
                moveEnemyRoutine = StartCoroutine(MoveEnemyRoutine(movementSteps));

            }
        }
    }
    
    /// <summary>
    /// Coroutine to move the enemy to the next location on the path
    /// </summary>
    private IEnumerator MoveEnemyRoutine(Stack<Vector3> movementSteps)
    {
        while (movementSteps.Count > 0)
        {
            var nextPosition = movementSteps.Pop();

            // while not very close continue to move - when close move onto the next step
            while (Vector3.Distance(nextPosition, transform.position) > 0.2f)
            {
                // Trigger movement event
                _enemy.MovementToPositionEvent.CallMovementToPositionEvent(nextPosition, transform.position, MoveSpeed, (nextPosition - transform.position).normalized);

                yield return _waitForFixedUpdate;  // moving the enmy using 2D physics so wait until the next fixed update

            }

            yield return _waitForFixedUpdate;
        }

        // End of path steps - trigger the enemy idle event
        _enemy.IdleEvent.CallIdleEvent();
    }


    /// <summary>
    /// Use the AStar static class to create a path for the enemy
    /// </summary>
    private void CreatePath()
    {
        var currentRoom = GameManager.Instance.CurrentRoom;

        Grid grid = currentRoom.InstantiatedRoom.Grid;

        // Get players position on the grid
        Vector3Int playerGridPosition = GetNearestNonObstaclePlayerPosition(currentRoom);


        // Get enemy position on the grid
        Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);

        // Build a path for the enemy to move on
        movementSteps = AStar.BuildPath(currentRoom, enemyGridPosition, playerGridPosition);

        // Take off first step on path - this is the grid square the enemy is already on
        if (movementSteps != null)
        {
            movementSteps.Pop();
        }
        else
        {
            // Trigger idle event - no path
            _enemy.IdleEvent.CallIdleEvent();
        }
    }
    
        /// <summary>
    /// Get the nearest position to the player that isn't on an obstacle
    /// </summary>
    private Vector3Int GetNearestNonObstaclePlayerPosition(Room currentRoom)
    {
        Vector3 playerPosition = GameManager.Instance.Player.GetPlayerPosition();

        Vector3Int playerCellPosition = currentRoom.InstantiatedRoom.Grid.WorldToCell(playerPosition);

        Vector2Int adjustedPlayerCellPositon = new Vector2Int(
            playerCellPosition.x - currentRoom.TemplateLowerBounds.x, 
            playerCellPosition.y - currentRoom.TemplateLowerBounds.y);

        int obstacle = Mathf.Min(
            currentRoom.InstantiatedRoom.AStarMovementPenalty[adjustedPlayerCellPositon.x, adjustedPlayerCellPositon.y], 
            currentRoom.InstantiatedRoom.AStarItemObstacles[adjustedPlayerCellPositon.x, adjustedPlayerCellPositon.y]);

        // if the player isn't on a cell square marked as an obstacle then return that position
        if (obstacle != 0)
        {
            return playerCellPosition;
        }
        // find a surounding cell that isn't an obstacle - required because with the 'half collision' tiles
        // and tables the player can be on a grid square that is marked as an obstacle
        else
        {
            // Empty surrounding position list
            surroundingPositionList.Clear();

            // Populate surrounding position list - this will hold the 8 possible vector locations surrounding a (0,0) grid square
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (j == 0 && i == 0) continue;

                    surroundingPositionList.Add(new Vector2Int(i, j));
                }
            }


            // Loop through all positions
            for (int l = 0; l < 8; l++)
            {
                // Generate a random index for the list
                int index = Random.Range(0, surroundingPositionList.Count);

                // See if there is an obstacle in the selected surrounding position
                try
                {
                    obstacle = Mathf.Min(
                        currentRoom.InstantiatedRoom.AStarMovementPenalty[
                            adjustedPlayerCellPositon.x + surroundingPositionList[index].x, 
                            adjustedPlayerCellPositon.y + surroundingPositionList[index].y], 
                        currentRoom.InstantiatedRoom.AStarItemObstacles[
                            adjustedPlayerCellPositon.x + surroundingPositionList[index].x, 
                            adjustedPlayerCellPositon.y + surroundingPositionList[index].y]);

                    // If no obstacle return the cell position to navigate to
                    if (obstacle != 0)
                    {
                        return new Vector3Int(
                            playerCellPosition.x + surroundingPositionList[index].x, 
                            playerCellPosition.y + surroundingPositionList[index].y, 
                            0);
                    }

                }
                // Catch errors where the surrounding positon is outside the grid
                catch
                {

                }

                // Remove the surrounding position with the obstacle so we can try again
                surroundingPositionList.RemoveAt(index);
            }


            // If no non-obstacle cells found surrounding the player - send the enemy in the direction of an enemy spawn position
            return (Vector3Int)currentRoom.SpawnPositionArray[Random.Range(0, currentRoom.SpawnPositionArray.Length)];

        }
    }
        
    /// <summary>
    /// Set the frame number that the enemy path will be recalculated on - to avoid performance spikes
    /// </summary>
    public void SetUpdateFrameNumber(int updateFrameNumber)
    {
        UpdateFrameNumber = updateFrameNumber;
    }
    
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
