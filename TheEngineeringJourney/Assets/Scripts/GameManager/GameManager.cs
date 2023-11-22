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
    [HideInInspector] public GameState gameState;
    
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
        StaticEventHandler.OnEnterLevel += EnterLevelEvent_CallEnterLevelEvent;
    }
    
    private void OnDisable()
    {
        StaticEventHandler.OnEnterLevel -= EnterLevelEvent_CallEnterLevelEvent;
    }


    // Start is called before the first frame update
    private void Start()
    {
        gameState = GameState.GameStarted;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleGameState();

        // For testing
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameState = GameState.GameStarted;
        }

    }

    /// <summary>
    /// Handle game state
    /// </summary>
    private void HandleGameState()
    {
        // Handle game state
        switch (gameState)
        {
            case GameState.GameStarted:

                // Play first level
                PlayMapLevel(currentMapLevelListIndex);

                gameState = GameState.PlayingLevel;

                break;
            case GameState.LevelCompleted:
                PlayMapLevel(currentMapLevelListIndex);
                gameState = GameState.PlayingLevel;
                break;
        }
    }


    private void PlayMapLevel(int MapLevelListIndex)
    {
        if (MapBuilder.Instance.GenerateMap(MapLevelList[MapLevelListIndex])) return;
        
        Debug.LogError("Couldn't build Map from specified rooms and node graphs");
    }
    

    private void EnterLevelEvent_CallEnterLevelEvent(SceneChangeArgs args)
    {
        currentMapLevelListIndex = args.Level;
        gameState = GameState.LevelCompleted;
    }

    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        EditorUtilities.ValidateCheckEnumerableValues(this, nameof(MapLevelList), MapLevelList);
    }

#endif

    #endregion Validation

}

