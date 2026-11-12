using Tavsan.Domain.Game.Enums;

namespace Tavsan.Domain.Game.Models;

public enum StepOutcome
{
    Moved,
    BlockedByObstacle,
    BlockedByBoundary,
    Died,
    ReachedHole
}

public sealed record StepResult(
    Command Command,
    Position From,
    Position To,
    StepOutcome Outcome,
    ObstacleType? Obstacle = null);
