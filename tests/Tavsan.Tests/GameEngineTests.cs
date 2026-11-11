using Tavsan.Core.Domain;
using Tavsan.Core.Services;

namespace Tavsan.Tests;

public class GameEngineTests
{
    [Fact]
    public void Rabbit_starts_top_left_facing_south()
    {
        var engine = new GameEngine(new Board(8));

        Assert.Equal(new Position(0, 0), engine.Position);
        Assert.Equal(Direction.South, engine.Facing);
        Assert.Equal(GameStatus.InProgress, engine.Status);
    }

    [Fact]
    public void Forward_moves_down_the_board_and_back_moves_up()
    {
        var engine = new GameEngine(new Board(8));

        engine.Execute(Command.Forward);
        engine.Execute(Command.Forward);
        Assert.Equal(new Position(0, 2), engine.Position);

        engine.Execute(Command.Back);
        Assert.Equal(new Position(0, 1), engine.Position);
    }

    [Fact]
    public void Left_and_right_are_relative_to_the_rabbit_facing_south()
    {
        var engine = new GameEngine(new Board(8));
        engine.Execute(Command.Forward);

        engine.Execute(Command.Left);
        Assert.Equal(new Position(1, 1), engine.Position);

        engine.Execute(Command.Right);
        Assert.Equal(new Position(0, 1), engine.Position);
    }

    [Theory]
    [InlineData(Command.Back)]
    [InlineData(Command.Right)]
    [InlineData(Command.Jump)]
    public void Leaving_the_forest_is_ignored(Command command)
    {
        var engine = new GameEngine(new Board(4));
        if (command == Command.Jump)
        {
            engine.Execute(Command.Forward);
            engine.Execute(Command.Forward);
        }

        var before = engine.Position;
        var step = engine.Execute(command);

        Assert.Equal(StepOutcome.BlockedByBoundary, step.Outcome);
        Assert.Equal(before, engine.Position);
        Assert.Equal(GameStatus.InProgress, engine.Status);
    }

    [Theory]
    [InlineData(ObstacleType.Wolf)]
    [InlineData(ObstacleType.Fox)]
    public void Walking_into_a_predator_kills_the_rabbit(ObstacleType predator)
    {
        var engine = new GameEngine(TestBoards.With(8, (0, 1, predator)));

        var step = engine.Execute(Command.Forward);

        Assert.Equal(StepOutcome.Died, step.Outcome);
        Assert.Equal(predator, step.Obstacle);
        Assert.Equal(GameStatus.Dead, engine.Status);
        Assert.Throws<InvalidOperationException>(() => engine.Execute(Command.Forward));
    }

    [Theory]
    [InlineData(ObstacleType.Wire)]
    [InlineData(ObstacleType.Fence)]
    public void Walking_into_wire_or_fence_blocks_without_dying(ObstacleType obstacle)
    {
        var engine = new GameEngine(TestBoards.With(8, (0, 1, obstacle)));

        var step = engine.Execute(Command.Forward);

        Assert.Equal(StepOutcome.BlockedByObstacle, step.Outcome);
        Assert.Equal(new Position(0, 0), engine.Position);
        Assert.Equal(GameStatus.InProgress, engine.Status);
    }

    [Fact]
    public void Crouch_passes_under_wire_and_rabbit_ends_on_that_cell()
    {
        var engine = new GameEngine(TestBoards.With(8, (0, 1, ObstacleType.Wire)));

        var step = engine.Execute(Command.Crouch);

        Assert.Equal(StepOutcome.Moved, step.Outcome);
        Assert.Equal(new Position(0, 1), engine.Position);
    }

    [Fact]
    public void Crouch_cannot_pass_a_fence()
    {
        var engine = new GameEngine(TestBoards.With(8, (0, 1, ObstacleType.Fence)));

        var step = engine.Execute(Command.Crouch);

        Assert.Equal(StepOutcome.BlockedByObstacle, step.Outcome);
        Assert.Equal(new Position(0, 0), engine.Position);
    }

    [Fact]
    public void Crouch_into_a_predator_kills()
    {
        var engine = new GameEngine(TestBoards.With(8, (0, 1, ObstacleType.Wolf)));

        Assert.Equal(StepOutcome.Died, engine.Execute(Command.Crouch).Outcome);
    }

    [Fact]
    public void Jump_clears_a_fence_and_lands_two_cells_ahead()
    {
        var engine = new GameEngine(TestBoards.With(8, (0, 1, ObstacleType.Fence)));

        var step = engine.Execute(Command.Jump);

        Assert.Equal(StepOutcome.Moved, step.Outcome);
        Assert.Equal(new Position(0, 2), engine.Position);
    }

    [Fact]
    public void Jump_moves_two_cells_even_without_a_fence()
    {
        var engine = new GameEngine(new Board(8));

        engine.Execute(Command.Jump);

        Assert.Equal(new Position(0, 2), engine.Position);
    }

    [Fact]
    public void Jump_over_a_predator_is_a_collision()
    {
        var engine = new GameEngine(TestBoards.With(8, (0, 1, ObstacleType.Fox)));

        var step = engine.Execute(Command.Jump);

        Assert.Equal(StepOutcome.Died, step.Outcome);
        Assert.Equal(GameStatus.Dead, engine.Status);
    }

    [Fact]
    public void Jump_landing_on_a_predator_kills()
    {
        var engine = new GameEngine(TestBoards.With(8, (0, 2, ObstacleType.Wolf)));

        Assert.Equal(StepOutcome.Died, engine.Execute(Command.Jump).Outcome);
    }

    [Theory]
    [InlineData(ObstacleType.Wire)]
    [InlineData(ObstacleType.Fence)]
    public void Jump_landing_on_wire_or_fence_is_blocked(ObstacleType obstacle)
    {
        var engine = new GameEngine(TestBoards.With(8, (0, 2, obstacle)));

        var step = engine.Execute(Command.Jump);

        Assert.Equal(StepOutcome.BlockedByObstacle, step.Outcome);
        Assert.Equal(new Position(0, 0), engine.Position);
    }

    [Fact]
    public void Reaching_the_hole_wins_and_remaining_commands_are_skipped()
    {
        var engine = new GameEngine(new Board(4));
        var commands = Enumerable.Repeat(Command.Forward, 3)
            .Concat(Enumerable.Repeat(Command.Left, 3))
            .Concat(new[] { Command.Back, Command.Back });

        var result = engine.Run(commands);

        Assert.Equal(GameStatus.Won, result.Status);
        Assert.Equal(new Position(3, 3), result.FinalPosition);
        Assert.Equal(6, result.Steps.Count);
        Assert.Equal(2, result.SkippedCommands);
        Assert.Equal(StepOutcome.ReachedHole, result.Steps[^1].Outcome);
    }

    [Fact]
    public void Run_stops_applying_commands_after_death()
    {
        var engine = new GameEngine(TestBoards.With(8, (0, 2, ObstacleType.Wolf)));

        var result = engine.Run(new[] { Command.Forward, Command.Forward, Command.Forward, Command.Left });

        Assert.Equal(GameStatus.Dead, result.Status);
        Assert.Equal(new Position(0, 2), result.FinalPosition);
        Assert.Equal(2, result.Steps.Count);
        Assert.Equal(2, result.SkippedCommands);
    }

    [Fact]
    public void Sample_scenario_is_valid_on_an_empty_forest()
    {
        ScenarioParser.TryParse("N,N,L,J,N,N,İ,P,J", out var commands, out _);
        var engine = new GameEngine(new Board(8));

        var result = engine.Run(commands);

        Assert.Equal(GameStatus.InProgress, result.Status);
        Assert.Equal(new Position(1, 6), result.FinalPosition);
        Assert.Equal(9, result.Steps.Count);
    }
}
