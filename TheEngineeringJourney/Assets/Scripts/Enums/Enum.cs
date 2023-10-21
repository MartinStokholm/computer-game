﻿public enum Orientation
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