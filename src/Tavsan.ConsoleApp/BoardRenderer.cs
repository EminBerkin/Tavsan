using System.Text;
using Tavsan.Core.Domain;

namespace Tavsan.ConsoleApp;

public static class BoardRenderer
{
    private const int CellWidth = 5;

    public static void Render(Board board, Position? rabbit = null)
    {
        var current = rabbit ?? board.Start;

        WriteHeader(board);

        for (var row = 0; row < board.Size; row++)
        {
            Console.Write($"{board.Size - row,2} ");

            for (var column = 0; column < board.Size; column++)
            {
                var position = new Position(column, row);
                var (text, color) = Describe(board, position, current);
                WriteCell(text, color);
            }

            Console.WriteLine();
        }

        Console.WriteLine();
        WriteLegend();
    }

    private static void WriteHeader(Board board)
    {
        var header = new StringBuilder("   ");
        for (var column = 0; column < board.Size; column++)
        {
            header.Append($" {(char)('A' + column)}".PadRight(CellWidth));
        }

        Console.WriteLine(header);
    }

    private static (string Text, ConsoleColor Color) Describe(Board board, Position position, Position rabbit)
    {
        if (position == rabbit)
        {
            return ("TAV", ConsoleColor.Cyan);
        }

        return board.ObstacleAt(position) switch
        {
            ObstacleType.Wolf => ("KRT", ConsoleColor.Red),
            ObstacleType.Fox => ("TLK", ConsoleColor.DarkYellow),
            ObstacleType.Wire => ("TEL", ConsoleColor.Magenta),
            ObstacleType.Fence => ("ÇİT", ConsoleColor.Yellow),
            _ when position == board.Hole => ("DEL", ConsoleColor.Green),
            _ => (" . ", ConsoleColor.DarkGray)
        };
    }

    private static void WriteCell(string text, ConsoleColor color)
    {
        var previous = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(text.PadRight(CellWidth - 1));
        Console.ForegroundColor = previous;
        Console.Write(' ');
    }

    private static void WriteLegend()
    {
        Console.WriteLine("TAV: Tavşan | KRT: Kurt | TLK: Tilki | TEL: Dikenli tel | ÇİT: Çit | DEL: Tavşan deliği");
    }
}
