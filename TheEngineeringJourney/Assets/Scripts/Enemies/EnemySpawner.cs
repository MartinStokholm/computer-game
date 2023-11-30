using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemeySpawner : SingletonMonobehaviour<EnemeySpawner>
{
    private int _enemiesToSpawn;
    private int _currentEnemyCount;
    private int _enemiesSpawnedSoFar;
    private int _enemyMaxConcurrentSpawnNumber;
    private RoomEnemySpawnParameters roomEnemySpawnParameters;
    private Room _currentRoom;
    
    private void OnEnable()
    {
        // subscribe to room changed event
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        // unsubscribe from room changed event
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        _currentRoom = roomChangedEventArgs.Room;
        _enemiesToSpawn = _currentRoom.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentMapLevel());
        
        // if the room is a corridor or the entrance then return
        if (_currentRoom.RoomNodeType.isCorridorEW || _currentRoom.RoomNodeType.isCorridorNS || _currentRoom.RoomNodeType.isEntrance)
            return;

        // if the room has already been defeated then return
        if (_currentRoom.IsClearedOfEnemies) return;
        Debug.Log("GameManager.Instance.GetCurrentMapLevel: "+ GameManager.Instance.GetCurrentMapLevel());
        // Get random number of enemies to spawn
        //_enemiesToSpawn = _currentRoom.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentMapLevel());

        // Get room enemy spawn parameters
        //roomEnemySpawnParameters = _currentRoom.GetRoomEnemySpawnParameters(GameManager.Instance.GetCurrentMapLevel());
        //_enemiesToSpawn = _currentRoom
            // .RoomEnemySpawnParametersList
            // .GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentMapLevel());
            
        //This line does not what you expect
        //It can't find the roomEnemySpawnParameters... :angry_smiley: IDK WHY??!?!?!
        roomEnemySpawnParameters = _currentRoom.GetRoomEnemySpawnParameters(GameManager.Instance.GetCurrentMapLevel());
        Debug.Log($"roomEnemySpawnParameters in enemy spawner: {roomEnemySpawnParameters.MinTotalEnemiesToSpawn} - {roomEnemySpawnParameters.MaxTotalEnemiesToSpawn}");
        _enemyMaxConcurrentSpawnNumber = roomEnemySpawnParameters.MaxTotalEnemiesToSpawn;
        // If no enemies to spawn return
        if (_enemiesToSpawn == 0)
        {
            // Mark the room as cleared
            _currentRoom.IsClearedOfEnemies = true;
        
            return;
        }
        
        SpawnEnemies();
    }


    /// <summary>
    /// Spawn the enemies
    /// </summary>
    private void SpawnEnemies()
    {
        // Set gamestate engaging boss
        Debug.Log("GameState.EngagingEnemies");
        GameManager.Instance.GameState = GameState.EngagingEnemies;
        StartCoroutine(SpawnEnemiesRoutine());
    }
    
    /// <summary>
    /// Get a random spawn interval between the minimum and maximum values
    /// </summary>
    private float GetEnemySpawnInterval()
    {
        return (Random.Range(roomEnemySpawnParameters.MinSpawnInterval, roomEnemySpawnParameters.MaxSpawnInterval));
    }

    /// <summary>
    /// Get a random number of concurrent enemies between the minimum and maximum values
    /// </summary>
    private int GetConcurrentEnemies()
    {
        return (Random.Range(roomEnemySpawnParameters.MinConcurrentEnemies, roomEnemySpawnParameters.MaxConcurrentEnemies));
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        Debug.Log("SpawnEnemiesRoutine");
        var grid = _currentRoom.InstantiatedRoom.Grid;
        Debug.Log($" Check we have somewhere to spawn the enemies: {_currentRoom.SpawnPositionArray.Length}");
        // Get an instance of the helper class used to select a random enemy
        var randomEnemyHelperClass = new RandomSpawnableObject<EnemyDetailsSO>(_currentRoom.EnemiesByLevels);

        // Check we have somewhere to spawn the enemies
        Debug.Log($" Check we have somewhere to spawn the enemies: {_currentRoom.SpawnPositionArray.Length}");
        if (_currentRoom.SpawnPositionArray.Length <= 0) yield break;
        
        // Loop through to create all the enemeies
        
        for (var i = 0; i < _enemiesToSpawn; i++)
        {
            //wait until current enemy count is less than max concurrent enemies
            while (_currentEnemyCount >= _enemyMaxConcurrentSpawnNumber)
            {
                Debug.Log($"Waiting to spawn, because there is {_currentEnemyCount} and there can only be {_enemyMaxConcurrentSpawnNumber}");
                yield return null;
            }

            var cellPosition = (Vector3Int)_currentRoom.SpawnPositionArray[Random.Range(0, _currentRoom.SpawnPositionArray.Length)];

            // Create Enemy - Get next enemy type to spawn 
            Debug.Log("CreateEnemy");
            CreateEnemy(randomEnemyHelperClass.GetItem(), grid.CellToWorld(cellPosition));

            yield return new WaitForSeconds(GetEnemySpawnInterval());
        }
    }
    
    /// <summary>
    /// Create an enemy in the specified position
    /// </summary>
    private void CreateEnemy(EnemyDetailsSO enemyDetails, Vector3 position)
    {
        // keep track of the number of enemies spawned so far 
        _enemiesSpawnedSoFar++;
    
        // Add one to the current enemy count - this is reduced when an enemy is destroyed
        _currentEnemyCount++;
    
        // Get current dungeon level
        var mapLevel = GameManager.Instance.GetCurrentMapLevel();
    
        // Instantiate enemy
        var enemy = Instantiate(enemyDetails.enemyPrefab, position, Quaternion.identity, transform);
    
        Debug.Log("Create Enemy");
        // // Initialize Enemy
        //enemy.GetComponent<Enemy>().EnemyInitialization(enemyDetails, _enemiesSpawnedSoFar, mapLevel);
        //
        // // subscribe to enemy destroyed event
        // enemy.GetComponent<DestroyedEvent>().OnDestroyed += Enemy_OnDestroyed;
    }
    
    /// <summary>
    /// Process enemy destroyed
    /// </summary>
    // private void Enemy_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    // {
    //     // Unsubscribe from event
    //     destroyedEvent.OnDestroyed -= Enemy_OnDestroyed;
    //
    //     // reduce current enemy count
    //     currentEnemyCount--;
    //
    //     // Score points - call points scored event
    //     StaticEventHandler.CallPointsScoredEvent(destroyedEventArgs.points);
    //
    //     if (currentEnemyCount <= 0 && enemiesSpawnedSoFar == enemiesToSpawn)
    //     {
    //         currentRoom.isClearedOfEnemies = true;
    //
    //         // Set game state
    //         if (GameManager.Instance.gameState == GameState.engagingEnemies)
    //         {
    //             GameManager.Instance.gameState = GameState.playingLevel;
    //             GameManager.Instance.previousGameState = GameState.engagingEnemies;
    //         }
    //
    //         else if (GameManager.Instance.gameState == GameState.engagingBoss)
    //         {
    //             GameManager.Instance.gameState = GameState.bossStage;
    //             GameManager.Instance.previousGameState = GameState.engagingBoss;
    //         }
    //
    //         // unlock doors
    //         currentRoom.instantiatedRoom.UnlockDoors(Settings.doorUnlockDelay);
    //
    //         // Update music for room
    //         MusicManager.Instance.PlayMusic(currentRoom.ambientMusic, 0.2f, 2f);
    //
    //         // Trigger room enemies defeated event
    //         StaticEventHandler.CallRoomEnemiesDefeatedEvent(currentRoom);
    //     }
    // }

}