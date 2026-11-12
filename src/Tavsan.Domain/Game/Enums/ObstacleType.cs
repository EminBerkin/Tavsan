namespace Tavsan.Domain.Game.Enums;

public enum ObstacleType
{
    Wolf,
    Fox,
    Wire,
    Fence
}

public static class ObstacleTypeExtensions
{
    public static bool IsLethal(this ObstacleType obstacle) => obstacle is ObstacleType.Wolf or ObstacleType.Fox;
}
