namespace Tavsan.Domain.Game.Enums;

public enum Direction
{
    North,
    East,
    South,
    West
}

public static class DirectionExtensions
{
    public static (int Dx, int Dy) Delta(this Direction direction) => direction switch
    {
        Direction.North => (0, -1),
        Direction.East => (1, 0),
        Direction.South => (0, 1),
        Direction.West => (-1, 0),
        _ => throw new ArgumentOutOfRangeException(nameof(direction))
    };

    public static Direction TurnRight(this Direction direction) => (Direction)(((int)direction + 1) % 4);

    public static Direction TurnLeft(this Direction direction) => (Direction)(((int)direction + 3) % 4);

    public static Direction Opposite(this Direction direction) => (Direction)(((int)direction + 2) % 4);
}
