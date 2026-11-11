using Tavsan.Core.Domain;
using Tavsan.Core.Services;

namespace Tavsan.ConsoleApp;

public static class ConsoleInput
{
    public static int ReadForestSize()
    {
        var options = string.Join(", ", Board.ValidSizes);

        while (true)
        {
            Console.Write($"Orman büyüklüğünü giriniz ({options}): ");
            var line = Console.ReadLine();

            if (line is null)
            {
                throw new EndOfStreamException();
            }

            if (int.TryParse(line.Trim(), out var size) && Board.IsValidSize(size))
            {
                return size;
            }

            WriteError($"Geçersiz büyüklük. Lütfen {options} değerlerinden birini giriniz.");
        }
    }

    public static IReadOnlyList<Command> ReadScenario()
    {
        while (true)
        {
            Console.WriteLine("Komutlar: İleri N, Geri P, Sağ R, Sol L, Zıpla J, Eğil İ");
            Console.Write("Senaryo (örn. N,N,L,J,N,N,İ,P,J): ");
            var line = Console.ReadLine();

            if (line is null)
            {
                throw new EndOfStreamException();
            }

            if (ScenarioParser.TryParse(line, out var commands, out var error))
            {
                return commands;
            }

            WriteError(error!);
        }
    }

    public static bool AskPlayAgain()
    {
        Console.Write("Tekrar oynamak ister misiniz? (E/H): ");
        var answer = Console.ReadLine()?.Trim();
        return answer is not null && answer.Equals("E", StringComparison.OrdinalIgnoreCase);
    }

    private static void WriteError(string message)
    {
        var previous = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ForegroundColor = previous;
    }
}
