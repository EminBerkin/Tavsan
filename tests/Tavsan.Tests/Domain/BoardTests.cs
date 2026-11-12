using Tavsan.Domain.Game.Entities;
using Tavsan.Domain.Game.Models;
using Tavsan.Domain.Game.Enums;
using Tavsan.Tests.Game;

namespace Tavsan.Tests.Domain;

public class BoardTests
{
    [Theory]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(16)]
    public void Accepts_supported_sizes(int size)
    {
        var board = new Board(size);

        Assert.Equal(new Position(0, 0), board.Start);
        Assert.Equal(new Position(size - 1, size - 1), board.Hole);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(32)]
    [InlineData(-8)]
    public void Rejects_unsupported_sizes(int size)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Board(size));
    }

    [Fact]
    public void Notation_uses_letters_for_columns_and_descending_numbers_for_rows()
    {
        var board = new Board(8);

        Assert.Equal("A8", board.ToNotation(board.Start));
        Assert.Equal("H1", board.ToNotation(board.Hole));
        Assert.Equal("D5", board.ToNotation(new Position(3, 3)));
    }

    [Fact]
    public void Rejects_two_obstacles_on_same_cell()
    {
        var obstacles = new[]
        {
            new KeyValuePair<Position, ObstacleType>(new Position(1, 1), ObstacleType.Wolf),
            new KeyValuePair<Position, ObstacleType>(new Position(1, 1), ObstacleType.Fox)
        };

        Assert.Throws<ArgumentException>(() => new Board(8, obstacles));
    }

    [Fact]
    public void Rejects_obstacles_on_start_hole_or_outside()
    {
        Assert.Throws<ArgumentException>(() => TestBoards.With(8, (0, 0, ObstacleType.Fence)));
        Assert.Throws<ArgumentException>(() => TestBoards.With(8, (7, 7, ObstacleType.Fence)));
        Assert.Throws<ArgumentException>(() => TestBoards.With(8, (8, 3, ObstacleType.Fence)));
    }

    [Fact]
    public void Rejects_more_than_four_obstacles_of_one_type()
    {
        Assert.Throws<ArgumentException>(() => TestBoards.With(
            8,
            (1, 0, ObstacleType.Wolf),
            (2, 0, ObstacleType.Wolf),
            (3, 0, ObstacleType.Wolf),
            (4, 0, ObstacleType.Wolf),
            (5, 0, ObstacleType.Wolf)));
    }

    [Fact]
    public void Allows_four_obstacles_of_each_type()
    {
        var obstacles = new List<(int, int, ObstacleType)>();
        var column = 0;
        foreach (var type in Enum.GetValues<ObstacleType>())
        {
            for (var i = 0; i < 4; i++)
            {
                obstacles.Add((++column % 8, 1 + column / 8, type));
            }
        }

        var board = TestBoards.With(8, obstacles.ToArray());

        Assert.All(Enum.GetValues<ObstacleType>(), type => Assert.Equal(4, board.Count(type)));
    }
}
