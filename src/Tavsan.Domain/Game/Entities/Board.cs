using Tavsan.Domain.Game.Models;
using Tavsan.Domain.Game.Enums;

namespace Tavsan.Domain.Game.Entities;

public sealed class Board
{
    public const int MaxObstaclesPerType = 4;

    private static readonly int[] AllowedSizes = { 4, 8, 16 };

    private readonly Dictionary<Position, ObstacleType> _obstacles;

    public Board(int size, IEnumerable<KeyValuePair<Position, ObstacleType>>? obstacles = null)
    {
        if (!IsValidSize(size))
        {
            throw new ArgumentOutOfRangeException(nameof(size), size, "Orman büyüklüğü 4, 8 veya 16 olmalıdır.");
        }

        Size = size;
        Start = new Position(0, 0);
        Hole = new Position(size - 1, size - 1);
        _obstacles = new Dictionary<Position, ObstacleType>();

        foreach (var (position, type) in obstacles ?? Enumerable.Empty<KeyValuePair<Position, ObstacleType>>())
        {
            AddObstacle(position, type);
        }
    }

    public int Size { get; }

    public Position Start { get; }

    public Position Hole { get; }

    public IReadOnlyDictionary<Position, ObstacleType> Obstacles => _obstacles;

    public static IReadOnlyList<int> ValidSizes => AllowedSizes;

    public static bool IsValidSize(int size) => Array.IndexOf(AllowedSizes, size) >= 0;

    public bool Contains(Position position) =>
        position.Column >= 0 && position.Column < Size && position.Row >= 0 && position.Row < Size;

    public ObstacleType? ObstacleAt(Position position) =>
        _obstacles.TryGetValue(position, out var type) ? type : null;

    public int Count(ObstacleType type) => _obstacles.Values.Count(o => o == type);

    public string ToNotation(Position position) => $"{(char)('A' + position.Column)}{Size - position.Row}";

    private void AddObstacle(Position position, ObstacleType type)
    {
        if (!Contains(position))
        {
            throw new ArgumentException($"Engel orman dışında olamaz: ({position.Column}, {position.Row}).");
        }

        if (position == Start || position == Hole)
        {
            throw new ArgumentException("Başlangıç ve tavşan deliği karesine engel konulamaz.");
        }

        if (_obstacles.ContainsKey(position))
        {
            throw new ArgumentException($"Aynı kareye birden fazla engel konulamaz: {ToNotation(position)}.");
        }

        if (Count(type) >= MaxObstaclesPerType)
        {
            throw new ArgumentException($"{type} için en fazla {MaxObstaclesPerType} engel konulabilir.");
        }

        _obstacles.Add(position, type);
    }
}
