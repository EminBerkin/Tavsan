namespace Tavsan.Core.Domain;

public readonly record struct Position(int Column, int Row)
{
    public Position Move(Direction direction, int distance = 1)
    {
        var (dx, dy) = direction.Delta();
        return new Position(Column + dx * distance, Row + dy * distance);
    }
}
