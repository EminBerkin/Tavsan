using Tavsan.Domain.Game.Entities;
using Tavsan.Domain.Game.Models;
using Tavsan.Domain.Game.Enums;

namespace Tavsan.Tests.Game;

internal static class TestBoards
{
    public static Board With(int size,
                             params (int Column, int Row, ObstacleType Type)[] obstacles) =>
        new(size, obstacles.Select(o => new KeyValuePair<Position, ObstacleType>(new Position(o.Column, o.Row), o.Type)));
}
