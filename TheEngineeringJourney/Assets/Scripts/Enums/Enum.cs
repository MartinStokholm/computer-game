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
    UpRight,
    UpLeft,
    Right,
    Left,
    Down
}

public enum Build
{
    Failed,
    Success,
    AttemptBuilding,
    AttemptFailed,
    ReAttemptFailedCreateNodeGraph,
    MaxAttemptFailed,
    CreateNodeGraph
}

public enum Attempt
{
    Zero,
    First,
    Second,
    Third,
    Fourth,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten
}

public enum RoomOverlapping
{
    Overlapping,
    Contiguous,
    Attempt
}