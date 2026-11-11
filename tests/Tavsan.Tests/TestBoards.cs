using Tavsan.Core.Domain;

namespace Tavsan.Tests;

internal static class TestBoards
{
    public static Board With(int size, params (int Column, int Row, ObstacleType Type)[] obstacles) =>
        new(size, obstacles.Select(o => new KeyValuePair<Position, ObstacleType>(new Position(o.Column, o.Row), o.Type)));
}
