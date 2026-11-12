using Tavsan.Application.Game.Operations.Solve;
using Tavsan.Application.Game.Services;
using Tavsan.Domain.Game.Entities;
using Tavsan.Domain.Game.Models;
using Tavsan.Domain.Game.Enums;

namespace Tavsan.Infrastructure.Game.Services;

public sealed class RandomObstaclePlacer : IObstaclePlacer
{
    private const int MaxAttempts = 500;

    private static readonly ObstacleType[] ObstacleTypes = Enum.GetValues<ObstacleType>();

    private readonly Random _random;
    private readonly bool _requireSolvable;

    public RandomObstaclePlacer(Random? random = null, bool requireSolvable = true)
    {
        _random = random ?? Random.Shared;
        _requireSolvable = requireSolvable;
    }

    public Board CreateBoard(int size)
    {
        for (var attempt = 0; attempt < MaxAttempts; attempt++)
        {
            var board = new Board(size, Scatter(size));

            if (!_requireSolvable || BoardSolver.IsSolvable(board))
            {
                return board;
            }
        }

        return new Board(size);
    }

    private Dictionary<Position, ObstacleType> Scatter(int size)
    {
        var reference = new Board(size);
        var freeCells = new List<Position>();

        for (var row = 0; row < size; row++)
        {
            for (var column = 0; column < size; column++)
            {
                var cell = new Position(column, row);
                if (cell != reference.Start && cell != reference.Hole)
                {
                    freeCells.Add(cell);
                }
            }
        }

        Shuffle(freeCells);

        var placed = new Dictionary<Position, ObstacleType>();
        var next = 0;

        foreach (var type in ObstacleTypes)
        {
            var count = Math.Min(_random.Next(Board.MaxObstaclesPerType + 1), freeCells.Count - next);

            for (var i = 0; i < count; i++)
            {
                placed.Add(freeCells[next++], type);
            }
        }

        return placed;
    }

    private void Shuffle(IList<Position> cells)
    {
        for (var i = cells.Count - 1; i > 0; i--)
        {
            var j = _random.Next(i + 1);
            (cells[i], cells[j]) = (cells[j], cells[i]);
        }
    }
}
