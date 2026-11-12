using Tavsan.Domain.Game.Entities;
using Tavsan.Domain.Game.Enums;
using Tavsan.Domain.Game.Models;
using Tavsan.Domain.Game.Rules;

namespace Tavsan.Application.Game.Operations.Solve;

public static class BoardSolver
{
    private static readonly Command[] AllCommands = Enum.GetValues<Command>();

    public static bool IsSolvable(Board board,
                                  Direction facing = Direction.South)
    {
        var visited = new HashSet<Position> { board.Start };
        var queue = new Queue<Position>();
        queue.Enqueue(board.Start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            foreach (var command in AllCommands)
            {
                var step = MoveRules.Resolve(board, current, command, facing);

                switch (step.Outcome)
                {
                    case StepOutcome.ReachedHole:
                        return true;
                    case StepOutcome.Moved when visited.Add(step.To):
                        queue.Enqueue(step.To);
                        break;
                }
            }
        }

        return false;
    }
}
