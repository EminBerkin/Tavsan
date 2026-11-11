using Tavsan.Core.Domain;

namespace Tavsan.Core.Services;

public interface IObstaclePlacer
{
    Board CreateBoard(int size);
}
