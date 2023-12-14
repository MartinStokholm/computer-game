using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    #region Header MAP LEVELS

    [Space(10)]
    [Header("MAP LEVELS")]

    #endregion Header MAP LEVELS

    #region Tooltip

    [Tooltip("Populate with pause menu game object in hierarchy")]

    #endregion Tooltip

    [SerializeField] private GameObject pauseMenu;
    
    #region Tooltip

    [Tooltip("Populate with the map level scriptable objects")]

    #endregion Tooltip

    [SerializeField] private List<MapLevelSO> MapLevelList;

    #region Tooltip
    
    [Tooltip("Populate with the starting MAP level for testing , first level = 0")]

    #endregion Tooltip

    [SerializeField] public int currentMapLevelListIndex = 0;

    public PlayerEvents PlayerEvents;
    public GoldEvents GoldEvents;
    public MiscEvents MiscEvents;
    public QuestEvents QuestEvents;
    public InputEvents InputEvents;
    
    
    public Player Player { get; private set; }
    private Room _currentRoom;
    private Room _previousRoom;
    private InstantiatedRoom _bossRoom;
    private PlayerDetailsSO _playerDetails;
    private GameState _gameState;
    private GameState _previousGameState;

    public GameState GameState
    {
        get => _gameState;
        set
        {
            _previousGameState = _gameState;
            _gameState = value;
        }
    }
    
    public Room CurrentRoom
    {
        get => _currentRoom;
        set
        {
            _previousRoom = _currentRoom;
            _currentRoom = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _playerDetails = GameResources.Instance.CurrentPlayer.PlayerDetails;
        InstantiatePlayer();
        PlayerEvents = new PlayerEvents();
        GoldEvents = new GoldEvents();
        MiscEvents = new MiscEvents();
        QuestEvents = new QuestEvents();
        InputEvents = new InputEvents();
    }
    
    /// <summary>
    /// Create player in scene at position
    /// </summary>
    private void InstantiatePlayer()
    {
        var playerGameObject = Instantiate(_playerDetails.PlayerPrefab);

        Player = playerGameObject.GetComponent<Player>();

        Player.Initialize(_playerDetails);
    }
    
    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
        StaticSceneChangeEvent.OnEnterLevel += EnterLevelEvent_CallEnterLevelEvent;
        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;
        StaticTeleportPositionEvent.OnTeleportEvent += StaticTeleportHandler_OnTeleportPosition;
        Player.DestroyedEvent.OnDestroyed += Player_OnDestroyed;
    }
    
    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
        StaticSceneChangeEvent.OnEnterLevel -= EnterLevelEvent_CallEnterLevelEvent;
        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;
        StaticTeleportPositionEvent.OnTeleportEvent -= StaticTeleportHandler_OnTeleportPosition;
        Player.DestroyedEvent.OnDestroyed -= Player_OnDestroyed;
    }


    // Start is called before the first frame update
    private void Start()
    {
        _gameState = GameState.GameStarted;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleGameState();

        // For testing
        if (Input.GetKeyDown(KeyCode.R))
        {
            _gameState = GameState.GameStarted;
        }
    }

    /// <summary>
    /// Handle game state
    /// </summary>
    private void HandleGameState()
    {
        // Handle game state
        switch (_gameState)
        {
            case GameState.GameStarted:
                PlayMapLevel(currentMapLevelListIndex);
                _gameState = GameState.PlayingLevel;
                break;
            case GameState.PlayingLevel:
                Pause();
                break;
            case GameState.EnterLevel: 
                PlayMapLevel(currentMapLevelListIndex);
                _gameState = GameState.PlayingLevel;
                break;
            case GameState.LevelCompleted:
                PlayMapLevel(0);
                _gameState = GameState.PlayingLevel;
                break;
            case GameState.LevelLost:
                PlayMapLevel(0);
                _gameState = GameState.PlayingLevel;
                break;
            case GameState.GamePaused:
                Pause();
                break;
            case GameState.EngagingEnemies:
                Pause();
                break;
            case GameState.BossStage:
                Pause();
                break;
            case GameState.EngagingBoss:
                Pause();
                break;
            case GameState.GameWon:
                break;
            case GameState.MapOverviewMap:
                break;
            case GameState.RestartGame:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    private void PlayMapLevel(int MapLevelListIndex)
    {
        var statusGenerateMap = MapBuilder.Instance.GenerateMap(MapLevelList[MapLevelListIndex]);
        Debug.Log($"Status for generate map: {statusGenerateMap}");
        
        switch (statusGenerateMap)
        {
            case Build.Failed:
                Debug.LogError("Couldn't build Map from specified rooms and node graphs");
                return;
            case Build.Success:
                StaticEventHandler.CallRoomChangedEvent(CurrentRoom);
                Player.gameObject.transform.position = _currentRoom.GetRoomAsVector3();
                Player.gameObject.transform.position = PlayerUtils.GetSpawnPosition(Player.gameObject.transform.position);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    /// <summary>
    /// Handle player destroyed event
    /// </summary>
    private void Player_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        var enemyArray = FindObjectsOfType<Enemy>();
        foreach (var enemy in enemyArray)
        {
            enemy.gameObject.SetActive(false);
        }
        _gameState = GameState.LevelLost;
    }

    private void EnterLevelEvent_CallEnterLevelEvent(SceneChangeArgs args)
    {
        currentMapLevelListIndex = args.Level;
        _gameState = GameState.EnterLevel;
    }
    
    /// <summary>
    /// Handle room enemies defeated event
    /// </summary>
    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs)
    {
        RoomEnemiesDefeated();
    }
    
    /// <summary>
    /// Room enemies defated - test if all dungeon rooms have been cleared of enemies - if so load
    /// next first game level
    /// </summary>
    private void RoomEnemiesDefeated()
    {
        var isDungeonClearOfRegularEnemies = true;
        _bossRoom = null;

        // Loop through all dungeon rooms to see if cleared of enemies
        foreach (var keyValuePair in MapBuilder.Instance.MapBuilderRoomDictionary)
        {
            // skip boss room for time being
            if (keyValuePair.Value.RoomNodeType.isBossRoom)
            {
                _bossRoom = keyValuePair.Value.InstantiatedRoom;
                continue;
            }

            // check if other rooms have been cleared of enemies
            if (keyValuePair.Value.IsClearedOfEnemies) continue;
            isDungeonClearOfRegularEnemies = false;
            break;
        }

        switch (isDungeonClearOfRegularEnemies)
        {
            case true when _bossRoom is null:
            case true when _bossRoom.Room.IsClearedOfEnemies:
                GameState = currentMapLevelListIndex < MapLevelList.Count - 1 
                    ? GameState.LevelCompleted 
                    : GameState.GameWon;
                break;

            case true:
                GameState = GameState.BossStage;
                StartCoroutine(BossStage());
                break;
        }
    }
    
    /// <summary>
    /// Enter boss stage
    /// </summary>
    private IEnumerator BossStage()
    {
        // Activate boss room
        _bossRoom.gameObject.SetActive(true);

        // Unlock boss room
        //_bossRoom.UnlockDoors(0f);

        // Wait 2 seconds
        yield return new WaitForSeconds(2f);

        // Fade in canvas to display text message
        //yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        // Display boss message
        //yield return StartCoroutine(DisplayMessageRoutine("WELL DONE  " + GameResources.Instance.currentPlayer.playerName + "!  YOU'VE SURVIVED ....SO FAR\n\nNOW FIND AND DEFEAT THE BOSS....GOOD LUCK!", Color.white, 5f));

        // Fade out canvas
        //yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));

    }
    
    /// <summary>
    /// Pause game menu - also called from resume game button on pause menu
    /// </summary>
    public void PauseGameMenu()
    {
        Debug.Log($"PauseGameMenu {GameState}");
        if (GameState != GameState.GamePaused)
        {
              pauseMenu.SetActive(true);
            // GetPlayer().playerControl.DisablePlayer();
            
            GameState = GameState.GamePaused;
        }
        else if (GameState == GameState.GamePaused)
        {
              pauseMenu.SetActive(false);
            // GetPlayer().playerControl.EnablePlayer();
            
            GameState = _previousGameState;
        }
    }
    
    /// <summary>
    /// Handle room changed event
    /// </summary>
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        
        CurrentRoom = roomChangedEventArgs.Room;
    }

    private void StaticTeleportHandler_OnTeleportPosition(TeleportPositionArgs teleportPositionArgs)
    {
        Player.transform.position = teleportPositionArgs.TeleportPosition;
    }

    private void Pause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGameMenu();
        }
    }

    /// <summary>
    /// Get the current map level
    /// </summary>
    public MapLevelSO GetCurrentMapLevel() => MapLevelList[currentMapLevelListIndex];

    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        EditorUtilities.ValidateCheckNullValue(this, nameof(pauseMenu), pauseMenu);
        EditorUtilities.ValidateCheckEnumerableValues(this, nameof(MapLevelList), MapLevelList);
    }

#endif

    #endregion Validation

}

