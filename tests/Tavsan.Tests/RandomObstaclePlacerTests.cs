using Tavsan.Core.Domain;
using Tavsan.Core.Services;

namespace Tavsan.Tests;

public class RandomObstaclePlacerTests
{
    [Theory]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(16)]
    public void Respects_all_placement_rules_for_many_random_forests(int size)
    {
        var placer = new RandomObstaclePlacer(new Random(1234));

        for (var i = 0; i < 300; i++)
        {
            var board = placer.CreateBoard(size);

            Assert.Equal(size, board.Size);
            Assert.All(Enum.GetValues<ObstacleType>(), type => Assert.InRange(board.Count(type), 0, 4));
            Assert.DoesNotContain(board.Start, board.Obstacles.Keys);
            Assert.DoesNotContain(board.Hole, board.Obstacles.Keys);
            Assert.All(board.Obstacles.Keys, position => Assert.True(board.Contains(position)));
        }
    }

    [Fact]
    public void Same_seed_produces_same_forest()
    {
        var first = new RandomObstaclePlacer(new Random(7)).CreateBoard(8);
        var second = new RandomObstaclePlacer(new Random(7)).CreateBoard(8);

        Assert.Equal(
            first.Obstacles.OrderBy(o => o.Key.Row).ThenBy(o => o.Key.Column),
            second.Obstacles.OrderBy(o => o.Key.Row).ThenBy(o => o.Key.Column));
    }

    [Theory]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(16)]
    public void Generated_forests_always_have_a_way_to_the_hole(int size)
    {
        var placer = new RandomObstaclePlacer(new Random(99));

        for (var i = 0; i < 200; i++)
        {
            Assert.True(BoardSolver.IsSolvable(placer.CreateBoard(size)));
        }
    }

    [Fact]
    public void Placer_uses_the_available_obstacle_variety()
    {
        var placer = new RandomObstaclePlacer(new Random(5));
        var seen = new HashSet<ObstacleType>();

        for (var i = 0; i < 50; i++)
        {
            foreach (var type in placer.CreateBoard(8).Obstacles.Values)
            {
                seen.Add(type);
            }
        }

        Assert.Equal(Enum.GetValues<ObstacleType>().Length, seen.Count);
    }
}

public class BoardSolverTests
{
    [Fact]
    public void Empty_board_is_solvable()
    {
        Assert.True(BoardSolver.IsSolvable(new Board(8)));
    }

    [Fact]
    public void Board_walled_off_by_fences_is_not_solvable()
    {
        var board = TestBoards.With(
            4,
            (2, 3, ObstacleType.Fence),
            (3, 2, ObstacleType.Fence),
            (3, 1, ObstacleType.Fence));

        Assert.False(BoardSolver.IsSolvable(board));
    }

    [Fact]
    public void Wire_does_not_block_because_rabbit_can_crouch_under_it()
    {
        var board = TestBoards.With(
            4,
            (2, 3, ObstacleType.Wire),
            (3, 2, ObstacleType.Wire));

        Assert.True(BoardSolver.IsSolvable(board));
    }
}
