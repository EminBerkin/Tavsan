using Tavsan.Application.Game.Operations.Play.Models;
using Tavsan.Domain.Game.Entities;
using Tavsan.Domain.Game.Models;
using Tavsan.Domain.Game.Enums;
using Tavsan.Domain.Game.Rules;

namespace Tavsan.Application.Game.Operations.Play;

public sealed class GameEngine
{
    public GameEngine(Board board)
    {
        Board = board ?? throw new ArgumentNullException(nameof(board));
        Position = board.Start;
    }

    public Board Board { get; }

    public Direction Facing => Direction.South;

    public Position Position { get; private set; }

    public GameStatus Status { get; private set; } = GameStatus.InProgress;

    public StepResult Execute(Command command)
    {
        if (Status != GameStatus.InProgress)
        {
            throw new InvalidOperationException("Oyun sona erdi, yeni komut uygulanamaz.");
        }

        var step = MoveRules.Resolve(Board, Position, command, Facing);

        switch (step.Outcome)
        {
            case StepOutcome.Moved:
                Position = step.To;
                break;
            case StepOutcome.ReachedHole:
                Position = step.To;
                Status = GameStatus.Won;
                break;
            case StepOutcome.Died:
                Position = step.To;
                Status = GameStatus.Dead;
                break;
        }

        return step;
    }

    public GameResult Run(IEnumerable<Command> commands)
    {
        var steps = new List<StepResult>();
        var skipped = 0;

        foreach (var command in commands)
        {
            if (Status != GameStatus.InProgress)
            {
                skipped++;
                continue;
            }

            steps.Add(Execute(command));
        }

        return new GameResult(Status, Position, steps, skipped);
    }
}
