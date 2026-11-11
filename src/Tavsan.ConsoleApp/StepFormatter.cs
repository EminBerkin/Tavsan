using Tavsan.Core.Domain;

namespace Tavsan.ConsoleApp;

public static class StepFormatter
{
    public static string Format(int index, StepResult step, Board board)
    {
        var from = board.ToNotation(step.From);
        var to = board.ToNotation(step.To);
        var action = CommandName(step.Command);

        var detail = step.Outcome switch
        {
            StepOutcome.Moved => $"{from} -> {to}",
            StepOutcome.ReachedHole => $"{from} -> {to} | Tavşan deliğine ulaştı.",
            StepOutcome.BlockedByBoundary => $"{from} | Orman sınırı, yerinde kaldı.",
            StepOutcome.BlockedByObstacle => $"{from} | {ObstacleName(step.Obstacle)} engelledi, yerinde kaldı.",
            StepOutcome.Died => $"{from} -> {to} | {ObstacleName(step.Obstacle)} ile karşılaştı, tavşan öldü.",
            _ => string.Empty
        };

        return $"{index,3}. {action,-6} {detail}";
    }

    public static string FormatStatus(GameStatus status) => status switch
    {
        GameStatus.Won => "Tebrikler! Yıldız tavşan deliğine ulaştı.",
        GameStatus.Dead => "Yıldız öldü. Oyun bitti.",
        _ => "Yıldız henüz tavşan deliğine ulaşamadı."
    };

    private static string CommandName(Command command) => command switch
    {
        Command.Forward => "İleri",
        Command.Back => "Geri",
        Command.Right => "Sağ",
        Command.Left => "Sol",
        Command.Jump => "Zıpla",
        Command.Crouch => "Eğil",
        _ => command.ToString()
    };

    private static string ObstacleName(ObstacleType? obstacle) => obstacle switch
    {
        ObstacleType.Wolf => "Kurt",
        ObstacleType.Fox => "Tilki",
        ObstacleType.Wire => "Dikenli tel",
        ObstacleType.Fence => "Çit",
        _ => "Engel"
    };
}
