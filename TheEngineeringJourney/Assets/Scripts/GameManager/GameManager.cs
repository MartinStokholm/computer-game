using System.Collections.Generic;
using Unity.VisualScripting;
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


    private Room _currentRoom;
    public Player Player { get; private set; }
    private Room _previousRoom;
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
    }
    
    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
        StaticSceneChangeEvent.OnEnterLevel -= EnterLevelEvent_CallEnterLevelEvent;
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

                // Play first level
                PlayMapLevel(currentMapLevelListIndex);

                _gameState = GameState.PlayingLevel;
                break;
            case GameState.PlayingLevel:
                if (Input.GetKeyDown(KeyCode.Escape))
                    PauseGameMenu();
                    break;
            case GameState.LevelCompleted:
                PlayMapLevel(currentMapLevelListIndex);
                _gameState = GameState.PlayingLevel;
                break;
            case GameState.GamePaused:
                if (Input.GetKeyDown(KeyCode.Escape))
                    PauseGameMenu();
                break;
        }
    }


    private void PlayMapLevel(int MapLevelListIndex)
    {
        if (MapBuilder.Instance.GenerateMap(MapLevelList[MapLevelListIndex]))
        {
            Debug.LogError("Couldn't build Map from specified rooms and node graphs");
        }

        Debug.Log("Got to be fucking kidding me");
        //StaticEventHandler.CallRoomChangedEvent(CurrentRoom);
    }
    

    private void EnterLevelEvent_CallEnterLevelEvent(SceneChangeArgs args)
    {
        currentMapLevelListIndex = args.Level;
        _gameState = GameState.LevelCompleted;
    }
    
    /// <summary>
    /// Pause game menu - also called from resume game button on pause menu
    /// </summary>
    public void PauseGameMenu()
    {
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

