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
    Attempt,
    AttemptFailed
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

public enum RebuildAttempt
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

public enum Building
{
    Failed,
    Success,
    Attempt,
    AttemptFailed
}

public enum RoomOverlaps
{
    Overlapping,
    Contiguous,
    Attempt
}