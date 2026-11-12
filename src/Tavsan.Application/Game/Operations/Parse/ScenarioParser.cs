using Tavsan.Domain.Game.Enums;

namespace Tavsan.Application.Game.Operations.Parse;

public static class ScenarioParser
{
    public static bool TryParse(string? input,
                                out IReadOnlyList<Command> commands,
                                out string? error)
    {
        var result = new List<Command>();
        commands = result;
        error = null;

        if (string.IsNullOrWhiteSpace(input))
        {
            error = "Senaryo boş olamaz.";
            return false;
        }

        var position = 0;
        foreach (var symbol in input)
        {
            if (char.IsWhiteSpace(symbol) || symbol == ',')
            {
                continue;
            }

            position++;

            if (!TryMap(symbol, out var command))
            {
                error = $"{position}. komut geçersiz: '{symbol}'. Geçerli komutlar: N, P, R, L, J, İ";
                result.Clear();
                return false;
            }

            result.Add(command);
        }

        if (result.Count == 0)
        {
            error = "Senaryo en az bir komut içermelidir.";
            return false;
        }

        return true;
    }

    private static bool TryMap(char symbol, out Command command)
    {
        var mapped = symbol switch
        {
            'N' or 'n' => Command.Forward,
            'P' or 'p' => Command.Back,
            'R' or 'r' => Command.Right,
            'L' or 'l' => Command.Left,
            'J' or 'j' => Command.Jump,
            'İ' or 'i' or 'I' or 'ı' => Command.Crouch,
            _ => (Command?)null
        };

        command = mapped ?? default;
        return mapped.HasValue;
    }
}
