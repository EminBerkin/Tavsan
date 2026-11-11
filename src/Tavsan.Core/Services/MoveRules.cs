using Tavsan.Core.Domain;

namespace Tavsan.Core.Services;

public static class MoveRules
{
    private const int JumpDistance = 2;

    public static StepResult Resolve(Board board, Position from, Command command, Direction facing)
    {
        var direction = command.ToDirection(facing);

        return command == Command.Jump
            ? ResolveJump(board, from, direction)
            : ResolveStep(board, from, command, direction);
    }

    private static StepResult ResolveStep(Board board, Position from, Command command, Direction direction)
    {
        var target = from.Move(direction);

        if (!board.Contains(target))
        {
            return new StepResult(command, from, from, StepOutcome.BlockedByBoundary);
        }

        var obstacle = board.ObstacleAt(target);

        if (obstacle is { } found)
        {
            if (found.IsLethal())
            {
                return new StepResult(command, from, target, StepOutcome.Died, found);
            }

            var canPassUnderWire = found == ObstacleType.Wire && command == Command.Crouch;
            if (!canPassUnderWire)
            {
                return new StepResult(command, from, from, StepOutcome.BlockedByObstacle, found);
            }
        }

        return Arrive(board, command, from, target, obstacle);
    }

    private static StepResult ResolveJump(Board board, Position from, Direction direction)
    {
        var middle = from.Move(direction);
        var landing = from.Move(direction, JumpDistance);

        if (!board.Contains(middle) || !board.Contains(landing))
        {
            return new StepResult(Command.Jump, from, from, StepOutcome.BlockedByBoundary);
        }

        if (board.ObstacleAt(middle) is { } passed && passed.IsLethal())
        {
            return new StepResult(Command.Jump, from, middle, StepOutcome.Died, passed);
        }

        var obstacle = board.ObstacleAt(landing);

        if (obstacle is { } found)
        {
            return found.IsLethal()
                ? new StepResult(Command.Jump, from, landing, StepOutcome.Died, found)
                : new StepResult(Command.Jump, from, from, StepOutcome.BlockedByObstacle, found);
        }

        return Arrive(board, Command.Jump, from, landing, null);
    }

    private static StepResult Arrive(Board board, Command command, Position from, Position to, ObstacleType? obstacle)
    {
        var outcome = to == board.Hole ? StepOutcome.ReachedHole : StepOutcome.Moved;
        return new StepResult(command, from, to, outcome, obstacle);
    }
}
