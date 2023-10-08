using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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

    [HideInInspector] public GameState gameState;


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

