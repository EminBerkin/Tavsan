using Tavsan.Application.Game.Operations.Play;
using Tavsan.Infrastructure.Game.Services;
using Tavsan.Application.Game.Services;
using Tavsan.ConsoleApp.Game;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

IObstaclePlacer placer = new RandomObstaclePlacer();

try
{
    do
    {
        Console.WriteLine("=== YILDIZ ===");
        Console.WriteLine();

        var size = ConsoleInput.ReadForestSize();
        var board = placer.CreateBoard(size);

        Console.WriteLine();
        BoardRenderer.Render(board);
        Console.WriteLine();

        var commands = ConsoleInput.ReadScenario();
        var engine = new GameEngine(board);
        var result = engine.Run(commands);

        Console.WriteLine();
        for (var i = 0; i < result.Steps.Count; i++)
        {
            Console.WriteLine(StepFormatter.Format(i + 1, result.Steps[i], board));
        }

        if (result.SkippedCommands > 0)
        {
            Console.WriteLine($"Oyun sona erdiği için {result.SkippedCommands} komut uygulanmadı.");
        }

        Console.WriteLine();
        BoardRenderer.Render(board, result.FinalPosition);
        Console.WriteLine();
        Console.WriteLine(StepFormatter.FormatStatus(result.Status));
        Console.WriteLine();
    }
    while (ConsoleInput.AskPlayAgain());
}
catch (EndOfStreamException)
{
}
