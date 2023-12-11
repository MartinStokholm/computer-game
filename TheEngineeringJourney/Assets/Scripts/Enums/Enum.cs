public enum Orientation
{
    North,
    East,
    South,
    West,
    None
}

public enum GameState
{
    GameStarted,
    EnterLevel,
    PlayingLevel,
    EngagingEnemies,
    BossStage,
    EngagingBoss,
    LevelCompleted,
    GameWon,
    GameLost,
    GamePaused,
    MapOverviewMap,
    RestartGame
}

public enum AimDirection
{
    Up,
    Right,
    Left,
    Down
}

public enum Build
{
    Failed,
    Success
}

public enum RoomOverlapping
{
    Overlapping,
    Contiguous,
    Attempt
}