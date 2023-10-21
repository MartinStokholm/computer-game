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

    [Tooltip("Populate with the map level scriptable objects")]

    #endregion Tooltip

    [SerializeField] private List<MapLevelSO> MapLevelList;

    #region Tooltip
    
    [Tooltip("Populate with the starting MAP level for testing , first level = 0")]

    #endregion Tooltip

    [SerializeField] private int currentMapLevelListIndex = 0;

   
    
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

        InstantiatePlayer(_playerDetails);
    }
    
    /// <summary>
    /// Create player in scene at position
    /// </summary>
    private void InstantiatePlayer(PlayerDetailsSO playerDetails)
    {
        var playerGameObject = Instantiate(playerDetails.PlayerPrefab);
        
        Player = playerGameObject.GetComponent<Player>();

        Player.Initialize(playerDetails);

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
        }
    }


    private void PlayMapLevel(int MapLevelListIndex)
    {
        if (MapBuilder.Instance.GenerateMap(MapLevelList[MapLevelListIndex])) return;
        
        Debug.LogError("Couldn't build Map from specified rooms and node graphs");
    }

    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(MapLevelList), MapLevelList);
    }

#endif

    #endregion Validation

}

