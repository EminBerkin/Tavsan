using Tavsan.Domain.Game.Entities;

namespace Tavsan.Application.Game.Services;

public interface IObstaclePlacer
{
    Board CreateBoard(int size);
}
