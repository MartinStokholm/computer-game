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
    private RoomEnemySpawnParameters _roomEnemySpawnParameters;
    private Room _currentRoom;
    
    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        _currentRoom = roomChangedEventArgs.Room;
        _enemiesToSpawn = _currentRoom.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentMapLevel());
        
        if (IsCorridorOrEntrance()) return;
        
        if (_currentRoom.IsClearedOfEnemies) return;
        
        _roomEnemySpawnParameters = _currentRoom.GetRoomEnemySpawnParameters(GameManager.Instance.GetCurrentMapLevel());
       
        _enemyMaxConcurrentSpawnNumber = _roomEnemySpawnParameters.MaxTotalEnemiesToSpawn;

        if (CheckIfRoomsClearedNow()) return;
        
        SpawnEnemies();
    }
    
    /// <summary>
    /// Spawn the enemies
    /// </summary>
    private void SpawnEnemies()
    {
        GameManager.Instance.GameState = GameState.EngagingEnemies;
        StartCoroutine(SpawnEnemiesRoutine());
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        var grid = _currentRoom.InstantiatedRoom.Grid;

        // Get an instance of the helper class used to select a random enemy
        var selectRandomEnemy = new RandomSpawnableObject<EnemyDetailsSO>(_currentRoom.EnemiesByLevels);
        
        if (IsSpawnPositionArrayLessThanZero()) yield break;
        
        for (var i = 0; i < _enemiesToSpawn; i++)
        {
            //wait until current enemy count is less than max concurrent enemies
            while (_currentEnemyCount >= _enemyMaxConcurrentSpawnNumber)
            {
                Debug.Log($"Waiting to spawn, because there is {_currentEnemyCount} and there can only be {_enemyMaxConcurrentSpawnNumber}");
                yield return null;
            }

            var cellPosition = (Vector3Int)_currentRoom.SpawnPositionArray[Random.Range(0, _currentRoom.SpawnPositionArray.Length)];
            
            CreateEnemy(selectRandomEnemy.GetItem(), grid.CellToWorld(cellPosition));

            yield return new WaitForSeconds(_roomEnemySpawnParameters.GetEnemySpawnInterval());
        }
    }
    
    /// <summary>
    /// Create an enemy in the specified position and subscribe to it until it is deleted
    /// </summary>
    private void CreateEnemy(EnemyDetailsSO enemyDetails, Vector3 position)
    {
        _enemiesSpawnedSoFar++;
        _currentEnemyCount++;
        
        var mapLevel = GameManager.Instance.GetCurrentMapLevel();
        
        var enemy = Instantiate(enemyDetails.enemyPrefab, position, Quaternion.identity, transform);
        enemy.GetComponent<Enemy>().EnemyInitialization(enemyDetails, _enemiesSpawnedSoFar, mapLevel);

        enemy.GetComponent<DestroyedEvent>().OnDestroyed += Enemy_OnDestroyed;
    }
    

    /// <summary>
    /// Process enemy destroyed
    /// </summary>
    private void Enemy_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        destroyedEvent.OnDestroyed -= Enemy_OnDestroyed;
        
        --_currentEnemyCount;
    
        // Score points - call points scored event
        //StaticEventHandler.CallPointsScoredEvent(destroyedEventArgs.points);

        if (IsRoomCleared()) return;
        
        _currentRoom.IsClearedOfEnemies = true;

        UpdateGameState();

        // unlock doors
        //_currentRoom.InstantiatedRoom.UnlockDoors(Settings.doorUnlockDelay);
    
        // Update music for room
        //MusicManager.Instance.PlayMusic(_currentRoom.ambientMusic, 0.2f, 2f);
    
        // Trigger room enemies defeated event
        StaticEventHandler.CallRoomEnemiesDefeatedEvent(_currentRoom);
    }
    

    private void UpdateGameState()
    {
        switch (GameManager.Instance.GameState)
        {
            case GameState.EngagingEnemies:
                GameManager.Instance.GameState = GameState.PlayingLevel;
                break;
            case GameState.EngagingBoss:
                GameManager.Instance.GameState = GameState.BossStage;
                break;
        }
    }
    private bool IsRoomCleared() =>
        _currentEnemyCount > 0 || _enemiesSpawnedSoFar != _enemiesToSpawn;
    
    private bool IsCorridorOrEntrance() => 
        _currentRoom.RoomNodeType.isCorridorEW || 
        _currentRoom.RoomNodeType.isCorridorNS || 
        _currentRoom.RoomNodeType.isEntrance;
    
    private bool IsSpawnPositionArrayLessThanZero() => _currentRoom.SpawnPositionArray.Length <= 0;
    
    private bool CheckIfRoomsClearedNow()
    {
        if (_enemiesToSpawn != 0) return false;
        
        // Mark the room as cleared
        _currentRoom.IsClearedOfEnemies = true;

        return true;
    }

}